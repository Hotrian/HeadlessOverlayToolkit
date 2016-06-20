using System;
using System.Linq;
using UnityEngine;
using Valve.VR;

public class HOTK_Overlay : MonoBehaviour
{
    #region Custom Inspector Vars
    [NonSerialized] public bool ShowSettingsAppearance = true; 
    [NonSerialized] public bool ShowSettingsInput = false;
    [NonSerialized] public bool ShowSettingsAttachment = false;
    #endregion
    
    #region Settings
    [Tooltip("The texture that will be drawn for the Overlay.")]
    public Texture OverlayTexture;
    [Tooltip("How, if at all, the Overlay is animated when being looked at.")]
    public AnimationType AnimateOnGaze;
    [Tooltip("The alpha at which the Overlay will be drawn.")]
    public float Alpha = 1.0f;			// opacity 0..1
    [Tooltip("The alpha at which the Overlay will be drawn.")]
    public float Alpha2 = 1.0f;			// opacity 0..1 - Only used for AnimateOnGaze
    [Tooltip("The scale at which the Overlay will be drawn.")]
    public float Scale = 1.0f;			// size of overlay view
    [Tooltip("The scale at which the Overlay will be drawn.")]
    public float Scale2 = 1.0f;			// size of overlay view - Only used for AnimateOnGaze
    [Tooltip("The speed of the animation.")]
    public float AnimateSpeed = 0.1f;
    [Tooltip("This causes the Overlay to draw directly to the screen, instead of to the VRCompositor.")]
    public bool Highquality;            // Only one Overlay can be HQ at a time
    [Tooltip("This causes the Overlay to draw with Anti-Aliasing. Requires High Quality.")]
    public bool Antialias;
    [Tooltip("This causes the Overlay to draw curved. Requires High Quality.")]
    public bool Curved;

    public Vector4 UvOffset = new Vector4(0, 0, 1, 1);
    public Vector2 MouseScale = Vector3.one;
    public Vector2 CurvedRange = new Vector2(1, 2);
    public VROverlayInputMethod InputMethod = VROverlayInputMethod.None;

    [Tooltip("Controls where the Overlay will be drawn.")]
    public AttachmentDevice AnchorDevice = AttachmentDevice.Screen;
    [Tooltip("Controls the base offset for the Overlay.")]
    public AttachmentPoint AnchorPoint = AttachmentPoint.Center;
    [Tooltip("Controls the offset for the Overlay.")]
    public Vector3 AnchorOffset = Vector3.zero;
    #endregion

    #region Interal Vars
    public static HOTK_Overlay HighQualityOverlay; // Only one Overlay can be HQ at a time
    public static string Key { get { return "unity:" + Application.companyName + "." + Application.productName; } }
    public static GameObject ZeroReference;
    public GameObject OverlayReference;

    private Texture _overlayTexture;
    private float _alpha; // Only used for AnimateOnGaze
    private float _scale; // Only used for AnimateOnGaze
    private AttachmentDevice _anchorDevice;
    private AttachmentPoint _anchorPoint;
    private Vector3 _anchorOffset = Vector3.zero;
    private Quaternion _anchorRotation = Quaternion.identity;
    private Quaternion _objectRotation = Quaternion.identity;
    private ulong _handle = OpenVR.k_ulOverlayHandleInvalid;
    private uint _anchor;
    private HOTK_TrackedDevice _hmdTracker;
    private MeshRenderer MeshRenderer
    {
        get { return _meshRenderer ?? (_meshRenderer = GetComponent<MeshRenderer>()); }
    }
    private MeshRenderer _meshRenderer;
    #endregion

    public void Start()
    {
        _scale = Scale;
        _alpha = Alpha;
        if (_hmdTracker != null) return;
        // Try to find an HOTK_TrackedDevice that is active and tracking the HMD
        foreach (var g in FindObjectsOfType<HOTK_TrackedDevice>().Where(g => g.enabled && g.Type == HOTK_TrackedDevice.EType.HMD))
        {
            _hmdTracker = g;
            break;
        }
    }

