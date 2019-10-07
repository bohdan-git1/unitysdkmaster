using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Graphic Blitting Examples
/// </summary>
[ExecuteInEditMode]
public class RenderScreenBlit : MonoBehaviour
{
    public Texture LicenseSpringTexture;
    RenderTexture BlitRender;
    public Material BlitMaterial;

    private void Awake()
    {
    }

    private void OnPreRender()
    {
        BlitRender = RenderTexture.GetTemporary(256, 256, 16);
        Camera.main.targetTexture = BlitRender;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Camera.main.targetTexture = null;
        Graphics.Blit(LicenseSpringTexture, null as RenderTexture);
        RenderTexture.ReleaseTemporary(BlitRender);
    }
}
