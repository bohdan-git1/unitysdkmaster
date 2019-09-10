using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LicenseSpring.Unity.Assets
{
    public class LicenseSpringNotification : MonoBehaviour
    {
        public Status LicenseStatus;

        private Texture _splashTexture;

        void Start()
        {
            switch (LicenseStatus)
            {
                case Status.Unlicensed:
                    _splashTexture = Resources.Load<Texture>("UI/license_unlicensed");
                    break;
                case Status.LicenseInvalid:
                    _splashTexture = Resources.Load<Texture>("UI/license_invalid");
                    break;
                case Status.TrialExpired:
                    _splashTexture = Resources.Load<Texture>("UI/license_trialexpired");
                    break;
                case Status.LicenseExpired:
                    _splashTexture = Resources.Load<Texture>("UI/license_expired");
                    break;
                default:
                    break;
            }

            if (_splashTexture == null)
                throw new UnityException("Splash Texture must be assigned");
        }
    }
}

}