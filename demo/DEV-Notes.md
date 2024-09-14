# pingod-addons Game Demo

## How this game demo was created by using the addons...
**Note: duplicating and moving scenes is best done in Godot
because it will update any references using the paths inside scene files**
### MainScene
1. Duplicate the addons `res://pingod-game/Node/MainScene.tscn` to the `demo` folder inside Godot.
2. Right click and `Set as main scene` to load this scene when the game is launched
3. Inside here is a `Modes` control layer for putting modes in from scripts.
Modes in this game would be `Game` or `Attract`. **actual pinball game modes go inside the game scene Modes.**
4. The inspector properties show the paths for some other scenes. `Attract, Game` and `Service`.
These all point to default scenes provided in the `addons`.
We can duplicate the game scene and change the path in this scene. This will load our scene as an override.
### GameScene
1. Duplicate the `Game.tscn` inside Godot into the `demo` folder.
You can call it what you want, you can point to this scene from the `MainScene`
2. Change the `Game Scene Path` in the `MainScene`'s inspector.
For example `res://demo/Game.tscn`.
Quick load file picker can be used or manual entry in the scene or direct in the file.
3. Save the main scene and open the new `demo/Game.tscn`.
4. In this scene there is a `Modes` and `Modes-Upper`.
On the game inspector there is a collection of scenes which are preloaded and can be used when needed.
These scenes get added to the modes canvaslayer.
### Custom GameScene Script
1. In the Inspector find the script and click to create new script. `C#` `DemoGame.cs`
2. After the script is created make it inherit from `Game` which is an `IGame`
3. In the inspector the script should be set to `DemoGame`
This script can be used for game related stuff and to override the `Game`'s methods when needed.
### Adding a mode
#### Mode Scene
1. Create directory in Godot named `modes`. `res://demo/modes`
2. Add a new scene into this directory named `BaseMode.tscn`. Use the type `Control`.
3. Use the anchor presets to make the `Control` fit the size of pane
4. Add this mode to a group named `Mode`. Next to tje `Inspector` tab is `Node > Groups`.
Select this and add and assign a group named `Mode`.
#### Mode Script
1. Create new `C#` script for the mode in the inspector under the same name `BaseMode.cs`.
The script should inherit from `Control` or you can use `PinGodGameControl` with game and resource access
2. Run the game and use the playfield control button to fill the trough, start the game.
Click back to Godot, and in the scene view click `Remote`.
If you drill into the `MainScene` you should see the `BaseMode` in the tree under `Game/Modes`.
**You can adjust controls live in the scene while running the game**
### Adding BallSave scene to the mode
1. add an Export property to a field in the `BaseMode.cs`.
`[Export(PropertyHint.File)] string BALL_SAVE_SCENE;`
After building the scene you will see this property we exported in the Godot inspector.
2. Select the `BallSave.tscn` which can be found under the addons.
`res://addons/pingod-modes/ballsave/BallSave.tscn`
3. Duplicate the `Resources.tscn` into the `autoload` folder,
this will override the addons version of the resources.
Open it and add the ball save to pre load.
4. Disable / Enable the `Resources` plug-in in Godot's Project Settings to make it set the new `autoload/Resources.tscn` scene.
5. See the script `BaseMode.cs` on how to active the scene and remove it automatically

**See the `res://demo` folder and `res://autoload` for more. You could duplicate this whole folder in Godot and make the game point to the main scene**