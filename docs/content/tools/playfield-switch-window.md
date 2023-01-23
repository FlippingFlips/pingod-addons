---
title: "Creating custom playfield switch window"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 45
---

In Godot with your project open:

1. Filter the Files to show `playfield`, you should see `PlayfieldWindow` and `playfield_control`
2. Duplicate the `playfield_control.tscn` and name like `playfield_control_mygamename.tscn`
3. Duplicate the `PlayfieldWindow.tscn` and name like `PlayfieldWindowMyGame.tscn`
4. Now right click the duplicated files and move to `res://autoload`. Could be anywhere, but easy to put here and find.
5. Open your `PlayfieldWindowMyGame.tscn` and set the Scene under `WindowPinGod` in the inspector to your `playfield_control_mygamename.tscn`
6. In your custom `res://Machine.tscn` change the Switch Window to your new window.
7. You can load the game but the playfield is missing. Add a texture to `playfield_control_mygamename.tscn` in the `PlayfieldTextureRect`
8. The texture size should be 400x900 but you can adjust the control and window if you need larger or smaller.

## New Buttons

![image](../../images/screens/playfieldswitch-window-edit.jpg)

Open the Machine.tscn in a text editor so you can see your switches you've added to reference and copy names.

Now you can just duplicate a button and rename it to the switch name. Select a button and use `Ctrl+D` and move it into position.

Some buttons are set not to toggle so they send on/off switch. Like slingshots and spinners are never held down.

![image](../../images/screens/playfieldswitch-window-motu.jpg)

![image](../../images/screens/playfieldswitch-window-motu-added.jpg)