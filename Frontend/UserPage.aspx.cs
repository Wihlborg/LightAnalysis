using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;


namespace Frontend
{
     
    public partial class UserPage : System.Web.UI.Page
    {
        static int increment = 0;
        string[] urls=new string[100];
        string[] msg= new string[100];
        private String accountName = "rallestorage";
        private String accountKey = "OLPmb7rXZfl2e+z2xM46/auXeesW9b11JdbRBLzdGzBJnpRglUAHhFpMJAr/PG48AAZHyGfHWTyS9N/P2MSx2g==";
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        string email=null;
        private CloudQueueMessage inMessage, outMessage;
        protected void Page_Load(object sender, EventArgs e)
        {

            initQueue();
            if (Request.QueryString["email"] != null)
            {
                 email = Request.QueryString["email"];
                Guid guid = Guid.NewGuid();
                string str = guid.ToString();
                UserRequest request = new UserRequest();
                request.method = UserRequest.RETRIEVE;
                Account loginFrontend = new Account();
                loginFrontend.email = email;
                loginFrontend.pw = "XXXXXXXX";
                request.id = str;
                request.account = loginFrontend;
                string jsonString;
                jsonString = JsonSerializer.Serialize(request);
                Debug.WriteLine("DEBUG jsonString: " + jsonString);

                outMessage = new CloudQueueMessage(jsonString);
                outqueue.AddMessage(outMessage);

                //request picture for certain user



                Thread.Sleep(5000);
                //Peek messages until the right id is found to avoid problems 
                bool flag = true;
                while (flag)
                {
                    CloudQueueMessage peekedMessage = inqueue.PeekMessage();
                    Debug.WriteLine("DEBUG LOGIN peekedMessage: " + peekedMessage.AsString);

                    if (peekedMessage.AsString.Contains(str))
                    {
                        inMessage = inqueue.GetMessage();
                        flag = false;

                    }
                }

                string response = inMessage.AsString;
                Debug.WriteLine("DEBUG jsonReturn: " + response);
                ResponeUrl responseObject = JsonSerializer.Deserialize<ResponeUrl>(response);
                Debug.WriteLine("DEBUG responseObject.msg: " + responseObject.msg);
                outqueue.Clear();
                inqueue.Clear();





                urls = responseObject.images;

                msg = responseObject.analyzeTxt;
                

            }
            else
            {
                Response.Redirect("Default.aspx", false);
            }




        }
        protected void lastP(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("bajs"+increment);

            if (increment>0)
            {
                System.Diagnostics.Debug.WriteLine("bajsX" + increment);
                increment--; 
                System.Diagnostics.Debug.WriteLine("bajsZ" + increment);
                analyze.Text = msg[increment];
            imageAnalyze.ImageUrl = urls[increment];
            }



        }
        protected void nextP(object sender, EventArgs e)
                    {
            System.Diagnostics.Debug.WriteLine("bajs" + increment);
            if (increment < urls.Length-1)
            {
                System.Diagnostics.Debug.WriteLine("bajs1" + increment);
                increment++;
                System.Diagnostics.Debug.WriteLine("bajs2" + increment);
                analyze.Text = msg[increment];
                imageAnalyze.ImageUrl = urls[increment];
            }
            else if (increment > urls.Length-1)
            {
                System.Diagnostics.Debug.WriteLine("bajs4" + increment);
                increment = 0;
                analyze.Text = msg[increment];
                imageAnalyze.ImageUrl = urls[increment];
            }
           

        }


        private void initQueue()
        {
            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            inqueue = queueClient.GetQueueReference("imageresponsequeue");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("imagerequestqueue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();
        }
    }
}