    public void Update()
    {
        if (MeshRenderer != null && _overlayTexture != OverlayTexture)
        {
            _overlayTexture = OverlayTexture;
            MeshRenderer.material.mainTexture = OverlayTexture;
        }
        UpdateOverlay();
        if (_anchorDevice != AnchorDevice ||_anchorPoint != AnchorPoint ||_anchorOffset != AnchorOffset)
            AttachTo(AnchorDevice, Scale, AnchorOffset, AnchorPoint);
        UpdateOverlayRotation();
        UpdateGaze();
    }

    public void OnEnable()
    {
        #pragma warning disable 0168
        // ReSharper disable once UnusedVariable
        var svr = SteamVR.instance; // Init the SteamVR drivers
        #pragma warning restore 0168
        var overlay = OpenVR.Overlay;
        if (overlay == null) return;
        var error = overlay.CreateOverlay(Key + gameObject.GetInstanceID(), gameObject.name, ref _handle);
        if (error == EVROverlayError.None) return;
        Debug.Log(error.ToString());
        enabled = false;
    }

    public void OnDisable()
    {
        if (_handle == OpenVR.k_ulOverlayHandleInvalid) return;
        var overlay = OpenVR.Overlay;
        if (overlay != null) overlay.DestroyOverlay(_handle);
        _handle = OpenVR.k_ulOverlayHandleInvalid;
    }

    private void UpdateOverlay()
    {
        var overlay = OpenVR.Overlay;
        if (overlay == null) return;

        if (OverlayTexture != null)
        {
            var error = overlay.ShowOverlay(_handle);
            if (error == EVROverlayError.InvalidHandle || error == EVROverlayError.UnknownOverlay)
            {
                if (overlay.FindOverlay(Key, ref _handle) != EVROverlayError.None) return;
            }

            var tex = new Texture_t
            {
                handle = OverlayTexture.GetNativeTexturePtr(),
                eType = SteamVR.instance.graphicsAPI,
                eColorSpace = EColorSpace.Auto
            };

            overlay.SetOverlayTexture(_handle, ref tex);
            overlay.SetOverlayAlpha(_handle, AnimateOnGaze == AnimationType.Alpha ? _alpha : Alpha);
            overlay.SetOverlayWidthInMeters(_handle, AnimateOnGaze == AnimationType.Scale ? _scale : Scale);
            overlay.SetOverlayAutoCurveDistanceRangeInMeters(_handle, CurvedRange.x, CurvedRange.y);

            var textureBounds = new VRTextureBounds_t
            {
                uMin = (0 + UvOffset.x)*UvOffset.z,
                vMin = (1 + UvOffset.y)*UvOffset.w,
                uMax = (1 + UvOffset.x)*UvOffset.z,
                vMax = (0 + UvOffset.y)*UvOffset.w
            };
            overlay.SetOverlayTextureBounds(_handle, ref textureBounds);

            var vecMouseScale = new HmdVector2_t
            {
                v0 = MouseScale.x,
                v1 = MouseScale.y
            };
            overlay.SetOverlayMouseScale(_handle, ref vecMouseScale);

            var vrcam = SteamVR_Render.Top();
            if (_anchor != OpenVR.k_unTrackedDeviceIndexInvalid) // Attached to some SteamVR_TrackedObject
            {
                var t = UpdateOverlayPosition();
                overlay.SetOverlayTransformTrackedDeviceRelative(_handle, _anchor, ref t);
            }
            else if (AnchorDevice == AttachmentDevice.World) // Attached to World
            {
                var t = UpdateOverlayPosition();
                overlay.SetOverlayTransformAbsolute(_handle, SteamVR_Render.instance.trackingSpace, ref t);
            }
            else if (vrcam != null && vrcam.origin != null) // Attached to Camera
            {
                var offset = new SteamVR_Utils.RigidTransform(vrcam.origin, transform);
                offset.pos.x /= vrcam.origin.localScale.x;
                offset.pos.y /= vrcam.origin.localScale.y;
                offset.pos.z /= vrcam.origin.localScale.z;

                var t = offset.ToHmdMatrix34();
                overlay.SetOverlayTransformAbsolute(_handle, SteamVR_Render.instance.trackingSpace, ref t);
            }
            else // HMD not found :(
            {
                var t = UpdateOverlayPosition();
                overlay.SetOverlayTransformTrackedDeviceRelative(_handle, 0, ref t);
            }

            overlay.SetOverlayInputMethod(_handle, InputMethod);

            if (Highquality)
            {
                if (HighQualityOverlay != this && HighQualityOverlay != null)
                {
                    if (HighQualityOverlay.Highquality)
                    {
                        Debug.LogWarning("Only one Overlay can be in HighQuality mode as per the OpenVR API.");
                        HighQualityOverlay.Highquality = false;
                    }
                    HighQualityOverlay = this;
                }else if (HighQualityOverlay == null)
                    HighQualityOverlay = this;
                overlay.SetHighQualityOverlay(_handle);
                overlay.SetOverlayFlag(_handle, VROverlayFlags.Curved, Curved);
                overlay.SetOverlayFlag(_handle, VROverlayFlags.RGSS4X, Antialias);
            }
            else if (overlay.GetHighQualityOverlay() == _handle)
            {
                overlay.SetHighQualityOverlay(OpenVR.k_ulOverlayHandleInvalid);
            }
        }
        else
        {
            overlay.HideOverlay(_handle);
        }
    }

