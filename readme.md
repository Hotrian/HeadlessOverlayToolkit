**This is the beta branch for the Headless OpenVR Overlay project.**

This is a stripped down version of the SteamVR Unity Plugin with a custom Overlay script that allows for a number of things not built into the default Overlay script. For instance, this Overlay script allows drawing multiple Overlays at once, as well as placing Overlays into the world.

**Demos:**

Note that these demos were taken during development, and do not necessarily represent the current state of the branch.
- [Here is a Youtube Video](https://www.youtube.com/watch?v=q1PTaL1Sx9I) that shows some of the default Controller attachment points.
- [Here is a Youtube Video](https://www.youtube.com/watch?v=nB19zl-_DlM) that gives an example of these Overlays in TiltBrush.

**Features:**
- Draw Overlays, regardless of the current VR application.
- Easily attach Overlays to the Screen, a Controller, or drop one in the World.
- Easily snap Controller attached Overlays to a set "Base Position".
- Offset Overlays positionally and rotationally.
- Draw Multiple Overlays Simultaneously (only one Overlay can be 'High Quality').
- Custom Inspector with Undo support.

**Known Issues:**
- SteamVR_ControllerManager.cs doesn't correctly auto-identify controllers. You must manually assign them right now :(

**Additional Notes:**
- When attaching Overlays to controllers, the offset is reoriented to match the Base Position's orientation. X+ should always move the Overlay to the Right, Y+ should always move Up, and Z+ should always move Forward, relative to the Overlay.
- The Custom Inspector has custom collapse elements. You can change the default "collapse status" by messing with the defaults for ShowSettingsAppearance, ShowSettingsInput, and ShowSettingsAttachment at the top of OpenVrOverlay.cs.
- Only one Overlay can be 'High Quality' at a time. An Overlay must be 'High Quality' to display Curved or with Anti-Aliasing as per the [OpenVR API](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::SetHighQualityOverlay). 'High Quality' Overlays skip the Compositor and are drawn directly to the display. If you enable multiple HQ Overlays, any additional ones will have HQ toggled off and you'll receive a warning.