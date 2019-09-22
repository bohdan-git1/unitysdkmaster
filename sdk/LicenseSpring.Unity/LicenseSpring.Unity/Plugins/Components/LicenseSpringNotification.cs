
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LicenseSpring.Unity.Components
{
    [ExecuteInEditMode]
    public class LicenseSpringNotification : MonoBehaviour
    {
        private LicenseStatus   _appLicenseStatus;
        private Texture         _splashTexture;
        private RenderTexture   _renderTexture;

        public void SetStatus(LicenseStatus licenseStatus)
        {
            _appLicenseStatus = licenseStatus;

            if (_appLicenseStatus == LicenseStatus.Active)
            {
                enabled = false;
            }
            else
            {
                enabled = true;
            }

            AssignRenderTexture();
        }

        void Start()
        {
            AssignRenderTexture();
        }

        private void AssignRenderTexture()
        {
            switch (_appLicenseStatus)
            {
                case LicenseStatus.Unknown:
                    _splashTexture = Resources.Load<Texture>("UI/license_unlicensed");
                    break;
                case LicenseStatus.Inactive:
                    _splashTexture = Resources.Load<Texture>("UI/license_invalid");
                    break;
                case LicenseStatus.Disabled:
                    _splashTexture = Resources.Load<Texture>("UI/license_disabled");
                    break;
                case LicenseStatus.Expired:
                    _splashTexture = Resources.Load<Texture>("UI/license_expired");
                    break;
                default:
                    break;
            }

            if (_splashTexture == null)
                throw new UnityException("Splash Texture must be assigned");
        }

        private void OnPreRender()
        {
            if (_appLicenseStatus != LicenseStatus.Active)
            {
                _renderTexture = RenderTexture.GetTemporary(256, 256, 16);
                Camera.main.targetTexture = _renderTexture;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_appLicenseStatus != LicenseStatus.Active)
            {
                Camera.main.targetTexture = null;
                Graphics.Blit(_splashTexture, null as RenderTexture);
                RenderTexture.ReleaseTemporary(_renderTexture); 
            }
        }
    }
}

