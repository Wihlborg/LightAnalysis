using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{     
    class Account
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public string email { get; set; }
        public string pw { get; set; }
        public bool isAdmin { get; set; }
    }
}
