using System;

namespace LicenseSpring.Unity.Plugins
{
    [Serializable]
    public class LocalKey
    {
        public bool IsDevelopment { get; set; }
        public string ApiKey { get; set; }
        public string SharedKey { get; set; }
        public string ProductCode { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
    }
}
