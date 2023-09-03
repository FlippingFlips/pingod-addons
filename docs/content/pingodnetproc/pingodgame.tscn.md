---
title: "PinGodGamePROC"
date: 2022-12-26T15:26:15Z
draft: false
weight: 15
---

This scene overrides from the default `PinGodGame.tscn` from `autoload/PinGodGame.tscn`. 

In Godots scene inspector there are some developer debugging options that can be set:

|Option|Default|Description|
|---|---|---|
|Proc Delay|10|Delays the proc loop, setting 10 here helps with simulation|
|Delete Db On Init|off|Deletes the database when the game is run. This is useful if you add switches to the .sql import file when developing|
|Simulated|on|Game is simulated and will use a FakePinProc|
|LogLevel|Verbose|Log level setting for PinGodGame and NetProc.|

### Autoload
---

This scene will only be run when the pingodgame plugin is enabled in the `ProjectSettings`. If disabled you should see a message in the logging window like `"PinGodGameProc no ProcScene found."`

In some cases you may want to turn all plugins off and develop scenes on their own. For instance if you disable this from the autoload plugins menu you will still see the attract mode. We would usually leave the window actions plugin enabled though to be able to quickly close the window with the `ESC` key.

You can also run scenes on their own without the attract from the scene you're on by running with `F6` in godot.

---
### Script
---

The scene uses a new script based on the `PinGodGame.cs` named `game/PinGodGameProc.cs`. Some notes on what this main script does.

* The script creates the P-ROC device and GameController created as a `PinGodProcGame` in the script. 

* Applies window settings that are saved in the database (you can set your display defaults in the SQL file)

* `AutoLoaded` and can be retrieved from `/root/PinGodGame` the same as a normal PinGodGame. To access the `PROCGame` from here to access the PROC from a scene would be like so, `_pinGodProc.PinGodProcGame`

* Waits for all resources to load from the game you have set and then will it start the `PinGodProcGameController`.
