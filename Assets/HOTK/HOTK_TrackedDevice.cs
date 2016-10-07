using System;
using UnityEngine;
using Valve.VR;

public class HOTK_TrackedDevice : MonoBehaviour
{
    public enum EIndex
    {
        None = -1,
        Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
        Device1,
        Device2,
        Device3,
        Device4,
        Device5,
        Device6,
        Device7,
        Device8,
        Device9,
        Device10,
        Device11,
        Device12,
        Device13,
        Device14,
        Device15
    }
    public enum EType
    {
        None = -1,
        HMD,
        LeftController,
        RightController,
        ThirdController,
        FourthController,
    }

    public EType Type;
    public EIndex Index;
    public Transform Origin; // if not set, relative to parent
    public bool IsValid;

    private EType _type;

    private void OnNewPoses(params object[] args)
    {
        // If our Tracked Type changes, we are no longer valid
        if (_type != Type)
        {
            _type = Type;
            IsValid = false;
        }

        // If we aren't valid, try and find our index
        if (!IsValid)
        {
            Index = EIndex.None;
            if (Type != EType.None)
            {
                switch (Type)
                {
                    case EType.None:
                        return;
                    case EType.HMD:
                        if (HOTK_TrackedDeviceManager.Instance.HMDIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
                            Index = (EIndex)HOTK_TrackedDeviceManager.Instance.HMDIndex;
                        break;
                    case EType.LeftController:
                        if (HOTK_TrackedDeviceManager.Instance.LeftIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
                            Index = (EIndex)HOTK_TrackedDeviceManager.Instance.LeftIndex;
                        break;
                    case EType.RightController:
                        if (HOTK_TrackedDeviceManager.Instance.RightIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
                            Index = (EIndex)HOTK_TrackedDeviceManager.Instance.RightIndex;
                        break;
                    case EType.ThirdController:
                        if (HOTK_TrackedDeviceManager.Instance.ThirdIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
                            Index = (EIndex)HOTK_TrackedDeviceManager.Instance.ThirdIndex;
                        break;
                    case EType.FourthController:
                        if (HOTK_TrackedDeviceManager.Instance.FourthIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
                            Index = (EIndex)HOTK_TrackedDeviceManager.Instance.FourthIndex;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // Check if our device is valid

        IsValid = false;

        if (Index == EIndex.None)
            return; // This TrackedDevice is not set to track anything

        var i = (int) Index;

        var poses = (TrackedDevicePose_t[]) args[0];
        if (poses.Length <= i)
            return; // SteamVR did not send poses this update

        if (!poses[i].bDeviceIsConnected)
            return; // SteamVR reports device has been disconnected or was never connected

        if (!poses[i].bPoseIsValid)
            return; // SteamVR reports device is not valid (not being tracked currently)

        IsValid = true;

        // Get our poses and change our position/rotation to match the device

        var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
        
        if (Origin != null) // Our device is 'anchored' to something else in the scene
        {
            pose = new SteamVR_Utils.RigidTransform(Origin)*pose;
            pose.pos.x *= Origin.localScale.x;
            pose.pos.y *= Origin.localScale.y;
            pose.pos.z *= Origin.localScale.z;
            transform.position = pose.pos;
            transform.rotation = pose.rot;
        }
        else // Our device is not attached to anything, use it's raw tracked position
        {
            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }
    }

    public void Start()
    {
        if (Type == EType.HMD) return;
        HOTK_TrackedDeviceManager.OnControllerIndexChanged += OnControllerIndexChanged; // Register our delegate for when a Tracked Device has changed index
        gameObject.SetActive(false); // Disable on Start, TrackedDeviceManager will awake us.
    }

    // If the controller we are tracking changes index, update
    private void OnControllerIndexChanged(EType role, uint index)
    {
        if (role != Type)
            return;
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            return;
        }
        Reset();
    }

    public void OnEnable()
    {
        Reset();
        SteamVR_Utils.Event.Listen("new_poses", OnNewPoses);
    }

    public void OnDisable()
    {
        SteamVR_Utils.Event.Remove("new_poses", OnNewPoses);
        Reset();
    }

    private void Reset()
    {
        Index = EIndex.None;
        IsValid = false;
    }
}
