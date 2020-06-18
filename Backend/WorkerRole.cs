using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Driver;


namespace Backend
{
    //worker
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private string accountName = "rallestorage";
        private string accountKey = "OLPmb7rXZfl2e+z2xM46/auXeesW9b11JdbRBLzdGzBJnpRglUAHhFpMJAr/PG48AAZHyGfHWTyS9N/P2MSx2g==";

        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        private CloudQueueMessage inMessage, outMessage;

        private readonly MongoClient client = new Dal().client;
        public override void Run()
        {
            Trace.TraceInformation("Backend is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("Backend has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Backend is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Backend has stopped");
        }

        private void InitQueues()
        {
            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            inqueue = queueClient.GetQueueReference("imagerequestqueue");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("imageresponsequeue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();

            outqueue.Clear();
            inqueue.Clear();
        }

        private string AnalyzePicture(Image image)
        {
            string txt = "";
            txt += "url: "+image.url;
            txt += Environment.NewLine + "email: " + image.email;
            txt += Environment.NewLine + "longitude: " + image.longitude;
            txt += Environment.NewLine + "latitude: " + image.latitude;
            Debug.Print("analyzeTxt:\n" + txt);
            return txt;
        }
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            InitQueues();
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                inMessage = null;
                inMessage = inqueue.GetMessage();

                if (inMessage != null)
                {
                    try
                    {
                        Debug.Print("Back Worker received: " + inMessage.AsString);
                        Request request = JsonSerializer.Deserialize<Request>(inMessage.AsString);
                        var collection = client.GetDatabase(Dal.dbName).GetCollection<Image>("image");
                        Response response = new Response();
                        string jsonResponse;
                        response.sessionId = request.id;

                        switch (request.method)
                        {
                            case Request.RETRIEVE:

                                var retrieveFilter = Builders<Image>.Filter.Eq("email", request.image.email);
                                var retrieveResult = collection.Find(retrieveFilter).ToList();
                                
                                if (retrieveResult.Count > 0)
                                {
                                    
                                    response.success = true;
                                    List<string> list = new List<string>();
                                    List<string> txt = new List<string>();
                                    foreach (Image image in retrieveResult)
                                    {
                                        list.Add(image.url);
                                        txt.Add(AnalyzePicture(image));
                                    }
                                    response.images = list.ToArray();
                                    response.analyzeTxt = txt.ToArray();
                                    response.msg = "images found";
                                    
                                }
                                else
                                {
                                    response.success = false;
                                    response.msg = "ERROR: no images found or email does not exist";
                                }

                                break;

                            case Request.RETRIEVEALL:
                                var retrieveAllFilter = Builders<Image>.Filter.Empty;
                                var retrieveAllResult = collection.Find(retrieveAllFilter).ToList();

                                if (retrieveAllResult.Count > 0)
                                {
                                    response.success = true;
                                    List<string> list = new List<string>();
                                    List<string> txt = new List<string>();
                                    foreach (Image image in retrieveAllResult)
                                    {
                                        list.Add(image.url);
                                        txt.Add(AnalyzePicture(image));
                                    }
                                    response.images = list.ToArray();
                                    response.analyzeTxt = txt.ToArray();
                                    response.msg = "images found";

                                }
                                else
                                {
                                    response.success = false;
                                    response.msg = "ERROR: no images found";
                                }
                                
                                break;

                            case Request.DELETE:
                                var deleteFilter = Builders<Image>.Filter.Eq("url", request.image.url);
                                var deleteResult = collection.DeleteOne(deleteFilter);

                                if (deleteResult.DeletedCount > 0)
                                {
                                    response.success = true;
                                    response.msg = "image deleted";
                                    
                                }
                                else
                                {
                                    response.success = false;
                                    response.msg = "no image to delete was found";
                                }
                                
                                break;

                            case Request.ADD:
                                var addFilter = Builders<Image>.Filter.Eq("url", request.image.url);
                                var addResult = collection.Find(addFilter).ToList();

                                if (addResult.Count == 0)
                                {
                                    collection.InsertOne(request.image);
                                    response.success = true;
                                    response.msg = "image was added";
                                }
                                else
                                {
                                    response.success = false;
                                    response.msg = "url already exists";
                                }
                                break;
                            default:
                                jsonResponse = "ERROR: no valid method was chosen";
                                break;
                        }

                        jsonResponse = JsonSerializer.Serialize<Response>(response);
                        inqueue.DeleteMessage(inMessage);
                        Debug.Print("backend worker response:" + jsonResponse);
                        outMessage = new CloudQueueMessage(jsonResponse);
                        outqueue.AddMessage(outMessage);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("error in backend switch");
                        Debug.Print(ex.StackTrace);
                    }
                
                }

                //Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
