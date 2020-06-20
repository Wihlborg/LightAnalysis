using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    class Image
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public string url { get; set; }
        public string email { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string caption { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int avgBrightness { get; set; }
        public int avgRed { get; set; }
        public int avgGreen { get; set; }
        public int avgBlue { get; set; }

    }
}
