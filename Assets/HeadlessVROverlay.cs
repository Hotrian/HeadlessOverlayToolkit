using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_Overlay))]
public class HeadlessVROverlay : MonoBehaviour
{

    public Texture2D OverlayTexture
    {
        get { return overlayTexture; }
        set {
                overlayTexture = value;
                SetTexture(overlayTexture);
            }
    }

    private Texture2D overlayTexture;

    private SteamVR_Overlay overlay
    {
        get {
                if (_overlay == null) _overlay = GetComponent<SteamVR_Overlay>();
                return _overlay;
            }
    }
    private MeshRenderer meshRenderer
    {
        get
        {
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
            return _meshRenderer;
        }
    }

    private SteamVR_Overlay _overlay;
    private MeshRenderer _meshRenderer;
    public void SetTexture(Texture2D tex)
    {
        if (overlay != null)
        {
            overlay.texture = tex;
        }
        if (meshRenderer != null)
        {
            meshRenderer.material.mainTexture = tex;
        }

        gameObject.transform.localScale = new Vector3(9f * (tex.width / tex.height), 9f, 1f);
    }

    void Start ()
    {
#pragma warning disable CS0168 // Variable is declared but never used
        var SVR = SteamVR.instance; // Init the SteamVR drivers
#pragma warning restore CS0168 // Variable is declared but never used
    }

    // Update is called once per frame
    void Update()
    {
        if (overlay != null)
        {
            overlay.UpdateOverlay();
        }
	}
}
