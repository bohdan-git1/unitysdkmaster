﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LicenseSpring.Unity.Assets
{
    [ExecuteInEditMode]
    public class LicenseSpringNotification : MonoBehaviour
    {
        public LicenseStatus AppLicenseStatus;

        private Texture         _splashTexture;
        private RenderTexture   _renderTexture;

        void Start()
        {
            switch (AppLicenseStatus)
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
            _renderTexture = RenderTexture.GetTemporary(256, 256, 16);
            Camera.main.targetTexture = _renderTexture;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Camera.main.targetTexture = null;
            Graphics.Blit(_splashTexture, null as RenderTexture);
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }
}

