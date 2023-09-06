# PinGod - P3-ROC - Service mode Menus

* Directories aside from `Shared` should reflect views as navigated to in UI.

Main Menu > Tests > Switches

Main Menu > Tests > Coils

## PROC Mode
---

This P_ROC mode `res://modes/ServiceMode.cs` is the base for the service mode, this handles events from the boards.

When `ModeStarted` is called then a `ServiceModePinGod` scene will be added to the ModesCanvasLayer. This is the base service mode scene and handles events sent from the `ServiceMode.cs`.

The `ServiceMode.cs` invokes the `OnSwitchPressed` in the `ServiceModePinGod` which shifts onto the UI.

Any scenes that are in the group `switch_views` and have a method name `OnSwitch` will be called after this. `CallGroup("switch_views", "OnSwitch", swName, swNum, isClosed)`

See the `Tests/Switches/MatrixSwitchesGridContainer.cs` for example use where it's updating the UI color of switch states.