﻿using System;
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
        RightController
    }

    public EType Type;
    public EIndex Index;
    public Transform Origin; // if not set, relative to parent
    public bool IsValid;

    private EType _type;

    private void OnNewPoses(params TrackedDevicePose_t[] args)
    {
        if (_type != Type)
        {
            _type = Type;
            IsValid = false;
        }

        if (!IsValid)
        {
            Index = EIndex.None;
            if (Type != EType.None)
            {
                switch (Type)
                {
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        IsValid = false;

        if (Index == EIndex.None)
            return;

        var i = (int) Index;

        var poses = args;
        if (poses.Length <= i)
            return;

        if (!poses[i].bDeviceIsConnected)
            return;

        if (!poses[i].bPoseIsValid)
            return;

        IsValid = true;

        var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
        
        if (Origin != null)
        {
            pose = new SteamVR_Utils.RigidTransform(Origin)*pose;
            pose.pos.x *= Origin.localScale.x;
            pose.pos.y *= Origin.localScale.y;
            pose.pos.z *= Origin.localScale.z;
            transform.position = pose.pos;
            transform.rotation = pose.rot;
        }
        else
        {
            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }
    }

    public void Start()
    {
        HOTK_TrackedDeviceManager.OnControllerIndexChanged += OnControllerIndexChanged;
    }

    // If the controller we are tracking changes index, update
    private void OnControllerIndexChanged(ETrackedControllerRole role, uint index)
    {
        if (Type == EType.LeftController && role == ETrackedControllerRole.LeftHand)
        {
            Reset();
        }
        else if(Type == EType.RightController && role == ETrackedControllerRole.RightHand)
        {
            Reset();
        }
    }

    public void OnEnable()
    {
        Reset();
        SteamVR_Events.NewPoses.Listen(OnNewPoses);
    }

    public void OnDisable()
    {
        SteamVR_Events.NewPoses.Remove(OnNewPoses);
        Reset();
    }

    private void Reset()
    {
        Index = EIndex.None;
        IsValid = false;
    }
}
