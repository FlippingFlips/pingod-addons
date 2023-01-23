---
title: "Mode - AttractMode"
date: 2022-12-26T15:26:15Z
draft: false
weight: 25
---

In the `modes` directory you'll find `AttractMode.cs`. This is a P-ROC mode script using the base `PinGodProcMode` because we need visible layers here.

This needs a reference to a scene in `res://scenes/AttractMode/AttractProc.tscn`. 

This scene is set in the `Resources.tscn` to pre load it so we can just create instances when we need to.

![image](../../images/p-roc/attract-proc.jpg)

---
### Scene and Script
---

The script gets the top scores from the database and populates the base `Attract.cs` high score lists. Usually this is done from the `Adjustments` in a normal PinGodGame.

There is no change to the scene file, only it uses a new script to get scores from a different source.

---
### P-ROC Mode Start / Stop
---

- When the Mode is started then this scene is retrieved from the Resources and added as a child.

- When the Mode is stopped then the scene instance we created is freed and removed.

---
### P-ROC Switch Handlers
---

This mode starts the game and this is done by using the switch handler `bool sw_start_active(NetProc.Domain.Switch sw)`

You can name your handlers like this or use `sw_start_active_for_1s` or `sw_start_active_for_1000ms` for held down handlers.

When they are made like this the ScanSwitchHandlers from the NetProc looks for these methods in the mode.
