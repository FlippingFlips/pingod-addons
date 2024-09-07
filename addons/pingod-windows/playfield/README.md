## Playfield window / switch controller
Test switches overlaid onto a playfield in another window. Customize the `playfield` files for your own switches.

TODO: read from a collection of switches or in this case read the machine switches.

TODO: NetPinProc has X and Y positions for machine items, these need to be imported and buttons created at runtime.
	- Should have options for rotation, shape of the switch
	- is the switch toggled? or a pulse switch?


## OLD
1. Add a switch named like `sw18` to godot's `Project Settings > Input Map`
2. Assign a key to the action
3. Make sure the switch is available in the `GameSwitches` list in the `WindowsActions.tscn` scene

---