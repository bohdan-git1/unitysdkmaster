using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LicenseSpring.Unity
{
    interface ILicenseSpringMessaging : IEventSystemHandler
    {
        void SetSender(GameObject gameObject);
        void Message(UnityLicenseMessageType messageType, string content);

    }

    public enum UnityLicenseMessageType
    {
        Trial,
        LicenseInActive,
        LicenseExpired,
        LicenseInvalid
    }
}
