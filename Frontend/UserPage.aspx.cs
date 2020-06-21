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
        string[] urls = new string[100];
        string[] msg = new string[100];
        private String accountName = "rallestorage";
        private String accountKey = "OLPmb7rXZfl2e+z2xM46/auXeesW9b11JdbRBLzdGzBJnpRglUAHhFpMJAr/PG48AAZHyGfHWTyS9N/P2MSx2g==";
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue, authInqueue, authOutqueue;
        string email = null;
        private CloudQueueMessage inMessage, outMessage;
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.Print("page load!!!!!!!");
            initQueue();
            if (Request.QueryString["email"] != null)
            {
                email = Request.QueryString["email"];
                string str = (string)Session["id"];
                //UserRequest request = new UserRequest();
                ImageRequest request = new ImageRequest();
                request.method = ImageRequest.RETRIEVE;

                Image emailHolder = new Image();
                emailHolder.email = email;
                //loginFrontend.pw = "XXXXXXXX";
                request.id = str;
                request.image = emailHolder;
                string jsonString;
                jsonString = JsonSerializer.Serialize(request);
                Debug.WriteLine("DEBUG jsonString: " + jsonString);

                outMessage = new CloudQueueMessage(jsonString);
                outqueue.AddMessage(outMessage);

                //request picture for certain user

                //Peek messages until the right id is found to avoid problems 
                bool flag = true;
                while (flag)
                {
                    CloudQueueMessage peekedMessage = inqueue.PeekMessage();
                    if (peekedMessage != null)
                    {
                        Debug.WriteLine("DEBUG userPage peekedMessage: " + peekedMessage.AsString);

                        if (peekedMessage.AsString.Contains(str))
                        {
                            inMessage = inqueue.GetMessage();
                            flag = false;

                        }
                    }
                }

                string response = inMessage.AsString;
                Debug.WriteLine("DEBUG jsonReturn: " + response);
                ResponeUrl responseObject = JsonSerializer.Deserialize<ResponeUrl>(response);
                Debug.WriteLine("DEBUG responseObject.msg: " + responseObject.msg);
                inqueue.DeleteMessage(inMessage);

                if (responseObject.images != null && responseObject.analyzeTxt != null)
                {
                    urls = responseObject.images;
                    msg = responseObject.analyzeTxt;
                }

                if (urls != null && msg != null)
                {
                    analyze.Text = msg[increment];
                    imageAnalyze.ImageUrl = urls[increment];

                }


            }
            else
            {
                Response.Redirect("Default.aspx", false);
            }




        }
        protected void lastP(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("bajs" + increment);

            if (increment > 0)
            {
                System.Diagnostics.Debug.WriteLine("bajsX" + increment);
                increment--;
                System.Diagnostics.Debug.WriteLine("bajsZ" + increment);
                if (urls != null && msg != null)
                {
                    analyze.Text = msg[increment];
                    imageAnalyze.ImageUrl = urls[increment];

                }
            }



        }
        protected void nextP(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("bajs" + increment);
            if (increment < urls.Length - 1)
            {
                System.Diagnostics.Debug.WriteLine("bajs1" + increment);
                increment++;
                System.Diagnostics.Debug.WriteLine("bajs2" + increment);
                if (urls != null && msg != null)
                {
                    analyze.Text = msg[increment];
                    imageAnalyze.ImageUrl = urls[increment];

                }
            }

            else if (increment >= urls.Length - 1)
            {
                System.Diagnostics.Debug.WriteLine("bajs4" + increment);
                increment = 0;
                if (urls != null && msg != null)
                {
                    analyze.Text = msg[increment];
                    imageAnalyze.ImageUrl = urls[increment];

                }
            }


        }

        protected void ExitUserPage(object sender, EventArgs e)
        {
            UserRequest request = new UserRequest();
            request.id = (string)Session["id"];
            request.method = UserRequest.LOGOUT;

            string jsonString = JsonSerializer.Serialize(request);
            outMessage = new CloudQueueMessage(jsonString);
            authOutqueue.AddMessage(outMessage);

            bool flag = true;
            while (flag)
            {
                CloudQueueMessage peekedMessage = authInqueue.PeekMessage();
                if (peekedMessage != null)
                {
                    Debug.WriteLine("DEBUG logout peekedMessage: " + peekedMessage.AsString);

                    if (peekedMessage.AsString.Contains(request.id))
                    {
                        inMessage = authInqueue.GetMessage();
                        flag = false;

                    }
                }
            }

            string response = inMessage.AsString;
            Debug.WriteLine("DEBUG jsonReturn: " + response);
            ResponeUrl responseObject = JsonSerializer.Deserialize<ResponeUrl>(response);
            Debug.WriteLine("DEBUG responseObject.msg: " + responseObject.msg);
            authInqueue.DeleteMessage(inMessage);

            Session.Abandon();
            Response.Redirect("Default.aspx", false);
        }
        private void initQueue()
        {
            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            inqueue = queueClient.GetQueueReference("imageresponsequeue");
            authInqueue = queueClient.GetQueueReference("authresponsequeue");
            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("imagerequestqueue");
            authOutqueue = queueClient.GetQueueReference("authrequestqueue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();
        }
    }
}