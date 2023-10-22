# pingod-windows

This module handles godot actions (key presses) to machine switches. A playfield switch window also enabled from this plugin.

---
### AutoLoad
This loads as a singleton to handle input events from the window.

| Key | Does? | Action Name |
| --- | --- | --- | 
| ESC  | Quit game | ui_cancel |
| F2  | Toggle window border (for resize, move) | toggle_border |
| 5  | Add coin | sw0 |
| 1  | Start Game | sw8 |

Open the `WindowsActions.tscn` in Godot and view Inspector options to disable handling of switches if you just want the switch window.

---
### Add new switch

1. Add a switch named like `sw18` to godot's `Project Settings > Input Map`
2. Assign a key to the action
3. Make sure the switch is available in the `GameSwitches` list in the `WindowsActions.tscn` scene

---
### Switch window

Test switches overlaid onto a playfield in another window. Customize the `playfield` files for your own switches.
