**This is the master branch for the Headless Overlay Toolkit project.**

**Be sure to check out the experimental beta branch.**

This is a stripped down version of the SteamVR Unity Plugin that includes just enough code to draw Overlays with Unity into OpenVR without throwing errors ;]

Specifically, this code creates [Overlay](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::CreateOverlay)s which can appear right inside any VR application, not [DashboardOverlay](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::CreateDashboardOverlay)s which can only appear on the dashboard menu.

The demo scene has two overlays. Only one overlay can be 'High Quality' at a time [as defined by the OpenVR API](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::SetHighQualityOverlay).
However, neither is in HQ mode by default. This can easily be changed by changing [any of these three settings](http://i.imgur.com/6SM7aab.png).

Here [is an example of the VR output](http://imgur.com/a/nU3fS) as produced by the HTC Vive, and here [is an example of the application itself](http://i.imgur.com/vKutqqA.png).
Note that it is not required to draw to the Unity display, this was done for demo purposes only. The Unity display can be used to display completely different information than is shown in the Overlay if desired, just remove or disable the MeshRenderer components from the Overlay gameobjects, and they will no longer show in Unity.

This was tested using [the null driver](https://www.reddit.com/r/SteamVR/comments/4i40k7/cant_get_steamvr_to_work_with_null_driver/d2uxgh5) and the HTC Vive, with Unity 5.3.5f1.

In the event an HMD is not detected, an "Overlay Reference Point" is spawned to anchor the overlays somewhere in relation to the camera, however these Overlays will be stuck to the screen.

If you want this attached to the controllers, I suspect you'll want to look into [SetOverlayTransformTrackedDeviceRelative](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::SetOverlayTransformTrackedDeviceRelative) or SteamVR_TrackedObject.cs and check out [this section of SteamVR_Overlay.cs](https://github.com/Hotrian/ViveOverlay/blob/master/Assets/SteamVR/Scripts/SteamVR_Overlay.cs#L110-L136) and adjust accordingly.

There is probably still a lot to be done for proper Overlays, but this should give everyone a good jump start.

**If you want to run this headless:**

Check out the [documentation here](http://docs.unity3d.com/Manual/CommandLineArguments.html) on how to run Unity headless.  There are a few different ways to do this.

The basic steps to create a shortcut on Windows that launches headless are:
- Build your Application
- Create a Shortcut to your Application
- Right Click the Shortcut > Properties
- Put " -batchmode" at the end of the text in the 'Target' box
- Launch your Shortcut, and your Application should launch hidden
- You can crash your Application through the Task Manager, but be sure to add a graceful way to quit in the future :)