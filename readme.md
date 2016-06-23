**This is the twitchtest branch for the Headless Overlay Toolkit project.**

This is a stripped down version of the SteamVR Unity Plugin with a custom Overlay script that displays twitch chat!

To use this, load up the HOTK_TwitchDemoScene, press Play, and enter your Username, OAuth Key (get your [OAuth Key here](http://www.twitchapps.com/tmi/)) and the desired Channel, then press "Press to Connect" and it should momentarilly connect to your Twitch chat. Check behind one of your controllers (should be left!) for your chat display! It should zoom up when you look at it.

**Basic Controls:**
![basic controls](http://image.prntscr.com/image/d4db6168f600458989f0307afbf1fd99.png)

**Demos:**

Note that these demos were taken during development, and do not necessarily represent the current state of the branch.
- ![Here is a GIF](https://thumbs.gfycat.com/SinfulHonestGenet-size_restricted.gif)
- Please see this [higher quality view of the GIF above](https://gfycat.com/SinfulHonestGenet) of the Twitch Chat in action!
- [Check out this Youtube Video](https://www.youtube.com/watch?v=JMk7Vy1Zq_s) which shows the desktop UI in action!
- [**Here it is inside TiltBrush!**](https://www.youtube.com/watch?v=tpqIQ5UkGrY) The text is perfectly readable in headset, but seems a little far away on the Display Mirror :P

**Features:**
- See Twitch Chat in VR! From Any Game!
- Easily attach Chat to the Screen, a Controller, or drop it in the World.
- Easily snap Controller attached Chat to a set "Base Position".
- Offset Chat positionally and rotationally. (build currently missing rotation control, but works with Unity Editor)
- Custom Inspector with Undo support.
- Basic Gaze Detection and Animation support (Fade In/Out or Scale Up/Down on Gaze).
- Extremely Basic Save/Load Support! Saves Username/Channel but does not save OAuth Key!

**Known Issues:**
- There is no feedback when you type in an incorrect Username, OAuth, or Channel. If you press connect and do not receive messages, double check your spelling!
- SteamVR_ControllerManager.cs doesn't correctly auto-identify controllers for me, so I wrote my own manager, HOTK_TrackedDeviceManager.cs. My Device Manager is super pre-alpha but should correctly identify both Controllers as long as at least one of them is assigned to either the left or right hand, and they are both connected. If neither Controller is assigned to a hand, they are assigned on a first come first serve basis. If only one Controller is connected, and it isn't already assigned, it will be assigned to the right hand.
- If you launch it and nothing happens in VR, check the Data folder for output_log.txt and look for "Connect to VR Server Failed (301)" If you are getting this, try relaunching as Admin and if that doesn't work try launching as Admin in Compatability mode, some users have found that it only works this way, but I am not sure why yet.
- The build is currently missing the ability to Rotate the Overlays. This will be added in a future version, but if you need to rotate them now, download the source and run it with Unity and rotate the Overlay Gameobject.

**Additional Notes:**
- When attaching Overlays to controllers, the offset is reoriented to match the Base Position's orientation. X+ should always move the Overlay to the Right, Y+ should always move Up, and Z+ should always move Forward, relative to the Overlay.
- The Custom Inspector has custom collapse elements. You can change the default "collapse status" by messing with the defaults for ShowSettingsAppearance, ShowSettingsInput, and ShowSettingsAttachment at the top of HOTK_Overlay.cs.
- Only one Overlay can be 'High Quality' at a time. An Overlay must be 'High Quality' to display Curved or with Anti-Aliasing as per the [OpenVR API](https://github.com/ValveSoftware/openvr/wiki/IVROverlay::SetHighQualityOverlay). 'High Quality' Overlays skip the Compositor and are drawn directly to the display. If you enable multiple HQ Overlays, any additional ones will have HQ toggled off and you'll receive a warning.

**Quick Start** for Unity Editor:
- Download and Install [Unity](https://unity3d.com/get-unity/download?ref=personal) (I'm using version 5.3.5f1)
- Download [the repo as a zip](https://github.com/Hotrian/HeadlessOverlayToolkit/archive/twitchtest.zip) and unzip the folder inside.
- Launch Unity
- [Click 'Open'](http://image.prntscr.com/image/49b79aeea81e4ad48d1b56df76cef5fb.png), select the folder you unzipped, [click 'Select Folder'](http://image.prntscr.com/image/8c851924eacf4d44944498b26dad74ed.png)
- Once Unity Loads the project, [Load up the TwitchChat Demo scene](http://image.prntscr.com/image/1bc9e550f875468bb49558b76cc2d1f9.png)
- [Click Play](http://image.prntscr.com/image/4eba127a9d59427fa1c5b6a06aac8eed.png)
- Once the project starts, enter your Username, OAuth key (this is basically a password, you can [get yours here](https://twitchapps.com/tmi/)), and desired Channel name [into these boxes](http://image.prntscr.com/image/9ba697c9537d431e8f1b58813cb677a0.png) and click "Press to Connect"
- With any luck, your twitch chat should appear behind one of your controllers :D

Note that you can copy and paste your oauth key, you don't have to manually type it out, but you do have to get it off that site, or off another site that generates Twitch OAuth keys.

**If you want to run this headless:**

Check out the [documentation here](http://docs.unity3d.com/Manual/CommandLineArguments.html) on how to run Unity headless.  There are a few different ways to do this.

The basic steps to create a shortcut on Windows that launches headless are:
- Build your Application
- Create a Shortcut to your Application
- Right Click the Shortcut > Properties
- Put " -batchmode" at the end of the text in the 'Target' box
- Launch your Shortcut, and your Application should launch hidden
- You can crash your Application through the Task Manager, but be sure to add a graceful way to quit in the future :)

To run the twitchtest variant headless, you'll have to modify the scripts so that they automatically input the correct Username, OAuth, and Channel name, before starting automatically.

**Special Thanks:**

Thanks to Grahnz for the [TwitchIRC.cs](https://github.com/Grahnz/TwitchIRC-Unity/blob/master/TwitchIRC.cs) script!