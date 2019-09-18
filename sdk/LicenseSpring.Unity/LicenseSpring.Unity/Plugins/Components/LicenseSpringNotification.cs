using UnityEngine;

namespace LicenseSpring.Unity.Plugins.Components
{
    [ExecuteInEditMode]
    public class LicenseSpringNotification : MonoBehaviour
    {
        private License _appLicense;
        private Texture _splashTexture;
        private RenderTexture _renderTexture;

        public void SetStatus(License licenseData)
        {
            if (licenseData == null)
            {
                enabled = true;

                _appLicense = null;
                AssignRenderTexture();

                return;
            }

            _appLicense = licenseData;

            if (_appLicense.Status() == LicenseStatus.Active)
            {
                enabled = false;
            }
            else
            {
                enabled = true;
            }

            if (Application.isEditor)
            {
                AssignRenderTexture();
            }
        }

        void Start()
        {
            AssignRenderTexture();
        }

        private void AssignRenderTexture()
        {
            if (_appLicense == null)
            {
                //if app license is null just show the unlicensed errors.
                _splashTexture = Resources.Load<Texture>("UI/license_unlicensed");
            }
            else
            {
                switch (_appLicense.Status())
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

                if (_appLicense.Status() != LicenseStatus.Active && _splashTexture == null)
                    throw new UnityException("Splash Texture must be assigned");
            }

        }

        private void OnPreRender()
        {
            if (_appLicense?.Status() != LicenseStatus.Active || _appLicense == null)
            {
                _renderTexture = RenderTexture.GetTemporary(256, 256, 16);
                Camera.main.targetTexture = _renderTexture;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_appLicense?.Status() != LicenseStatus.Active || _appLicense == null)
            {
                Camera.main.targetTexture = null;
                Graphics.Blit(_splashTexture, null as RenderTexture);
                RenderTexture.ReleaseTemporary(_renderTexture);
            }
        }
    }
}
