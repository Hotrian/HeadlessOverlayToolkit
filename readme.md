This is a stripped down version of the SteamVR Unity Plugin that includes just enough code to draw Overlays with Unity into OpenVR without throwing errors ;]

Specifically, this code creates [Overlay](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::CreateOverlay)s which can appear right inside any VR application, not [DashboardOverlay](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::CreateDashboardOverlay)s which can only appear on the dashboard menu.

Specifically, this code creates [Overlay](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::CreateOverlay)s which can appear right inside any VR application, not [DashboardOverlay](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::CreateDashboardOverlay)s which can only appear on the dashboard menu.

The demo scene has two overlays. Only one overlay can be 'High Quality' at a time [as defined by the OpenVR API](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::SetHighQualityOverlay).
However, neither is in HQ mode by default. This can easily be changed by changing [any of these three settings](http://i.imgur.com/6SM7aab.png).

This was tested using [the null driver](https://www.reddit.com/r/SteamVR/comments/4i40k7/cant_get_steamvr_to_work_with_null_driver/d2uxgh5). It should work perfectly with a Vive HMD, but this hasn't been tested.

If this does not work properly in VR with an actual HMD, check out lines 113-136 of SteamVR_Overlay.cs, as this is where the problem likely lies.

In the event an HMD is not detected, an "Overlay Reference Point" is spawned to anchor the overlays somewhere in the world in relation to the camera.
If you want this attached to an HMD or the controllers, I suspect you'll want to look into SteamVR_TrackedObject.cs, but as I am testing with the null driver this doesn't seem to work properly.

There is probably still a lot to be done for proper Overlays, but this should give everyone a good jump start.