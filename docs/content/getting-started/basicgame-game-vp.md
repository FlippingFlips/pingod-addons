---
title: "BasicGame - Visual Pinball"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 30
---

Now that the game is building and running it can be launched with Visual Pinball.

- Table can be run from any location in Visual Pinball. 

- The controller will load the game when the vp player is launched.

{{% panel status="primary" title="Tip" icon="far fa-lightbulb" %}}
Use the `_vp_edit.bat` or `_vp_play.bat` to open the table easier
{{% /panel %}}

Provided you have the controller setup and have built the godot project this will play.

![image](../../images/basicgame-vploaded.jpg)

{{% panel status="primary" title="Note" icon="far fa-lightbulb" %}}
If the trough isn't working, then you touched the window :). You can launch the game make adjustments then quit to VP editor and play
{{% /panel %}}


---
### COM Controller setup
---

We need to register the COM object before it can be used by other applications such as a simulator.

---
#### Register controller
---

1. Download the latest at [pingod-controller-com/releases](https://github.com/FlippingFlips/pingod-controller-com/releases)
2. Extract the download anywhere on your system (if following this guide it may pay to create a new folder `pingod-controller-vp` inside `pingod`)
3. Run the `PinGod.VP.Setup.exe` to unregister / register the COM object
4. If you move these files then you would have to register it again.

---
#### Simulator Scripts
---

1. Copy the scripts directory to your visual pinball directory. This would be something like `C:\Visual Pinball\Scripts`

|Script|What For?|
|---|---|
|core_c_sharp.vbs|a copy of core.vbs but with slightly different ways to invoke methods in C#|
|PinGod.vbs|a machine definition file|

---
---
---

## Debugging
---

When debugging you can load the game direct from the `BasicGameGodot` directory as you would in the previous section only with the controller.

This table script is set to `Debug = True` and to look for `..\BasicGameGodot`, which is a level up in this repository.

These settings will run the game debug with the Godot editor, show display and also a console window.
