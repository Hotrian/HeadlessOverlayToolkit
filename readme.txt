This is a stripped down version of the SteamVR that includes just enough code to draw Overlays with Unity into SteamVR, without throwing errors ;]

The demo scene has two overlays. Only one overlay can be 'High Quality' at a time [as defined by the OpenVR API](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::SetHighQualityOverlay).

This was tested using [the null driver](https://www.reddit.com/r/SteamVR/comments/4i40k7/cant_get_steamvr_to_work_with_null_driver/d2uxgh5). It should work perfectly with a Vive HMD, but this hasn't been tested.

If this does not work properly in VR with an actual HMD, check out lines 113-136 of SteamVR_Overlay.cs, as this is where the problem likely lies.

In the event an HMD is not detected, an "Overlay Reference Point" is spawned to anchor the overlays somewhere in the world in relation to the camera.
If you want this attached to an HMD or the controllers, I suspect you'll want to look into SteamVR_TrackedObject.cs, but as I am testing with the null driver this doesn't seem to work properly.