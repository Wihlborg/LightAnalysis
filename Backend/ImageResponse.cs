﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    class ImageResponse
    {
        public string sessionId { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public Image[] images { get; set; }

    }
}
