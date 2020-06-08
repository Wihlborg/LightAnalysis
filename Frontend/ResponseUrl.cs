using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend
{
    public class ResponeUrl
    {
        public string sessionId { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public string[] images { get; set; }
        public string[] analyzeTxt { get; set; }
    }
}