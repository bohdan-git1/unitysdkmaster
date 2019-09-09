using System;
using System.Collections.Generic;
using System.Text;

namespace LicenseSpring.Unity.Assets
{
    public interface ILicenseBehaviour
    {
        bool AllowableFeature { get; }
        void CheckAllowableFeature();
    }
}
