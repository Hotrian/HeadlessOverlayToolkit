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

**If you want to run this headless (Windows OS Only):**

It seems like `-batchmode` no longer does what it is supposed to do, and that you cannot run HOTK using `-batchmode`.

Instead, I have written a script that causes the Standalone Player Window to move off screen, and hide itself from the Taskbar. This is very similar to running headless, except the Splash Screen will still display if you are using Unity Personal Edition and cannot disable the Splash.

To run this in sort-of-headless mode:

- Attach [this script](https://gist.github.com/Hotrian/1b0512e9feed42085a11446d17c26a12) to a single GameObject.
- (optional) Disable/Delete the "Main Camera".
- (If Main Camera disabled) The Game View in the Unity Editor should now say `Scene is missing a fullscreen camera`.

Now when you launch your application, you should see the Splash Screen (if on Unity Personal or not disabled), then the application should appear to close, as the Window should disappear and it will no longer appear on the Taskbar or Alt+Tab. However, if you check Task Manager you should see the application is in fact running in the background, and if you check SteamVR you should see your Overlay(s) as before.

By Disabling/Deleting the Main Camera, you can further reduce resource usage. When there is no Main Camera, Unity will no longer render anything to the Desktop Window, which is fine if you are trying to run Headless. Your Overlays will still be sent to SteamVR and rendered as before.