    /// <summary>
    /// Update the Overlay's Rotation if necessary
    /// </summary>
    /// <param name="force"></param>
    private void UpdateOverlayRotation(bool force = false)
    {
        if (_anchor == OpenVR.k_unTrackedDeviceIndexInvalid) return;
        if (OverlayReference == null) return;
        if (!force && _objectRotation == gameObject.transform.localRotation) return;
        _objectRotation = gameObject.transform.localRotation;
        OverlayReference.transform.localRotation = _anchorRotation * _objectRotation;
    }

    /// <summary>
    /// Update the Overlay's Position and return the resulting HmdMatrix34_t
    /// </summary>
    /// <returns></returns>
    private HmdMatrix34_t UpdateOverlayPosition()
    {
        if (OverlayReference == null) OverlayReference = new GameObject("Overlay Reference") {hideFlags = HideFlags.HideInHierarchy};
        if (_anchor == OpenVR.k_unTrackedDeviceIndexInvalid)
        {
            var offset = new SteamVR_Utils.RigidTransform(OverlayReference.transform, transform);
            offset.pos.x /= OverlayReference.transform.localScale.x;
            offset.pos.y /= OverlayReference.transform.localScale.y;
            offset.pos.z /= OverlayReference.transform.localScale.z;
            var t = offset.ToHmdMatrix34();
            return t;
        }
        else
        {
            if (ZeroReference == null) ZeroReference = new GameObject("Zero Reference") {hideFlags = HideFlags.HideInHierarchy};
            var offset = new SteamVR_Utils.RigidTransform(ZeroReference.transform, OverlayReference.transform);
            offset.pos.x /= ZeroReference.transform.localScale.x;
            offset.pos.y /= ZeroReference.transform.localScale.y;
            offset.pos.z /= ZeroReference.transform.localScale.z;
            var t = offset.ToHmdMatrix34();
            return t;
        }
    }

    private void UpdateGaze()
    {
        var hit = false;
        if (_hmdTracker != null && _hmdTracker.IsValid)
        {
            var result = new IntersectionResults();
            hit = ComputeIntersection(_hmdTracker.gameObject.transform.position, new Vector3(0f, 0f, 1f), ref result);
            //Debug.Log("Hit! " + gameObject.name);
        }
        HandleAnimateOnGaze(hit);
    }

