using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LicenseSpring.Unity.Plugins.Components
{
    public class LicenseBehaviour : MonoBehaviour, ILicenseBehaviour
    {
        private bool _allowableFeature;

        public bool AllowableFeature => _allowableFeature;

        public void CheckAllowableFeature()
        {

        }

        private void Awake()
        {

        }
    }
}
