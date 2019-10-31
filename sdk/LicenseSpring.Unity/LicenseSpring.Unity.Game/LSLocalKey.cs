using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LicenseSpring.Unity.Game
{
    /// <summary>
    /// License Spring Local Key.
    /// </summary>
    public class LSLocalKey : ScriptableObject
    {
        public string ApiKey { get; set; }
        public string SharedKey { get; set; }
        public string ProductCode { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }

        public override string ToString()
        {
            return $"{ApplicationName}.{ApplicationVersion}-{SharedKey}-{ApiKey}";
        }
       
    }
}