    /// <summary>
    /// Animate this Overlay, based on it's AnimateOnGaze setting.
    /// </summary>
    /// <param name="hit"></param>
    private void HandleAnimateOnGaze(bool hit)
    {
        if (hit)
        {
            switch (AnimateOnGaze)
            {
                case AnimationType.Alpha:
                    if (Alpha < Alpha2)
                    {
                        if (_alpha < Alpha2)
                        {
                            _alpha += AnimateSpeed;
                            if (_alpha > Alpha2)
                                _alpha = Alpha2;
                        }
                    }
                    else // Not sure why you'd want it to fade out on gaze, but just in case
                    {
                        if (_alpha > Alpha2)
                        {
                            _alpha -= AnimateSpeed;
                            if (_alpha < Alpha2)
                                _alpha = Alpha2;
                        }
                    }
                    break;
                case AnimationType.Scale:
                    if (Scale < Scale2)
                    {
                        if (_scale < Scale2)
                        {
                            _scale += AnimateSpeed;
                            if (_scale > Scale2)
                                _scale = Scale2;
                        }
                    }
                    else // Not sure why you'd want it to scale down on gaze, but just in case
                    {
                        if (_scale > Scale2)
                        {
                            _scale -= AnimateSpeed;
                            if (_scale < Scale2)
                                _scale = Scale2;
                        }
                    }
                    break;
            }
        }
        else
        {
            switch (AnimateOnGaze)
            {
                case AnimationType.Alpha:
                    if (Alpha < Alpha2)
                    {
                        if (_alpha > Alpha)
                        {
                            _alpha -= AnimateSpeed;
                            if (_alpha < Alpha)
                                _alpha = Alpha;
                        }
                    }
                    else // Not sure why you'd want it to fade in when you look away from it, but just in case
                    {
                        if (_alpha < Alpha)
                        {
                            _alpha += AnimateSpeed;
                            if (_alpha > Alpha)
                                _alpha = Alpha;
                        }
                    }
                    break;
                case AnimationType.Scale:
                    if (Scale < Scale2)
                    {
                        if (_scale > Scale)
                        {
                            _scale -= AnimateSpeed;
                            if (_scale < Scale)
                                _scale = Scale;
                        }
                    }
                    else // Not sure why you'd want it to scale up when you look away from it, but just in case
                    {
                        if (_scale < Scale)
                        {
                            _scale += AnimateSpeed;
                            if (_scale > Scale)
                                _scale = Scale;
                        }
                    }
                    break;
            }
        }
    }

    /*private bool PollNextEvent(ref VREvent_t pEvent)
    {
        var overlay = OpenVR.Overlay;
        if (overlay == null) return false;
        var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));
        return overlay.PollNextOverlayEvent(_handle, ref pEvent, size);
    }*/

    private bool ComputeIntersection(Vector3 source, Vector3 direction, ref IntersectionResults results)
    {
        var overlay = OpenVR.Overlay;
        if (overlay == null) return false;

        var input = new VROverlayIntersectionParams_t
        {
            eOrigin = SteamVR_Render.instance.trackingSpace,
            vSource =
            {
                v0 = source.x,
                v1 = source.y,
                v2 = -source.z
            },
            vDirection =
            {
                v0 = direction.x,
                v1 = direction.y,
                v2 = -direction.z
            }
        };

        var output = new VROverlayIntersectionResults_t();
        if (!overlay.ComputeOverlayIntersection(_handle, ref input, ref output)) return false;

        results.Point = new Vector3(output.vPoint.v0, output.vPoint.v1, -output.vPoint.v2);
        results.Normal = new Vector3(output.vNormal.v0, output.vNormal.v1, -output.vNormal.v2);
        results.UVs = new Vector2(output.vUVs.v0, output.vUVs.v1);
        results.Distance = output.fDistance;
        return true;
    }

