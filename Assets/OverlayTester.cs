using UnityEngine;
using System.Collections;

public class OverlayTester : MonoBehaviour
{
    public HeadlessVROverlay Overlay;
    public Texture2D TestTexture;
	void Start ()
    {
        if (Overlay != null && TestTexture != null)
        {
            Overlay.SetTexture(TestTexture);
        }
	}
}
