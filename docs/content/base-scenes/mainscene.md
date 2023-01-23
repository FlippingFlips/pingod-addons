---
title: "Main Scene"
date: 2022-12-26T15:26:15Z
draft: false
weight: 2
---

This scene is the first to be loaded as a base display set in the projects settings and the scene is never removed from the game, but can add / remove modes like removing the Attract then adding a Game scene.

The first run scene can also be found in the `project.godot` file. `res://MainScene.tscn"`

---
### Scene inspector - Exports
---

|Property|Description|
|---|---|
|Game Scene Path|The path to the Game scene the main scene will use when started from attract|
|Service Menu Scene Path|The path to the service menu (default)|


### Modes
---

When resources are fully loaded an attract mode is added to the `Modes`, when a game is started a game is added here in place of the attract.

### Pause & Settings Display
---

In game menu to adjust audio, display, game and pingod settings.

Default scene `res://addons/modes/pause-settings/PauseSetting.tscn`


![image](../../images/mainscene.jpg)

---
## Script
---

Script Location: `res://addons/pingod-core/Game/MainScene.cs`.

This script controls what goes into the Modes control. It has control over game start and resets.

---
### Godot Overrides
---
### _EnterTree
---
- Gets the Machine plugin and hooks up to the `SwitchCommand` handler to listen for service menu `enter` switch

---
### _Ready
---

- Gets Resources plugin
- Gets PinGodGame plugin
- Hooks to signals listed below

### _Input - Actions
---

- reset 	= resets the game F3

#### PinGodGame - Signals connected

- PinGodGame.GameStarted		= Calls StartGame which loads the Game scene
- PinGodGame.GameEnded			= Removes the `Game` from Modes tree.
- PinGodGame.ServiceMenuExit	= Reloads the scene. `ReloadCurrentScene`