    /// <summary>
    /// Attach the Overlay to [device] at base position [point].
    /// [point] isn't used for HMD or World, and can be ignored.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="point"></param>
    public void AttachTo(AttachmentDevice device, AttachmentPoint point = AttachmentPoint.Center)
    {
        AttachTo(device, 1f, Vector3.zero, point);
    }
    /// <summary>
    /// Attach the Overlay to [device] at [scale], and base position [point].
    /// [point] isn't used for HMD or World, and can be ignored.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="scale"></param>
    /// <param name="point"></param>
    public void AttachTo(AttachmentDevice device, float scale, AttachmentPoint point = AttachmentPoint.Center)
    {
        AttachTo(device, scale, Vector3.zero, point);
    }
    /// <summary>
    /// Attach the Overlay to [device] at [scale] size with offset [offset], and base position [point].
    /// [point] isn't used for HMD or World, and can be ignored.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="scale"></param>
    /// <param name="offset"></param>
    /// <param name="point"></param>
    public void AttachTo(AttachmentDevice device, float scale, Vector3 offset, AttachmentPoint point = AttachmentPoint.Center)
    {
        UpdateOverlayPosition();

        var manager = HOTK_TrackedDeviceManager.Instance;
        _anchorDevice = device;
        AnchorDevice = device;
        _anchorPoint = point;
        AnchorPoint = point;
        _anchorOffset = offset;
        AnchorOffset = offset;
        Scale = scale;

        switch (device)
        {
            case AttachmentDevice.Screen:
                _anchor = OpenVR.k_unTrackedDeviceIndexInvalid;
                OverlayReference.transform.localPosition = -offset;
                OverlayReference.transform.localRotation = Quaternion.identity;
                break;
            case AttachmentDevice.World:
                _anchor = OpenVR.k_unTrackedDeviceIndexInvalid;
                OverlayReference.transform.localPosition = -offset;
                OverlayReference.transform.localRotation = Quaternion.identity;
                break;
            case AttachmentDevice.LeftController:
                _anchor = manager.LeftIndex;
                AttachToController(point, offset);
                break;
            case AttachmentDevice.RightController:
                _anchor = manager.RightIndex;
                AttachToController(point, offset);
                break;
            default:
                throw new ArgumentOutOfRangeException("device", device, null);
        }
    }

    /// <summary>
    /// Update the Overlay's Position and Rotation, relative to the selected controller, attaching it to [point] with offset [offset]
    /// </summary>
    /// <param name="point"></param>
    /// <param name="offset"></param>
    private void AttachToController(AttachmentPoint point, Vector3 offset)
    {
        // Shift Offset for default position
        float dx = offset.x, dy = offset.y, dz = offset.z;
        var rot = Quaternion.identity;
        Vector3 pos;
        switch (point)
        {
            case AttachmentPoint.Center:
                break;
            case AttachmentPoint.FlatAbove:
                dz += 0.05f;
                break;
            case AttachmentPoint.FlatBelow:
                dz -= 0.18f;
                break;
            case AttachmentPoint.FlatBelowFlipped:
                dz += 0.18f;
                break;
            case AttachmentPoint.Above:
                dz -= 0.01f;
                break;
            case AttachmentPoint.AboveFlipped:
                dz += 0.01f;
                break;
            case AttachmentPoint.Below:
                dz += 0.1f;
                break;
            case AttachmentPoint.BelowFlipped:
                dz -= 0.1f;
                break;
            case AttachmentPoint.Up:
                dy += 0.5f;
                break;
            case AttachmentPoint.Down:
                dy -= 0.5f;
                break;
            case AttachmentPoint.Left:
                dx -= 0.5f;
                break;
            case AttachmentPoint.Right:
                dx += 0.5f;
                break;
            default:
                throw new ArgumentOutOfRangeException("point", point, null);
        }

        // Apply position and rotation to Overlay anchor
        switch (point)
        {
            case AttachmentPoint.FlatAbove:
            case AttachmentPoint.FlatBelow:
                pos = new Vector3(dx, dy, dz);
                break;
            case AttachmentPoint.FlatBelowFlipped:
                pos = new Vector3(dx, -dy, -dz);
                rot = Quaternion.AngleAxis(180f, new Vector3(1f, 0f, 0f));
                break;
            case AttachmentPoint.Center:
            case AttachmentPoint.Above:
            case AttachmentPoint.Below:
                pos = new Vector3(dx, -dz, dy);
                rot = Quaternion.AngleAxis(90f, new Vector3(1f, 0f, 0f));
                break;
            case AttachmentPoint.Up:
            case AttachmentPoint.Down:
            case AttachmentPoint.Left:
            case AttachmentPoint.Right:
                pos = new Vector3(dx, -dz, dy);
                rot = Quaternion.AngleAxis(90f, new Vector3(1f, 0f, 0f));
                break;
            case AttachmentPoint.AboveFlipped:
            case AttachmentPoint.BelowFlipped:
                pos = new Vector3(-dx, dz, dy);
                rot = Quaternion.AngleAxis(90f, new Vector3(1f, 0f, 0f)) * Quaternion.AngleAxis(180f, new Vector3(0f, 1f, 0f));
                break;
            default:
                throw new ArgumentOutOfRangeException("point", point, null);
        }
        OverlayReference.transform.localPosition = pos;
        _anchorRotation = rot;
        UpdateOverlayRotation(true); // Force rotational update
    }

