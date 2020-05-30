using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAuthentication
{

    class Request
    {
        public const string LOGIN = "LOGIN";
        public const string REGISTER = "REGISTER";
        public const string CHECKIN = "CHECKIN";

        public string method;
        public string id;
        public Account account;
    }
}
