using UnityEngine;
using System.Collections;

public class InvertColors : MonoBehaviour {

    public float mIntensity = 0;
    public Material mMaterial;

    void OnRenderImage (RenderTexture src, RenderTexture dest)
	{
		mMaterial.SetFloat("_blend", mIntensity);
		Graphics.Blit (src, dest, mMaterial);
	}
}
