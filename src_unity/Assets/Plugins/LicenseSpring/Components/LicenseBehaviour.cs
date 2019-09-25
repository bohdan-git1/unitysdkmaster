using LicenseSpring.Unity.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LicenseSpring.Unity.Components
{
    public class LicenseBehaviour : MonoBehaviour, ILicenseBehaviour
    {
        public bool AllowableFeature { get; }

        public void CheckAllowableFeature()
        {
            
        }
    }
}
