---
title: "MachineConfig (Items)"
date: 2022-12-26T15:26:15Z
draft: false
weight: 10
---

---
In the `game/PinGodGame.tscn` scenes inspector in Godot editor you can edit the machines `Coils, Switches, Lamps, Leds`.

This can be accessed from the `MachineConfig` scene by selecting it.

![image](../../images/pingodgame-machine-items.jpg)

When the game is loaded these items will be added to the machine. 

When adding from the UI (green box) select `String` and `Int` for the value.

---

It's quicker to edit this scene file in a text editor and add your items directly in there with the name and number. The following image displays where where the coils and switches are saved to.

![image](../../images/basicgame-pingodgame-tscn.jpg)



---
### MachineConfig
---

Select the MachineConfig scene to view the exported variables

![image](../../images/machineconfig.jpg)

#### Machine Items
---

These Dictionaries populate the `Machine` items. 

Add Key/Value Pairs string / int : `"auto_plunger" - 3`

- Coils
- Switches
- Lamps
- Leds

*It is faster if you edit the scene file in a text editor and add your items there and reload scene if open in Godot*

Switches are added to the input map on load. You should add like the following when you want to assing keys to a switch. 

`Input Map`. `sw34`, `sw55`

They can be accessed from script inside the `Machine.cs` or use the PinGodGame helper methods with the item name.