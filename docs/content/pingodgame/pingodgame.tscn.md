---
title: "PinGodGame.tscn - Scene"
date: 2022-12-26T15:26:15Z
draft: false
weight: 5
---

[view full image](../../images/godot-pingodgame-tscn.jpg)

![image](../../images/godot-pingodgame-tscn.jpg)

This scene is autoloaded with the game with all of its child scenes.

Selecting the scene will display the exports options in the scene inspector, see the highlighted right panel in the image.

These godot script exports are created in the `PinGodGame.cs` script as `[Export]` properties and the values set here will be saved into the parent scene file, in this case `PinGodGame.tscn`

Here we have enabled recording and playback file is set, saved to the scene.

![image](../../images/basicgame-pingodgame-tscn-exports.jpg)

The exports here are for developer options.

---
## Script Exports
---

|Export|Description|
|-|-|
|Lamp Overlay Enabled|Enables a lamp dev overlay [Link](../devoverlays)
|Switch Overlay Enabled|Enables a switch dev overlay [Link](../devoverlays)
|Record Game|Record game events for playback
|Playback Game|Enables Playback for a saved .record file
|Playbackfile|File to playback if enabled. res://recordings or user://recordings/recordname.record

---
## Scene Tree
---

Top left pane scenes in `PinGodGame.tscn`

|Link|Scene|Description|
|-|-|-|
|[Details](../items)|MachineConfig|Manage machine items, ball search
|[Details](../../getting-started/basicgame-game-resources)|Resources|When this is loaded with the scene it looks for the packs you have set in the Resource Packs.
|[Details](../trough)|Trough|Manage machine items, ball search
|[Details](../audiomanager)|AudioManager|Scene the game uses to manage audio, media controller


#### res:// paths for assets

Quicker to add asset paths here

![image](../../images/audiomanager_options_code.jpg)


