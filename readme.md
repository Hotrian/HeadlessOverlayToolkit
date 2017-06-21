**This is the redux branch for the Headless Overlay Toolkit project.**

## About this branch

HOTK has been out of development for a bit and it's time to get it going again!

HOTK has a number of issues that occured due to me being new to SteamVR's APIs, and as such I am starting fresh with a clean copy of the SteamVR APIs and Unity 5.6.0f3. The new version of HOTK will be able to run as an Overlay Engine in new and existing applications which render a scene, as well as applications which do not render a scene.

Previously I did not push past Unity 5.3 due to NativeVR support causing issues with HOTK, however, I have decided to push forwards and get HOTK working properly :).

## Where are things at?

**As of the last time this readme was updated, this branch is NOT READY FOR USAGE!**

It is open for auditing and suggestions, however it is not ready to be used in ANY sort of application at this time.

## Planned Features

- Draw Overlays, regardless of the current VR application.
- Easily attach Overlays to the Screen, a Controller, or drop one in the World.
- Easily snap Controller attached Overlays to a set "Base Position".
- Offset Overlays positionally and rotationally.
- Draw Multiple Overlays Simultaneously (only one Overlay can be 'High Quality').
- Support for events such as `OnDeviceAim` and `OnDeviceTouch`.
- Integration with the SteamVR Laser Pointer (I couldn't get this working before, but I intend to find out why and get it working properly now).
- "Grab" support for World _and Controller Attached_ Overlays.
- Support for ALL Devices (as opposed to 4+HMD as the last version of HOTK supported).
- And more!

## How is this different from the last version of HOTK?

The old version of HOTK had a bunch of subtle issues, like how it treated SteamVR's TrackingSpaces and how it handled translating Transforms into VR space.

I'm staring HOTK over again so it can be built on a properly foundation and understanding on SteamVR, instead of constantly having to work around all the patched code and improper implementations.

In all honesty, one issue is that the SteamVR Unity Examples are not written properly in many cases :(. For example, the SteamVR Overlay Example code is specifically setup to only allow a single Overlay in your project, despite the SteamVR APIs allowing a very large number (64?) of Overlays simultaneously.

## Building

#### Prerequisites 
- Unity v5.6.0f3 (Other versions may work as well, but likely need changes)
- SteamVR Unity Plugin v1.2.1


