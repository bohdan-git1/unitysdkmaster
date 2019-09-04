using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseSpring.Unity
{
    /// <summary>
    /// License Spring Behaviour Interface
    /// </summary>
    public interface ILSPBehaviour
    {
        bool AllowableFeature { get; }
    }
}
