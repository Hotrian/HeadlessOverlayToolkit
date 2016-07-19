**This is the master branch for the Headless Overlay Toolkit project.**

This is a stripped down version of the SteamVR Unity Plugin with a custom Overlay script that allows for a number of things not built into the default Overlay script. For instance, this Overlay script allows drawing multiple Overlays at once, as well as placing Overlays into the world.

**Demos:**

Note that these demos were taken during development, and do not necessarily represent the current state of the branch.
- [Here is a Youtube Video](https://www.youtube.com/watch?v=q1PTaL1Sx9I) that shows some of the default Controller attachment points.
- [Here is a Youtube Video](https://www.youtube.com/watch?v=nB19zl-_DlM) that gives an example of these Overlays in TiltBrush.
- [Here is a GIF](https://gfycat.com/SoftJointFrigatebird) that very quickly demonstrates an AlphaAndScale animation on Gaze.

**Features:**
- Draw Overlays, regardless of the current VR application.
- Easily attach Overlays to the Screen, a Controller, or drop one in the World.
- Easily snap Controller attached Overlays to a set "Base Position".
- Offset Overlays positionally and rotationally.
- Draw Multiple Overlays Simultaneously (only one Overlay can be 'High Quality').
- Custom Inspector with Undo support.
- Basic Gaze Detection and Animation support (Fade In/Out or Scale Up/Down on Gaze).

**Known Issues:**
- SteamVR_ControllerManager.cs doesn't correctly auto-identify controllers for me, so I wrote my own manager, HOTK_TrackedDeviceManager.cs. My Device Manager is super pre-alpha but should correctly identify both Controllers as long as at least one of them is assigned to either the left or right hand, and they are both connected. If neither Controller is assigned to a hand, they are assigned on a first come first serve basis. If only one Controller is connected, and it isn't already assigned, it will be assigned to the right hand.

**Additional Notes:**
- When attaching Overlays to controllers, the offset is reoriented to match the Base Position's orientation. X+ should always move the Overlay to the Right, Y+ should always move Up, and Z+ should always move Forward, relative to the Overlay.
- The Custom Inspector has custom collapse elements. You can change the default "collapse status" by messing with the defaults for ShowSettingsAppearance, ShowSettingsInput, and ShowSettingsAttachment at the top of HOTK_Overlay.cs.
- Only one Overlay can be 'High Quality' at a time. An Overlay must be 'High Quality' to display Curved or with Anti-Aliasing as per the [OpenVR API](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::SetHighQualityOverlay). 'High Quality' Overlays skip the Compositor and are drawn directly to the display. If you enable multiple HQ Overlays, any additional ones will have HQ toggled off and you'll receive a warning.

**If you want to run this headless:**

Check out the [documentation here](http://docs.unity3d.com/Manual/CommandLineArguments.html) on how to run Unity headless.  There are a few different ways to do this.

The basic steps to create a shortcut on Windows that launches headless are:
- Build your Application
- Create a Shortcut to your Application
- Right Click the Shortcut > Properties
- Put " -batchmode" at the end of the text in the 'Target' box
- Launch your Shortcut, and your Application should launch hidden
- You can crash your Application through the Task Manager, but be sure to add a graceful way to quit in the future :)