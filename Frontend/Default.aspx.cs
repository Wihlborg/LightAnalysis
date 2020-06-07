using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
using Microsoft.Ajax.Utilities;

namespace Frontend
{

    public partial class _Default : Page
    {
        //Strings
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
        
        protected void OnclickRegister(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx", false);

        }


        protected void OnclickLogin(object sender, EventArgs e)
        {

            initQueue();

            if (accountNamez.Text!=null && passwordz.Text!=null)
            {
                string accountName = accountNamez.Text;
                string password = passwordz.Text;
                //Class object for Json String
                //LoginFrontend loginFrontend = new LoginFrontend();
                Guid guid = Guid.NewGuid();
                string str = guid.ToString();
                UserRequest request = new UserRequest();
                Account loginFrontend = new Account();
                loginFrontend.email = accountName;
                loginFrontend.pw = password;
                request.method = UserRequest.LOGIN;
                request.id =str;
                request.account = loginFrontend;
                string jsonString;
                jsonString = JsonSerializer.Serialize(request);
                Debug.WriteLine("DEBUG jsonString: " + jsonString);

                outMessage = new CloudQueueMessage(jsonString);
                outqueue.AddMessage(outMessage);

                //Retrieve stuff
                Thread.Sleep(5000);
                inMessage = inqueue.GetMessage();
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
                        Response.Redirect("UserPage.aspx?email=" + accountName, false);
                    }
                }
                else
                {
                    LabelLogin.Text = responseObject.msg;
                }

              

            }

        }
    }
}