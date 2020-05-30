using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace UserAuthentication
{
    class Dal
    {
        private string host = "ralle.mongo.cosmos.azure.com";
        private string password = "TSsI4KkOZ4SJHpvT4UR3Vz2ytAzHbENPOZYbuABCNUoDToltFejnBckMlvIR83rhZCJRfkYHUQXTaWqq3SNrlQ==";
        private string userName = "ralle";

        static public string dbName = "lab3";
        public MongoClient client;
        public Dal()
        {

            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(host, 10255);
            settings.UseTls = true;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            settings.RetryWrites = false;

            MongoIdentity identity = new MongoInternalIdentity(dbName, userName);
            MongoIdentityEvidence evidence = new PasswordEvidence(password);

            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            client = new MongoClient(settings);

        }
    }
}
