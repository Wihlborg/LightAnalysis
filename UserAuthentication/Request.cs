using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAuthentication
{

    class Request
    {
        public static readonly string LOGIN = "LOGIN";
        public static readonly string REGISTER = "REGISTER";
        public static readonly string CHECKIN = "CHECKIN";

        public string method;
        public string id;
        public Account account;
    }
}
