using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Frontend
{
    public partial class Forgot : System.Web.UI.Page

    {
        private String accountName = "rallestorage";
        private String accountKey = "OLPmb7rXZfl2e+z2xM46/auXeesW9b11JdbRBLzdGzBJnpRglUAHhFpMJAr/PG48AAZHyGfHWTyS9N/P2MSx2g==";
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        private CloudQueueMessage inMessage, outMessage;
        private void initQueue()
        {
            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            inqueue = queueClient.GetQueueReference("authresponsequeue");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("authrequestqueue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonForgot(object sender, EventArgs e)
        {
            initQueue();

            string email = emailForgot.Text;
            Account forgotPassword = new Account();
            UserRequest request = new UserRequest();
            Guid guid = Guid.NewGuid();
            string str = guid.ToString();
            forgotPassword.email = email;
            request.method = UserRequest.FORGOT;
            request.id = str;
            request.account = forgotPassword;
            string jsonString;
            jsonString = JsonSerializer.Serialize(request);
            Debug.WriteLine("DEBUG jsonString: " + jsonString);
            outMessage = new CloudQueueMessage(jsonString);

            if (emailForgot.Text != null)
            {
                outqueue.AddMessage(outMessage);
            }

            //Retrieve stuff
            //Peeking messages to avoid problems
            bool flag = true;
            while (flag)
            {
                CloudQueueMessage peekedMessage = inqueue.PeekMessage();
                if (peekedMessage != null)
                {
                    Debug.WriteLine("DEBUG FORGOT peekedMessage: " + peekedMessage.AsString);

                    if (peekedMessage.AsString.Contains(str))
                    {
                        inMessage = inqueue.GetMessage();
                        flag = false;

                    }
                }
            }
            string response = inMessage.AsString;
            Debug.WriteLine("DEBUG jsonReturn: " + response);
            Response responseObject = JsonSerializer.Deserialize<Response>(response);
            // responseObject.sessionId;
            // responseObject.success;
            // responseObject.msg;


            Debug.WriteLine("DEBUG responseObject.msg: " + responseObject.msg);

            outqueue.Clear();
            inqueue.Clear();

            if (responseObject.success)
            {
                if (responseObject.sessionId.Equals(str))
                {
                    Response.Redirect("Default.aspx", false);
                }
            }
            else
            {
                LabelForgot.Text = responseObject.msg;
            }

            //Response.Redirect("Default.aspx", false);


        }
    }
}
