---
title: "Adding Actions - Project Settings"
date: 2022-12-26T15:26:15Z
draft: false
weight: 12
---

### Godot Actions - Window inputs

Godot uses Actions and we can use the input map to map switches / actions to them. In the projects `Input Map` add switches to keys like the example below.

![image](../../images/godot-input-actions.jpg)

`sw20-27` are unassigned, but `sw19` is set to 1, which is the default start button.

This start button action 19 (sw19) is checked states in the `PinGodGame.cs._Input`

---

You will need to add switches you want to use on the window in the `WindowActions.tscn` scene.

---

Switch numbers convert to these actions `sw{number}` and you can assign keyboard to test game switches directly in the window.

You can create any action here and trigger it from a simulator controller. See `pause` and `quit` which is assigned to ESC but also used externally.
