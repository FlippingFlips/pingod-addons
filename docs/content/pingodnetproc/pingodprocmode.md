---
title: "Mode - PinGodProcMode:IMode"
date: 2022-12-26T15:26:15Z
draft: false
weight: 10
---

This is a Base mode using the NetProc `P-ROC` Mode / IMode and can be found in `modes/PinGodProcMode.cs`.

These modes have access to the P-ROC game controller and can be used for your game logic and switch handling.

When a mode is created it gets access to the `Resources` pingod plugin and gets the `Modes` canvas from the `ProcScene`.

The default modes node path,  `string modesRootPath = "/root/ProcScene/Modes";`

![image](../../images/p-roc/mode_pingodprocmode.jpg)

---
## About P-ROC Modes
---

The p-roc modes have some methods to override. The ones you'll mainly be using are:

|method|Description|
|---|---|
|ModeStarted|Called when the mode is added to the Game.Modes|
|ModeStopped|Called when the mode is remove from the Game.Modes. This mode when stopped removes the CanvasLayer from the `Modes` Canvas|
|UpdateLamps|When the game calls UpdatesLamps this will be called|

---
## Canvas - Optional Visible Layer
---

This base mode has a Godot `CanvasLayer` which has an integer property of `Layer`. The layer is the order drawn or visible. The P-ROC Mode priority is set here.

Any scenes (nodes) you want to show here can be added to this CanvasLayer.

|method|Description|
|---|---|
|AddChildSceneToCanvasLayer(Node node) |Creates the CanvasLayer if null and adds the node to it.|
|RemoveChildSceneFromCanvasLayer(Node node)|Remove a node from the Canvas|
|CreateCanvasLayer(string name, int priority, bool addAsChild = true)|Creates a new Canvas layer and adds to the `ProcScene/Modes`|

See examples in the `modes/AttractMode.cs` or `modes/ScoreDisplayProcMode.cs`

---
## Clean up
---

When a mode is removed from the `IGameController.Modes` it will invoke `ModeStopped` and here we remove our CanvasLayer from the tree.

---

Note: *Use the Godot scene view when game is running and click `Remote` to see the tree sructure after modes are added.*

---

Switch handler mode example of handling switches and invoking methods like adding credits for coins:

![image](../../images/p-roc/mode_switchhandlebase.jpg)