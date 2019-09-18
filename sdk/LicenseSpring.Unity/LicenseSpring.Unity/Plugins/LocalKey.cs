using System;
using System.Collections.Generic;
using System.Text;

namespace LicenseSpring.Unity.Plugins
{
    [Serializable]
    public class LocalKey
    { 
        public string ApiKey { get; set; }
        public string SharedKey { get; set; }
        public string ProductCode { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
    }
}
