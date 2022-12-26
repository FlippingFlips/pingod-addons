---
title: "BasicGame - Visual Pinball"
date: 2017-10-17T15:26:15Z
lastmod: 2019-10-26T15:26:15Z
draft: false
weight: 30
---

Now that the game is building and running it can be launched with Visual Pinball.

- Table can be run from any location in Visual Pinball. 

- The controller will load the game when the vp player is launched.

{{% panel status="primary" title="Tip" icon="far fa-lightbulb" %}}
Use the `_vp_edit.bat` or `_vp_play.bat` to open the table easier
{{% /panel %}}

![image](../../images/basicgame-vploaded.jpg)

Provided you have the controller setup and have built the godot project this will play.

### Debugging
---

When debugging you can load the game direct from the `BasicGameGodot` directory as you would in the previous section but with the controller.

This table script is set to `Debug = True` and to look for `..\BasicGameGodot`, which is a level up in this repository.

These settings will run the game debug with the Godot editor, show display and also a console window.
