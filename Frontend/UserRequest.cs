using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend
{
    class UserRequest
    {
        public const string LOGIN = "LOGIN";
        public const string REGISTER = "REGISTER";
        public const string CHECKIN = "CHECKIN";
        public const string LOGOUT = "LOGOUT";
        public const string RETRIEVE = "RETRIEVE";
        public const string RETRIEVEALL = "RETRIEVEALL";
        public const string DELETE = "DELETE";
        public const string FORGOT = "FORGOT";


        public string method { get; set; }
        public string id { get; set; }
        public Account account { get; set; }
    }
}