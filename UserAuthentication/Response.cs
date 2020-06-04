using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAuthentication
{
    class Response
    {
        public string sessionId { get; set; }
        public bool success { get; set; }

        public string token { get; set; }
    }
}
