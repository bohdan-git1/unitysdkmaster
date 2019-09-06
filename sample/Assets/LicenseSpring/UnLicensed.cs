using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LicenseSpring.Unity
{
    public class UnLicensed : MonoBehaviour, ILicenseBehaviour
    {
        public Texture Background;

        RenderTexture _renderTexture;

        public bool AllowableFeature {
            get;
        }

        private void Awake()
        {
            Background = Resources.Load<Texture>("UI/license_spring_unlicensed");
        }

        private void OnPreRender()
        {
            _renderTexture = RenderTexture.GetTemporary(256, 256, 16);
            Camera.main.targetTexture = _renderTexture;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Camera.main.targetTexture = null;
            Graphics.Blit(Background, null as RenderTexture);
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }
}
