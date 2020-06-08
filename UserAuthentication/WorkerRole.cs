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

namespace UserAuthentication
{
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

        private List<loggedInUsers> currentUsers = new List<loggedInUsers>();

        

        public override void Run()
        {
            Trace.TraceInformation("UserAuthentication is running");

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

            Trace.TraceInformation("UserAuthentication has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("UserAuthentication is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("UserAuthentication has stopped");
        }

        private void InitQueues()
        {
            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            inqueue = queueClient.GetQueueReference("authrequestqueue");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("authresponsequeue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();

            outqueue.Clear();
            inqueue.Clear();
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
                        Debug.Print("Worker received: " + inMessage.AsString);
                        Request request = JsonSerializer.Deserialize<Request>(inMessage.AsString);
                        var collection = client.GetDatabase(Dal.dbName).GetCollection<Account>("account");
                        
                        string jsonResponse;
                        Response response = new Response();
                        response.sessionId = request.id;

                        switch (request.method)
                        {
                            case Request.LOGIN:
                                
                                var loginFilter = Builders<Account>.Filter.Eq("email", request.account.email);
                                var loginResult = collection.Find(loginFilter).ToList();

                                
                                if (loginResult.Count > 0)
                                {
                                    if (loginResult.ElementAt(0).pw == request.account.pw)
                                    {

                                        response.success = true;
                                        response.msg = "ADMIN:";
                                        if (loginResult.ElementAt(0).isAdmin)
                                            response.msg += "TRUE";
                                        else
                                            response.msg += "FALSE";
                                        loggedInUsers newUser = new loggedInUsers();
                                        newUser.id = request.id;
                                        DateTime foo = DateTime.UtcNow;
                                        long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
                                        newUser.lastActivityTimeStamp = unixTime;
                                        currentUsers.Add(newUser);

                                    }
                                    else
                                    {
                                        response.success = false;
                                        response.msg = "wrong pw";
                                    }
                                }
                                else
                                {
                                    response.success = false;
                                    response.msg = "email does not exist";
                                }
                                jsonResponse = JsonSerializer.Serialize<Response>(response);
                                

                                break;

                            case Request.REGISTER:
                                var registerFilter = Builders<Account>.Filter.Eq("email", request.account.email);
                                var registerResult = collection.Find(registerFilter).ToList();

                                

                                if (registerResult.Count == 0)
                                {
                                    collection.InsertOne(request.account);
                                    response.success = true;
                                    response.msg = "account added to db";
                                }
                                else
                                {
                                    response.success = false;
                                    response.msg = "Email allready exists";
                                }

                                jsonResponse = JsonSerializer.Serialize<Response>(response);
                                

                                break;

                            case Request.CHECKIN:
                                response.success = false;

                                for (int i = 0; i < currentUsers.Count; i++)
                                {
                                    if (currentUsers.ElementAt(i).id.Equals(request.id))
                                    {
                                        DateTime foo = DateTime.UtcNow;
                                        long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

                                        if ((unixTime - currentUsers.ElementAt(i).lastActivityTimeStamp) < 60 * 10)
                                        {
                                            response.success = true;
                                            currentUsers.ElementAt(i).lastActivityTimeStamp = unixTime;
                                        }
                                        else
                                        {
                                            response.success = false;
                                            currentUsers.RemoveAt(i);
                                        }

                                    }
                                }

                                jsonResponse = JsonSerializer.Serialize<Response>(response);                

                                break;

                            case Request.LOGOUT:
                                response.success = false;
                                for (int i = 0; i < currentUsers.Count; i++)
                                {
                                    if (currentUsers.ElementAt(i).id.Equals(request.id))
                                    {
                                        currentUsers.RemoveAt(i);
                                        response.success = true;
                                    }
                                }

                                jsonResponse = JsonSerializer.Serialize<Response>(response);
                                
                                break;  

                            default:
                                jsonResponse = "ERROR: no valid method was chosen";
                                break;

                            
                        }
                        inqueue.DeleteMessage(inMessage);
                        Debug.Print("response:" + jsonResponse);
                        outMessage = new CloudQueueMessage(jsonResponse);
                        outqueue.AddMessage(outMessage);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("error in switch");
                        Debug.Print(ex.StackTrace);
                    }


                }
                //Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        private void UpdateUserActivity(string id)
        {
            for (int i = 0; i < currentUsers.Count; i++)
            {
                if (currentUsers.ElementAt(i).id.Equals(id))
                {
                    DateTime foo = DateTime.UtcNow;
                    long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

                    if ((unixTime - currentUsers.ElementAt(i).lastActivityTimeStamp) < 60 * 10)
                    {
                       
                        currentUsers.ElementAt(i).lastActivityTimeStamp = unixTime;
                    }
                    else
                    {
                        
                        currentUsers.RemoveAt(i);
                    }

                }
            }
        }

        private string generateId(int nrOfChars)
        {
            char[] chars = {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
            };
            Random rnd = new Random();
            string id = "";
            for (int i = 0; i < nrOfChars; i++)
            {
                id += chars[rnd.Next(0, chars.Length)];
            }

            return id;
        }
    }
}
