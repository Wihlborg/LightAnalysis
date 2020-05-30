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

        private int[] currentUsers;

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
                        Request request = JsonSerializer.Deserialize<Request>(inMessage.AsString);

                        switch (request.method)
                        {
                            case Request.LOGIN:
                                var collection = client.GetDatabase("lightanalysis").GetCollection<Account>("account");

                                var filter = Builders<Account>.Filter.Eq("email", request.account.email);
                                var result = collection.Find(filter).ToList();

                                if (result.ElementAt(0).pw == request.account.pw)
                                {

                                }

                                break;
                            case Request.REGISTER:

                                break;
                            case Request.CHECKIN:

                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("error in login request");
                        Debug.Print(ex.StackTrace);
                    }


                }
                //Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
