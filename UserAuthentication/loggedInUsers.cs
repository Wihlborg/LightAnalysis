using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAuthentication
{
    class loggedInUsers
    {
        public string id { get; set; }
        public long lastActivityTimeStamp { get; set; }

        public string email { get; set; }
    }
}
