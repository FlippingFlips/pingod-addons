# pingod-windows
This plug-in handles can handle godot actions (key presses) to machine switches.
A tool panel window can be enabled from this plug-in where you can add buttons assigned to open windows when developing.

---
### AutoLoad
This plug-in loads on startup as a singleton to handle input events from the window.

| Key | Does? | Action Name |
| --- | --- | --- | 
| ESC or F8  | Quit window, game | ui_cancel |
| F2  | Toggle window border (for resize, move) | toggle_border |
| 5  | Add coin | sw0 |
| 1  | Start Game | sw8 |

Open the `WindowsActions.tscn` scene in Godot and view the `Inspector` options to disable the tool window and add game switches

## About other windows

[Tools Panel Readme](tools_panel/README.md)

[Playfield Control Readme](playfield/README.md)
