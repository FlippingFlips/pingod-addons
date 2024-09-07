# Tools panel
This is to display a button tool-bar which can show/hide extra windows for the developer
on top of the game.

The script doesn't load an extra scene like the playfield window examplem the buttons are direct inside the `HBoxContainer`.

## How to add Button + Window
#### Create Window and Script
- Create a folder under the window name (doesn't have to be perfect)
- A scene can be loaded into the scene which will get loaded at runtime.(see playfield)
- `Name` the button. When the button is clicked it sends the `Name` and the toggled window state.
This signal is picked up by the `WindowActions`

#### Update WindowActions Scene
- Open the WindowActions scene in Godot, or your override WindowActions scene from the `Autoload` in Godot
- Under the `Dev Windows` section in the inspector you can add a `Key` and a `PackedScene`
- See example playfield window: This has a key named `Switches` and the button from the Tools Pane `Switches` triggers this to open.
- Add Key as a string and load a packed scene from the quick load menu when selecting the object
