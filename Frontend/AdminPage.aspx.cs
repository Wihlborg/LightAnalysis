using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Frontend
{
    public partial class AdminPage : System.Web.UI.Page
    {
        static int increment = 0;
        string[] urls = new string[3];
        string[] msg = new string[3];
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
            Debug.Print("page load!!!!");
            initQueue();
            if (Request.QueryString["email"] != null)
            {
                email = Request.QueryString["email"];
                Guid guid = Guid.NewGuid();
                string str = guid.ToString();
                ImageRequest request = new ImageRequest();
                request.method = ImageRequest.RETRIEVEALL;
                Image emailHolder = new Image();
                emailHolder.email = email;
                request.id = str;
                request.image = emailHolder;
                string jsonString;
                jsonString = JsonSerializer.Serialize(request);
                Debug.WriteLine("DEBUG jsonString: " + jsonString);

                outMessage = new CloudQueueMessage(jsonString);
                outqueue.AddMessage(outMessage);
                /*
                //request picture for certain user
                urls[0] = "http://mruanova.com/img/1.jpg";
                urls[1] = "http://image10.bizrate-images.com/resize?sq=60&uid=2216744464";
                urls[2] = "http://www.google.com/intl/en_ALL/images/logo.gif";
                msg[0] = "hihi";
                msg[1] = "huhu";
                msg[2] = "haha";
                */

                bool flag = true;
                while (flag)
                {
                    CloudQueueMessage peekedMessage = inqueue.PeekMessage();
                    if (peekedMessage != null)
                    {
                        Debug.WriteLine("DEBUG adminPage peekedMessage: " + peekedMessage.AsString);

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
            System.Diagnostics.Debug.WriteLine("bajs" + increment);

            if (increment > 0)
            {
                System.Diagnostics.Debug.WriteLine("bajsX" + increment);
                increment--;
                System.Diagnostics.Debug.WriteLine("bajsZ" + increment);
                analyze.Text = msg[increment];
                imageAnalyze.ImageUrl = urls[increment];
            }



        }

        protected void deleteP(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("delete!");

            
            deletePicture deletePic = new deletePicture();
            deletePic.method = UserRequest.DELETE;
            deletePic.email = email;
            deletePic.url = urls[increment];
            deletePic.textAnalyze = msg[increment];
            string jsonString;
            jsonString = JsonSerializer.Serialize(deletePic);
            Debug.WriteLine("DEBUG jsonString: " + jsonString);

            outMessage = new CloudQueueMessage(jsonString);
            outqueue.AddMessage(outMessage);


        }
        protected void nextP(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("bajs" + increment);
            if (increment < urls.Length - 1)
            {
                System.Diagnostics.Debug.WriteLine("bajs1" + increment);
                increment++;
                System.Diagnostics.Debug.WriteLine("bajs2" + increment);
                analyze.Text = msg[increment];
                imageAnalyze.ImageUrl = urls[increment];
            }
            else if (increment >= urls.Length - 1)
            {
                System.Diagnostics.Debug.WriteLine("bajs4" + increment);
                increment = 0;
                analyze.Text = msg[increment];
                imageAnalyze.ImageUrl = urls[increment];
            }


        }

        protected void ExitAdminPage(object sender, EventArgs e) 
        {
            UserRequest request = new UserRequest();
            request.id = (string)Session["id"];
            request.method = UserRequest.LOGOUT;

            string jsonString = JsonSerializer.Serialize(request);
            outMessage = new CloudQueueMessage(jsonString);
            Debug.Print("logout msg: " + jsonString);
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
            Response responseObject = JsonSerializer.Deserialize<Response>(response);
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
            authOutqueue.CreateIfNotExists();
        }
    }
}