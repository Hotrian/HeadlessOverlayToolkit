**This is the beta branch for the Headless OpenVR Overlay project.**

This is a stripped down version of the SteamVR Unity Plugin with a custom Overlay script that allows for a number of things not built into the default Overlay script. For instance, this Overlay script allows drawing multiple overlays at once, as well as placing Overlays into the world.

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

**Known Issues:**
- SteamVR_ControllerManager doesn't correctly auto-identify controllers. You must manually assign them right now :(

