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
using System.Drawing;

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Backend
{
    //worker
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        // azure queues
        private string accountName = "rallestorage";
        private string accountKey = "OLPmb7rXZfl2e+z2xM46/auXeesW9b11JdbRBLzdGzBJnpRglUAHhFpMJAr/PG48AAZHyGfHWTyS9N/P2MSx2g==";

        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        private CloudQueueMessage inMessage, outMessage;

        // azure cosmos
        private readonly MongoClient client = new Dal().client;

        // computer vision
        // subscriptionKey = "0123456789abcdef0123456789ABCDEF"  
        private const string subscriptionKey = "c75235e7bc0e4ac69a273e4c4c4c4193";
        ComputerVisionClient computerVision;


        // Specify the features to return  
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };


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

        private void InitComputerVision()
        {
            computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // You must use the same region as you used to get your subscription  
            // keys. For example, if you got your subscription keys from westus,  
            // replace "westcentralus" with "westus".  
            //  
            // Free trial subscription keys are generated in the "westus"  
            // region. If you use a free trial subscription key, you shouldn't  
            // need to change the region.  

            // Specify the Azure region  
            computerVision.Endpoint = "https://ralleimageanalysis.cognitiveservices.azure.com/";

        }

        // Display the most relevant caption for the image  


        // Analyze a remote image  
        private static async Task AnalyzeRemoteAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            tempStr = "";
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Debug.Print(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            Debug.Print("started computervision analysis!!!!");

            ImageAnalysis analysis =
                await computerVision.AnalyzeImageAsync(imageUrl, features);

            if (analysis.Description.Captions.Count != 0)
            {
                Debug.Print("computerVision:\n");
                Debug.Print(analysis.Description.Captions[0].Text + "\n");
                tempStr = analysis.Description.Captions[0].Text + "\n";
            }
            else
            {
                tempStr = "No description generated.";
            }
        }

        static string tempStr;

        private string GetText(Image image)
        {
            string txt = "";
            txt += "url: " + image.url;
            txt += Environment.NewLine + "email: " + image.email;
            txt += Environment.NewLine + "longitude: " + image.longitude;
            txt += Environment.NewLine + "latitude: " + image.latitude;
            txt += Environment.NewLine + "caption: " + image.caption;
            txt += Environment.NewLine + "Width: " + image.width;
            txt += Environment.NewLine + "Height: " + image.height;
            txt += Environment.NewLine + "avg brightness: " + image.avgBrightness;
            txt += Environment.NewLine + "avg red: " + image.avgRed;
            txt += Environment.NewLine + "avg green: " + image.avgGreen;
            txt += Environment.NewLine + "avg blue: " + image.avgBlue;

            Debug.Print(txt);
            return txt;
        }
        private Image AnalyzePicture(Image image) 
        { 
            // get the bitmap from a url
            System.Net.WebRequest request =
            System.Net.WebRequest.Create(image.url);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
                response.GetResponseStream();
            Bitmap img = new Bitmap(responseStream);
            
            int alphaTotal = 0;
            int redTotal = 0;
            int greenTotal = 0;
            int blueTotal = 0;
            int nrOfPixels = 0;
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);
                    alphaTotal += pixel.A;
                    redTotal += pixel.R;
                    greenTotal += pixel.G;
                    blueTotal += pixel.B;
                    nrOfPixels++;
                }
            }
            stopwatch.Stop();

            image.width = img.Width;
            image.height = img.Height;
            image.avgBrightness = (alphaTotal / nrOfPixels);
            image.avgRed = (redTotal / nrOfPixels);
            image.avgGreen = (greenTotal / nrOfPixels);
            image.avgBlue = (blueTotal / nrOfPixels);
            
            TimeSpan ts = stopwatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Debug.Print("RunTime " + elapsedTime);
            return image;
        }
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            InitComputerVision();
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
                                        txt.Add(GetText(image));
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
                                    Debug.Print("amount of pictures: " + retrieveAllResult.Count);

                                    foreach (Image image in retrieveAllResult)
                                    {
                                        list.Add(image.url);
                                        txt.Add(GetText(image));
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
                                    var t1 = AnalyzeRemoteAsync(computerVision, request.image.url);
                                    t1.Wait(5000);
                                    //Task.WhenAll(t1).Wait(5000);
                                    request.image = AnalyzePicture(request.image);
                                    request.image.caption = tempStr;
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
