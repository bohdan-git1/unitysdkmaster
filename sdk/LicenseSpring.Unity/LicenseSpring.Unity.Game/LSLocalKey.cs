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
            //          0               1                   2               3           4
            return $"{ApplicationName}#{ApplicationVersion}#{ProductCode}#{SharedKey}#{ApiKey}";
        }

        public static LSLocalKey FromString(string value)
        {
            var split = value.Trim().Split('#');

            return new LSLocalKey
            {
                ApiKey = split[4],
                ApplicationName = split[0],
                ApplicationVersion = split[1],
                ProductCode = split[2],
                SharedKey = split[3]
            };
        }
    }
}
