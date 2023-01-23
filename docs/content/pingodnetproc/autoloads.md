---
title: "Autoloads"
date: 2022-12-26T15:26:15Z
draft: false
weight: 7
---

Autoloads are scenes that we are overriding from the `res://autoload` directory. These are plugins that can be disabled in the Godot Project Settings. Parts of your game should be able to run without these plugins but in some cases they will need each other to work fully in a game.

|Scene|Description|
|---|---|---|
|Adjustments.tscn|Not used. Our adjustments come from the database through P-ROC not PinGod|
|AudioManager.tscn|Same as existing. For your own sound assets.|
|Machine.tscn|This scene is edited. We've removed the trough and other modes PinGod uses. No need to add machine items here because done elsewhere. More info below |
|MemoryMap.tscn|Simulator: Just changed the write delay to 1000. We are not writing to memory from this node but direct in the p-roc game loop|
|PinGodGame.tscn|Has added options for P-ROC and uses new script see [PinGodGameProc](../pingodgame.tscn)|
|Resources.tscn|In the inspector we are pre loading the AttractMode and ScoreDisplay|
|WindowActions.tscn|Nothing changed|


---
## autoload/MachinePROC.cs
---

The machine script is for using with P-ROC which is based on the PinGod MachineNode.

- `SetSwitchFakeProc` - This method adds the fake switch to the switch_events queue if it's a `IFakeProcDevice`. This gets invoked from windows keys on window with WindowActions.

- Also overrides `SetSwitch` from the machine and calls `SetSwitchFakeProc` before that base method which signals to Godot switch handlers.

---
## proc/MemoryMapPROCNode.cs
---

This creates a slight different of the memory map below and frees it up on any failures.

The `WriteStates` here is used by the `PinGodProcGameController` in the `RunLoop`.

---
## proc/MemoryMapPROC.cs
---

This script overrides the existing memory map and stops writing states to memory. This is just for simulating so you can disable the plug-in if running a real board.

These states are written by the p-roc game loop here by getting the `_lastCoilStates` or `_lastLedsStates` and writing them to the mapping.

For now this seems the most consistent way when running with a simulator like Visual Pinball.
