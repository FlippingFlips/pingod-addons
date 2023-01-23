---
title: "PinGodGamePROC"
date: 2022-12-26T15:26:15Z
draft: false
weight: 15
---

This scene overrides the normal `PinGodGame.tscn` from `autoload/PinGodGame.tscn`. 

In Godots scene inspector there are some developer options that can be set:

|Option|Default|Description|
|---|---|---|
|Proc Delay|10|Delays the proc loop, setting 10 here helps with simulation|
|Delete Db On Init|off|Deletes the database when the game is run. This is useful if you add switches to the .sql import file|
|Simulated|on|Game is simulated and will use a FakePinProc|
|LogLevel|Verbose|Log level setting for PinGodGame and NetProc.|

This will only be run when the plugin is enabled in the `ProjectSettings`, so in some cases you want to turn all plugins off and develop scenes on their own. 

I would usually leave the window actions plugin enabled though to close the window with `ESC`.

---
### Script
---

The scene uses a new script based on the `PinGodGame.cs` named `game/PinGodGameProc.cs`.

The script creates the P-ROC device and GameController. This is created as `PinGodProcGame` in the script.

It also applies window settings saved in the database, you can set your display defaults in the SQL file.

The script is `AutoLoaded` and can be retrieved from `/root/PinGodGame` the same as a normal PinGodGame.

This script waits for all resources to load from the game then will it start the `PinGodProcGameController`.
