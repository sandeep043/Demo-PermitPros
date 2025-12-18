using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Config
{
    internal class AppSettings
    {
        public string BaseUrl { get; set; }
        public string BrowserType { get; set; }
        public bool Headless { get; set; }
        public int Timeout { get; set; }
        public int SlowMo { get; set;  }
    }
}
