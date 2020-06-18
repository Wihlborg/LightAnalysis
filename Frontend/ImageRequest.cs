using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{

    class ImageRequest
    {
        public const string RETRIEVE = "RETRIEVE";
        public const string RETRIEVEALL = "RETRIEVEALL";
        public const string DELETE = "DELETE";
        public const string ADD = "ADD";

        public ImageRequest()
        {
            method = "";
            id = "";
            image = new Image();
        }

        public string method { get; set; }
        public string id { get; set; }
        public Image image { get; set; }
    }
}