    #region Structs and Enums
    public struct IntersectionResults
    {
        public Vector3 Point;
        public Vector3 Normal;
        public Vector2 UVs;
        public float Distance;
    }

    /// <summary>
    /// Used to determine where an Overlay should be attached.
    /// </summary>
    public enum AttachmentDevice
    {
        /// <summary>
        /// Attempts to attach the Overlay to the World
        /// </summary>
        World,
        /// <summary>
        /// Attempts to attach the Overlay to the Screen / HMD
        /// </summary>
        Screen,
        /// <summary>
        /// Attempts to attach the Overlay to the Left Controller
        /// </summary>
        LeftController,
        /// <summary>
        /// Attempts to attach the Overlay to the Right Controller
        /// </summary>
        RightController,
    }

    /// <summary>
    /// Used when attaching Overlays to Controllers, to determine the base attachment offset.
    /// </summary>
    public enum AttachmentPoint
    {
        /// <summary>
        /// Directly in the center at (0, 0, 0), facing upwards through the Trackpad.
        /// </summary>
        Center,
        /// <summary>
        /// At the end of the controller, like a staff ornament, facing towards the center.
        /// </summary>
        FlatAbove,
        /// <summary>
        /// At the bottom of the controller, facing away from the center.
        /// </summary>
        FlatBelow,
        /// <summary>
        /// At the bottom of the controller, facing towards the center.
        /// </summary>
        FlatBelowFlipped,
        /// <summary>
        /// Just above the Trackpad, facing away from the center.
        /// </summary>
        Above,
        /// <summary>
        /// Just above thr Trackpad, facing the center.
        /// </summary>
        AboveFlipped,
        /// <summary>
        /// Just below the Trigger, facing the center.
        /// </summary>
        Below,
        /// <summary>
        /// Just below the Trigger, facing away from the center.
        /// </summary>
        BelowFlipped,
        /// <summary>
        /// When holding the controller out vertically, Like "Center", but "Up", above the controller.
        /// </summary>
        Up,
        /// <summary>
        /// When holding the controller out vertically, Like "Center", but "Down", below the controller.
        /// </summary>
        Down,
        /// <summary>
        /// When holding the controller out vertically, Like "Center", but "Left", to the side of the controller.
        /// </summary>
        Left,
        /// <summary>
        /// When holding the controller out vertically, Like "Center", but "Right", to the side of the controller.
        /// </summary>
        Right,
    }

    public enum AnimationType
    {
        /// <summary>
        /// Don't animate this Overlay.
        /// </summary>
        None,
        /// <summary>
        /// Animate this Overlay by changing its Alpha.
        /// </summary>
        Alpha,
        /// <summary>
        /// Animate this Overlay by scaling it.
        /// </summary>
        Scale,
    }
    #endregion
}