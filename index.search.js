var relearn_search_index=[{content:`Start with duplicating the ScoreMode.tscn file from the addons.
Filter the files in godot for score and duplicate it, rename it to MygameScoreMode or anything like that. You want to move the file after it’s created to somewhere in your root like res://scenes.
The following image is the steps above and loaded.
If you select the ScoreMode node and look in the inspector these are the options from that script.
Focusing on the Score Labels this is where you can select the scores for the players. If you had a 10 player game you could add some more labels and select them here.
Customizing It’s up to you what you do here but you are not limited to anything but imagination.
I have made several different score displays all using this as a base which are all a bit different.
You can extend the script, so this script can cover most scoring, so all you worry about here is making it nice with Godot and the game will handle the scores.
Extend ScoreMode.cs If you click on the script dropdown in the ScoreMode inspector window, as 2nd image shows then you can extend from here.
Click extend then C#, the name should be the same as your scene but with the .cs extension.
Godot will asign this script after extending but the base class is incorrect. Change this from Node to ScoreMode.
using Godot; using PinGod.Modes; using System; public partial class MyTemplateScoreMode : ScoreMode { }Now you can do what you like here too if you need something extra in the mode.
An example would be overriding the OnScoresUpdated and updating other things like other player stats in the game that you have in your scene.
`,description:"",tags:null,title:"Creating custom score mode scene",uri:"/pingod-addons/tips-guides/custom-score-mode/index.html"},{content:`Image edit Tools Here are some tools we use a lot and have been used when making games for pinball.
A quick way to install anything is through chocolatey.
Inkscape (scalable vector) choco install inkscape
🔗 Inkscape (chocolatey)
ImageMagick (converting webp) choco install imagemagick.app
🔗 ImageMagick (chocolatey)
Gimp (Image editing) choco install gimp
🔗 Gimp (chocolatey)
`,description:"",tags:null,title:"Image Tools",uri:"/pingod-addons/game-assets/tools/image-tools/index.html"},{content:"",description:"",tags:null,title:"Tools",uri:"/pingod-addons/game-assets/tools/index.html"},{content:`This scene is the first to be loaded as a base display set in the projects settings and the scene is never removed from the game, but can add / remove modes like removing the Attract then adding a Game scene.
The first run scene can also be found in the project.godot file. res://MainScene.tscn"
Scene inspector - Exports Property Description Game Scene Path The path to the Game scene the main scene will use when started from attract Service Menu Scene Path The path to the service menu (default) Modes When resources are fully loaded an attract mode is added to the Modes, when a game is started a game is added here in place of the attract.
Pause & Settings Display In game menu to adjust audio, display, game and pingod settings.
Default scene res://addons/modes/pause-settings/PauseSetting.tscn
Script Script Location: res://addons/pingod-core/Game/MainScene.cs.
This script controls what goes into the Modes control. It has control over game start and resets.
Godot Overrides _EnterTree Gets the Machine plugin and hooks up to the SwitchCommand handler to listen for service menu enter switch _Ready Gets Resources plugin Gets PinGodGame plugin Hooks to signals listed below _Input - Actions reset = resets the game F3 PinGodGame - Signals connected PinGodGame.GameStarted	= Calls StartGame which loads the Game scene PinGodGame.GameEnded	= Removes the Game from Modes tree. PinGodGame.ServiceMenuExit	= Reloads the scene. ReloadCurrentScene `,description:"",tags:null,title:"Main Scene",uri:"/pingod-addons/base-scenes/mainscene/index.html"},{content:`Attract Class Reference Class Reference
`,description:"",tags:null,title:"Attract Scene",uri:"/pingod-addons/base-scenes/attractscene/index.html"},{content:`This scene is loaded from the attract mode. (Add to favorites for quick access)
Add scenes to the modes CanvasLayer to use in game or add to from script
Default Modes (CanvasLayer) ScoreMode [[ScoreMode.tscn ScoreMode.cs|06a.-ScoreMode]]
BaseMode Set the Ball Save Scene [[BaseMode.tscn BaseMode.cs|06b.-BaseMode]]
Bonus Set the default text and display time [[Bonus.tscn Bonus.cs|06c.-Bonus]]
Tilt Set number of tilt warnings [[Tilt.tscn Tilt.cs|06d.-Tilt]]
ScoreEntry Scene to show when player is entering a score [[ScoreEntry.tscn ScoreEntry.cs|06e.-Tilt]]
Base Game scene for Game.tscn. BasicGame.cs inherits this class and is used in the game/Game.tscn scene.
You can add some of your own overrides here to existing methods at common stages of a pinball game. See Methods to override below.
This is a good scene to invoke methods on groups. Used in res://addons/PinGodGame/Game/Game.cs
Exports MULTIBALL_SCENE = Default scene file to use for Multi-Ball: res://addons/PinGodGame/Scenes/Multiball.tscn
_service_menu_scene_path = The Service Menu Scene to load. Default: res://addons/PinGodGame/Scenes/ServiceMenu.tscn
Godot - Overrides _EnterTree Creates a tilted timer which starts a new ball on timedout
Nodes (Scenes) referenced Modes/ScoreEntry Modes/Bonus Signals connected PinGodGame.BallDrained	= PinGodGame.OnBallDrained(SceneTree, string, string) on the whole tree so any modes in group named Mode with OnBallDrained will be called. PinGodGame.BallEnded	= Ends multiball. Starts Bonus if not tilted. Displays high score if last ball PinGodGame.BallSaved	= PinGodGame.OnBallSaved on modes group as node in tree. Mode group. PinGodGame.BonusEnded	= Displays high score entry scoreEntry.DisplayHighScore if last ball or StartNewBall PinGodGame.MultiBallEnded	= EndMultiball on all scenes in the multiball group PinGodGame.ScoreEntryEnded	= PinGodGame.EndOfGame, which will send a game ended _Ready Sets the games BallInPlay = 1 and starts a new ball
Methods to Override AddMultiballSceneToTree	= Gets an instance of the multi-ball scene and add it to the Modes tree in group multiball AddPoints	= add points (pinGod.AddPoints) with 25 bonus EndMultiball	= Any node that is in the multiball group is removed from tree OnBallEnded	= Add a display at end of ball OnBonusEnded	= Displays high score if this is the last ball. OnScoreEntryEnded	= pingod.EndOfGame StartNewBall	= Starts new ball with PinGodGame and invokes the OnBallStarted method on all in Mode group OnBallDrained	= PinGod.OnBallDrained invokes the OnBallDrained method on all in Mode group OnBallSaved	= Calls OnBallSaved for the whole tree in Mode group `,description:"",tags:null,title:"Game Scene",uri:"/pingod-addons/base-scenes/gamescene/index.html"},{content:`Create Playfield image to use: Open the game in VP and take a screenshot of the editor with the playfield in view. You can do this with Alt + PrtScn. Open Microsoft Paint and paste in you screenshot of the editor. Get there with windows key and typing paint Select the outer playfield and crop it in MsPaint Click resize by pixels, uncheck Maintain aspect ratio and set to 400 x 900 Save this image into your game folder so Godot picks it up and you can assign this to window image. res://assets/img is a good place to put it. Create custom Window Filter the Files to show playfield, you should see PlayfieldWindow and playfield_control Duplicate the playfield_control.tscn and name like playfield_control_mygamename.tscn Duplicate the PlayfieldWindow.tscn and name like PlayfieldWindowMyGame.tscn Now right click the duplicated files and move to res://autoload. Could be anywhere, but easy to put here and find. Open your PlayfieldWindowMyGame.tscn and set the Scene under WindowPinGod in the inspector to your playfield_control_mygamename.tscn In your custom res://Machine.tscn change the Switch Window to your new window. You can load the game but the playfield is missing. Add a texture to playfield_control_mygamename.tscn in the PlayfieldTextureRect The texture size should be 400x900 but you can adjust the control and window if you need larger or smaller. New Buttons Open the Machine.tscn in a text editor so you can see your switches you’ve added to reference and copy names.
Now you can just duplicate a button and rename it to the switch name. Select a button and use Ctrl+D and move it into position.
Some buttons are set not to toggle so they send on/off switch. Like slingshots and spinners are never held down.
`,description:"",tags:null,title:"Creating custom playfield switch window",uri:"/pingod-addons/tips-guides/playfield-switch-window/index.html"},{content:` Filter Godot files by ScoreEntry Duplicate the ScoreEntry.tscn and move to your res://autoload directory Add the packed scene location, replacing any existing in the res:autoload/Resources.tscn Add the new scene into the Game.tscn so it knows where your version is Edit away without effecting the addons and script. You can take it further than cosmetic from here with a script. Things you can try in the scene:
Setting a theme on the top control ScoreEntry, which will set all in the scene, provided you have made custom theme entries like Label themes. Changing the labels font and colors, sizes, outlines, shadows. If you change the size you can Select Char Margin to adjust the distance between the characters. `,description:"",tags:null,title:"Creating custom Score Entry scene",uri:"/pingod-addons/tips-guides/scoreentry/index.html"},{content:`We’ll use the template project we built for PinGod P-ROC for this guide / overview. examples/pingod-netproc-pdb
We will focus on the P3-ROC / PDB board. We do intend to add the same for the original P-ROC board, it’s just untested.
This uses a IFakeProcDevice, but that can be swapped with a single simulated flag when the game is created in the ProcPinGodGame.cs.
You will want more or less the setup from the beginning guide.
pingod-addons dotnet sdk 6+ Godot 4 Visual Studio (Debugging / Scripting) pingod-netproc-pdb 🔗 Examples Project: PinGod-AddOns–pingod-netproc-pdb
This is a similar game setup to a PinGodGame but with added scripts and references to use and work with NetProc.
NetProc library - Project References The csproj in this directory has references to NetProc which are installed from the nuget packages folder.
The folder for the packages is set in the nuget.config. You won’t need to worry about this unless you update the NetProc, which is just a case of adding packages to the nuget folder and updating to a newer version in the csproj.
Note: When running a real board you need to build as x86. If the game is not set to simulated then it will load the pinproc library
`,description:"",tags:null,title:"Getting Started",uri:"/pingod-addons/pingodnetproc/getting-started/index.html"},{content:`This scene is autoloaded with the game with all of its child scenes.
Selecting the scene will display the exports options in the scene inspector, see the highlighted right panel in the image.
Scene Tree Top left pane scenes in PinGodGame.tscn
Link Scene Description Details MachineConfig Manage machine items, ball search Details Resources When this is loaded with the scene it looks for the packs you have set in the Resource Packs. Details Trough Manage machine items, ball search Details AudioManager Scene the game uses to manage audio, media controller res:// paths for assets Quicker to add asset paths here
`,description:"",tags:null,title:"PinGodGame.tscn - Scene",uri:"/pingod-addons/plugins/pingodgame/pingodgame.tscn/index.html"},{content:`Basic Score Mode for multi players. Game/Modes/ScoreMode
Exports (ScoreMode.cs) _single_player_p1_visible = false . Show Player ones ScoreP1 label if set to true. Normally in a pinball the scorep1 would not display with main score unless multi-player _show_main_score_multiplayer = true Select NodePaths for the following export properties. A NodePath here should be a label that is added to your layout. If you have 8 players then create 8 labels and add them to the array in godot editor.
_ballInfoLabel _playerInfoLabel _scoreLabel _score Labels Scene Tree breadown BackgroundColor = Simple green color CenterContainer with logo and ScoreMain label in center screen Score Labels PlayerInfoLabel BallInfoLabel CreditsLabel Creating / Modding your own ScoreMode See Create Custom Score Mode
`,description:"",tags:null,title:"ScoreMode - Scene",uri:"/pingod-addons/base-scenes/score-mode/index.html"},{content:` Building Project You should be able to build the project with dotnet build providing you have linked or added the addons directory.
It can be built inside Godot 4 by running the project editor godot -e.
Running You can run from command line with godot no debug
Debugging Visual Studio is very good here for debugging. Use the Properties/launchSettings.json to adjust launch profiles.
Option Description Godot4 (Debug with Console) Uses the godot_console.exe. doesn’t debug with breakpoints but gives you a console window Godot4 (Verbose with Console) Uses the godot_console.exe. doesn’t debug with breakpoints but gives you verbose a console output Godot (Debug) Uses the godot.exe and can hit breakpoints in visual studio Godot (Editor) Launches the Godot Editor. Usually run without debug to just launch the godot project faster Export Release (Windows Desktop) Runs an export `,description:"",tags:null,title:"Building / Debug",uri:"/pingod-addons/pingodnetproc/building-running/index.html"},{content:`Autoloads are scenes that we are overriding from the res://autoload directory. These are plugins that can be disabled in the Godot Project Settings. Parts of your game should be able to run without these plugins but in some cases they will need each other to work fully in a game.
Scene Description Adjustments.tscn Not used. Our adjustments come from the database through P-ROC not PinGod AudioManager.tscn Same as existing. For your own sound assets. Machine.tscn This scene is edited. We’ve removed the trough and other modes PinGod uses. No need to add machine items here because done elsewhere. More info below MemoryMap.tscn Simulator: Just changed the write delay to 1000. We are not writing to memory from this node but direct in the p-roc game loop PinGodGame.tscn Has added options for P-ROC and uses new script see PinGodGameProc Resources.tscn In the inspector we are pre loading the AttractMode and ScoreDisplay WindowActions.tscn Nothing changed autoload/MachinePROC.cs The machine script is for using with P-ROC which is based on the PinGod MachineNode.
SetSwitchFakeProc - This method adds the fake switch to the switch_events queue if it’s a IFakeProcDevice. This gets invoked from windows keys on window with WindowActions.
Also overrides SetSwitch from the machine and calls SetSwitchFakeProc before that base method which signals to Godot switch handlers.
proc/MemoryMapPROCNode.cs This creates a slight different of the memory map below and frees it up on any failures.
The WriteStates here is used by the PinGodProcGameController in the RunLoop.
proc/MemoryMapPROC.cs This script overrides the existing memory map and stops writing states to memory. This is just for simulating so you can disable the plug-in if running a real board.
These states are written by the p-roc game loop here by getting the _lastCoilStates or _lastLedsStates and writing them to the mapping.
For now this seems the most consistent way when running with a simulator like Visual Pinball.
`,description:"",tags:null,title:"Autoloads",uri:"/pingod-addons/pingodnetproc/autoloads/index.html"},{content:`This BaseMode scene is loaded in the Game.tscn Modes scene used in the BasicGame project. A basic mode which has a saucer (kicker) and starts multiball when active.
BaseMode.tscn Exports BALL_SAVE_SCENE = “res://addons/PinGodGame/Modes/ballsave/BallSave.tscn”;
BallStackPinball In Node / Signals pane we have connected signals to the BallStackPinball scene
Here’s what it looks like connected inside a .tscn. The methods are inside the BaseMode.cs script.
[connection signal="SwitchActive" from="BallStackPinball" to="." method="OnBallStackPinball_SwitchActive"] [connection signal="timeout" from="BallStackPinball" to="." method="OnBallStackPinball_timeout"]Signals OnBallStackPinball_SwitchActive OnBallStackPinball_timeout BaseMode.cs - Overrides _EnterTree Gets reference to BasicGame scene which is the parent of the Modes layer (this scenes parent). game = GetParent().GetParent() as BasicGame; Loads a packed scene from BALL_SAVE_SCENE. This scene is added when called from DisplayBallSaveScene Gets reference to for the _ballStackSaucer. A [[BallStackPinball|06b.BallStackPinball]] Timer. The _ballStackSaucer is used to start multiball. _Input Processes the following actions if PinGodGame.GameInPlay and not PinGodGame.IsTitled:
start button	= calls pingod.StartGame flippers	= nothing outlanes	= add 100 points inlanes	= add 100 points slingshots	= add 50 points Methods OnBallSaved = Displays a ball save scene for 2 seconds if not in multi-ball. DisplayBallSaveScene OnBallStackPinball_SwitchActive = Sets PinGodGame.IsMultiballRunning and BasicGame.AddMultiballSceneToTree to add multiball to the tree `,description:"",tags:null,title:"BaseMode (BasicGame)",uri:"/pingod-addons/base-scenes/base-mode/index.html"},{content:`This scene is the first scene to load. It is a placeholder with nothing to load inside of it apart from two CanvasLayers, which are placeholders for Modes and an Overlay.
When the game is running, any modes that need to be visible can be added to the Modes CanvasLayer.
The CanvasLayers have a priority, so that covers mode priority in a similar fashion to DMD layers / modes from the original P-ROC.
`,description:"",tags:null,title:"ProcScene - Main Scene",uri:"/pingod-addons/pingodnetproc/procscene/index.html"},{content:`The following guides aim to have you up and running the BasicGameGodot window from the pingod-basicgame.
Install, setup Godot
Configure Godot
Basic Game Setup
Basic Game Visual Pinball
`,description:"Text about this post",tags:null,title:"Getting Started",uri:"/pingod-addons/getting-started/index.html"},{content:`The following are the default switches in the autoload\\Machine.tscn file.
When created in the Godot interface, they are saved in this scene file. You can add your switches here from text editor which is faster.
_switches = { "coin1": 0, "coin2": 1, "coin3": 2, "coin4": 3, "exit": 4, "down": 5, "up": 6, "enter": 7, "coinDoor": 8, "flipperLwL": 9, "flipperLwR": 11, "flipperUpL": 13, "flipperUpR": 15, "slamTilt": 16, "tilt": 17, "ballShooter": 18, "start": 19, "trough0": 21, "trough1": 22, "trough2": 23, "trough3": 24, "plungerLane": 25, "inlaneL": 26, "inlaneR": 27, "outlaneL": 28, "outlaneR": 29, "slingL": 30, "slingR": 31, "bumper0": 32, "bumper1": 33, "bumper2": 34, "mballSaucer": 35 } `,description:"",tags:null,title:"Adding Switches",uri:"/pingod-addons/plugins/machine/adding-switches/index.html"},{content:`Bonus scene used in BasicGame Game.tscn Modes.
StartBonusDisplay is used from the Game.OnBallEnded if the game isn’t tilted. So you can override this with your own.
Scene Tree breadown backgroundColor pingod-logo = sprite Label Timer = 1 second
Exports Default Text Display For Seconds Overrides _EnterTree sets Timer and Label If no _defaultText is available it uses translated BONUS_EOB _Ready Finds Trough scene Gets Timer and BlinkingLabel from scene tree _Input Watches actions for tilt, slam_tilt Both tilting actions disable flippers and ball saves. Show a message how many warnings or if tilted on top of everything else Methods OnTimedOut = Stops the timer, hides the scene and emits the BonusEnded signal SetBonusText(string) StartBonusDisplay(bool visible = true) = Starts display for the amount of seconds set `,description:"",tags:null,title:"Bonus",uri:"/pingod-addons/base-scenes/bonus-mode/index.html"},{content:`Download Godot 4 editor PinGod is using Godot4 Mono (C#). Godot is portable (no installation required) just the download.
On windows the easiest way is to install through chocolatey package manager using the command choco install godot-mono.
This particular package installs both x86 and x64 versions but uses x64 by default. This becomes an issue when you are wanting to run P-ROC because there’s currently no dll for x64 pinproc. Most users are on the x86 build of visual pinball and if the game was made specifically for that use in a simulator then it’s easier to set a flag when installing from chocalatey with the following in an admin powershell or terminal to force default x86:
choco install godot-mono --forceX86 --version=4.0.1 = 4.0.3 32bit is usable on x64 systems, 20/8/2023 later versions do not work, subject to change
When download is complete it will extract the 32 bit version. Extracting 32-bit ....
Now when you run godot anywhere then it will use this 32bit mono version.
Setup chocolatey godot console Godot Console debug window was removed by default so it has to be launched with a godot console executable. Make a shim to this from the choco tools directory: C:\\ProgramData\\Chocolatey\\tools
.\\shimgen.exe -o ..\\bin\\godot_console.exe -p ..\\lib\\godot-mono\\tools\\Godot_v4.0.3-stable_mono_win32\\Godot_v4.0.3-stable_mono_win32_console.exe -i C:\\ProgramData\\chocolatey\\bin\\readelf.exe
Uninstall It can be uninstalled with choco uninstall godot-mono
Dotnet SDK If you already have a dotnet sdk then you can skip this step.
Install x64: sdk-6.0.307-windows-x64-installer
or x86: sdk-6.0.307-windows-x86-installer
Git Use Git to pull the repositories from Github, you don’t need a Github account just the software to do this.
Git For Windows - Download Once the installer has started, follow the instructions as provided in the Git Setup wizard screen until the installation is complete. Open the windows command prompt (or Git Bash).Type git version to verify Git was installed.
`,description:"",tags:null,title:"Godot - Installation",uri:"/pingod-addons/getting-started/installation/index.html"},{content:`This is a Base mode using the NetProc P-ROC Mode / IMode and can be found in modes/PinGodProcMode.cs.
These modes have access to the P-ROC game controller and can be used for your game logic and switch handling.
When a mode is created it gets access to the Resources pingod plugin and gets the Modes canvas from the ProcScene.
The default modes node path, string modesRootPath = "/root/ProcScene/Modes";
About P-ROC Modes The p-roc modes have some methods to override. The ones you’ll mainly be using are:
method Description ModeStarted Called when the mode is added to the Game.Modes ModeStopped Called when the mode is remove from the Game.Modes. This mode when stopped removes the CanvasLayer from the Modes Canvas UpdateLamps When the game calls UpdatesLamps this will be called Canvas - Optional Visible Layer This base mode has a Godot CanvasLayer which has an integer property of Layer. The layer is the order drawn or visible. The P-ROC Mode priority is set here.
Any scenes (nodes) you want to show here can be added to this CanvasLayer.
method Description AddChildSceneToCanvasLayer(Node node) Creates the CanvasLayer if null and adds the node to it. RemoveChildSceneFromCanvasLayer(Node node) Remove a node from the Canvas CreateCanvasLayer(string name, int priority, bool addAsChild = true) Creates a new Canvas layer and adds to the ProcScene/Modes See examples in the modes/AttractMode.cs or modes/ScoreDisplayProcMode.cs
Clean up When a mode is removed from the IGameController.Modes it will invoke ModeStopped and here we remove our CanvasLayer from the tree.
Note: Use the Godot scene view when game is running and click Remote to see the tree sructure after modes are added.
Switch handler mode example of handling switches and invoking methods like adding credits for coins:
`,description:"",tags:null,title:"Mode - PinGodProcMode:IMode",uri:"/pingod-addons/pingodnetproc/pingodprocmode/index.html"},{content:` Empty Project Steps below to create new empty project
Create new project folder, add empty text file named project.godot Symbolic link the addons to the project so you can reuse it, remove it etc. Symbolic Link If we’re in pingod\\PinGodDev and pingod\\pingod-addons is a directory above the current then we can link the addons
SET cDir=%cd% cd ../pingod-addons/addons SET addDir=%cd% cd %cDir% mklink /D addons "%addDir%" Open project in Godot Open the project in Godot (godot -e) and you’ll see the directory which you have linked Playing the game will ask you to define a scene to load Add Scene Add new Node2D Rename the node and save it as MyScene Save in the root directory and not the addons directory The new scene should be at res://MyScene.tscn Here is the project after adding MyScene.tscn with the addons linked
Run project Hit play in Godot and run project. Select this scene as the current scene to load.
`,description:"",tags:null,title:"PinGodGame Projects - Create Template",uri:"/pingod-addons/tips-guides/new-project-pingodgame-template/index.html"},{content:`Most PinGod Godot plugins are included in the addons/AutoLoad directory.
These plug-ins (addons) are used to enable certain features or disable them completely for when you want to develop scenes on their own for instance.
These should work in any order you enable them. When you enable one then this is added to the Autoload in Project Settings.
Plugin Description WindowActions Helper to use keys with window, switches, should always be enabled to close with ESC and F2 to move window Adjustments Global adjustments so your scene scripts can use global variables or settings Resources async loading of scenes, stores packed scenes and loads packs etc Machine Plugin to hold all the machine items and do other things like switch handling MemoryMap Windows memory mapped file for sharing states with a simulator or other applications AudioManager Load audio assets and play PinGodGame Main game framework Overriding the autoload scenes The autoload folder in the game directory is used when you want to override the AutoLoad scenes.
If you wanted edit and add more switches to the plugin then you would want to have your own copy of Machine.tscn.
In Godot you can duplicate the default Machine.tscn like so:
In the FileSystem pane filter files. Filter it for machine The one we want is Machine.tscn. There will be just a few results. Right click Duplicate and suffix it with 2 or anything like Machine2.tscn You will see it in the FileSystem and you can right click MoveTo, move to the res://autoload folder With the FileSystem still open, on the moved file, F2 and rename back to Machine.tscn Now you can open the res://Machine.tscn and change some settings, add items.
Disable, then Enable the plugin in ProjectSettings/Autoload for the plugin to pick up your AutoLoad scene
Extra info To find what scene the AutoLoad is using by default go to Project Settings > AutoLoad, in the list is the path to the scene.
You can also click to take you there. Note: You will need the plugins activated to see them in that pane.
`,description:"",tags:null,title:"Plugins - Autoload",uri:"/pingod-addons/plugins/index.html"},{content:`Basic Score mode with charachter selection for players at end of game
Scene Tree breadown ColorRect = Black background color SelectedChar Label = HI_SCORE_ENTRY CenterContainer = Shows selected char center screen ColorRect2 = used as red marker under a letter Exports (ScoreEntry.cs) _includeZeroToNine _nameMaxLength _playerMessage _selectCharMargin = space when changing between chars with flippers _selectedChar Overrides _Ready Sets IsPlayerEnteringScore to true quits the scene if no players or MoveNextPlayer
_Input If scene is visible and IsPlayerEnteringScore then flippers and start button actions are watched to then be able to select letter.
`,description:"",tags:null,title:"Score Entry",uri:"/pingod-addons/base-scenes/score-entry-mode/index.html"},{content:`Base Tilt scene to act on any machine tilting actions and displaying
Scene Tree breadown background CenterContainer > BlinkingLabel = Shows the tilt messages Timer = 2 second One Shot timer
Exports Num Tilt Warnings = Warning before titlt , default = 2
Overrides _Ready Finds Trough scene Gets Timer and BlinkingLabel from scene tree _Input Watches actions for tilt, slam_tilt Both tilting actions disable flippers and ball saves. Show a message how many warnings or if tilted on top of everything else Methods SetText(string) ShowTilt() ShowWarning() `,description:"",tags:null,title:"Tilt Mode Scene",uri:"/pingod-addons/base-scenes/tilt-mode/index.html"},{content:`In the examples directory is a folder named !template. This is bare bones minimum. If on windows just copy and paste this folder in the examples and rename it to your name.
What’s in the folder? Properties This contains a launchSettings.json for launching the project inside Visual Studio. These are launch profiles and you can add your own in this file.
_link_addons.bat This file is explained in previous here but it will link the addons folder into the game so you are not duplicating files, just reusing the addons.
project.godot This is the Godot project for the game. It uses the .csproj and .sln to build mono c#.
You can edit the project.godot before launching it with Godot and change the name in there to save time under config/name.
Under the [dotnet] section you can change the assembly name from PinGod to your name. If you do this just delete the PinGod.csproj and PinGod.sln files then when you run it in Godot create a new C# which will create the solution and project.
Loading project first time Run the addons link so you have the addons enabled and have a shortcut to it. Run project with godot with the editor flag -e After loading you should see the addons directory. If you go to Project Settings > Input Map you can see some inputs to control the window.
Playing first time If you try to play the project then it will ask for a scene. Just create any old scene name it MainScene and set that as the start up. You can right click any scene to set as startup or run a scene with F6.
Play the project and you should be greeted with a grey screen window, push ESC to close but doesn’t close. You need to wire ESC to close the window or we can use the plugin WindowActions.
Enabling a plugin Go to Project Settings > Plugins and you will see a list of plugins that have been loaded from the addons folder.
Enable the WindowActions. What this will do is add an Autoload entry and autoload when a game is run. Even if you are running a scene on it’s own, these autoload also load there.
You maybe greeted with an error here, this could be because you’re missing a csproj and solution.
To fix this, create a new C# script in the root inside godot, this will trigger it to create those files.
A build button should now be available (top right of Godot), build the project then enable the WindowActions.
Using the WindowActions plugin Load the game again and use ESC, which should now close the game. F2 will also be available to toggle the window border
Enabling all plugins Enable all plugins and you should be greeted with a similar log to this.
Using the addons default startup - MainScene Duplicate the MainScene.tscn into the root and set that to run first scene.
In this scene is an Attract mode, Pause and settings setup and can run the default game scene.
If you run the scene with all plug-ins enabled you will be able to add credits and play a complete game, that is, start it, drain balls, show bonus until a game over.
To troubleshoot, push F1 and change the logging level to verbose or debug in the PinGod tab.
You can reset the game with F3, move the window by toggling the border with F2.
Enable a Switch window A switch window can send switches to the machine which is much faster than using key presses.
To enable this go to your duplicated autoload/Machine.tscn and in the inspector Switch Window.
Enable it and QuickLoad the window scene by filtering for playfield. Select PlayfieldWindow.
Run the game and you will have a window to push switches. To customize see the Playfield Switch Window
`,description:"",tags:null,title:"Blank Empty Projects",uri:"/pingod-addons/tips-guides/new-project/index.html"},{content:`The nuget package NetProc.Data will provide you with .sql files to seed database data.
This creates a sqlite database and saves everything here from the game, from machine items to game audits and adjustments.
Adjustments Switches Coils LEDS Lamps Audits GamesPlayed BallsPlayed Players Scores You will edit one of the files depending on the machine type, but before you edit, copy the file and create your own into the sql directory. So usually you edit this file outside of Visual Studio.
If you try and edit the file from the package it will never save it, so create your own in same directory by copying the sql directory from the package
If you’re using a p3-roc or intend to just for simulating then the sql file we’ve used in this project is a good starter. (subject to change)
You can adjust everything in this template then create the database. This can easily be done from the PinGodGame.tscn / PinGodGamePROC by enabling delete on init and running the game.
Development settings To change the display settings in the database use the tool like sqlitestudio to edit rows in the Adjustments table under SubMenuName DISPLAY by loading the netproc.db into the application. Helpful when you want to run in a window and other settings.
Quick overiew of tables Besides the common Switches, Coils, Leds, Lamps some need a bit more explanation.
Machine Settings for the machine and the type of machine it is where the default is set to 7 for a PDB machine. Balls per machine for the trough and display monitor game runs on.
Adjustments Game adjustments table with means to be shown in a UI.
Rows should have unique keys with their MenuName and SubMenuName to be used for service menus and the like. A default example would be a parent / child STANDARD_ADJ / GENERAL.
Value must be integer. OptionType is the type of options display in a UI.
0 = A range of ints like 8,30 or 0,15 1 = List ints like 0,1,2,3 or 10,20,75 2 = Enum like NO,YES which would be 0,1 to display text for option Players There is a player named default with initials (display name) set to NETPROC. Players should have a unique login name but display name (initials) could be the same as another player.
Players are used in the BallsPlayed and Scores tables.
Scores Scores stored here against he GamePlayedId and PlayerId are storing the total points scored in that game.
Games Played Games played just stored the start and end time of the game with the duration and the adjustments that were set.
Balls Played Saves every balls score for each player and the duration.
`,description:"",tags:null,title:"Database - Sql",uri:"/pingod-addons/pingodnetproc/database-sql/index.html"},{content:` Duplicate Template Copy the pingod-projects-template and paste in same directory, by default windows adds - Copy to the directory Rename the copy to pingod-projects-myproject You will need to link the addons again. Delete addons directory then run the _link_addons.bat project.godot file You can add a project name to the project.godot file before running the project. These are found under [application] in the config section. Yours will be empty.
The reason why we’ve added Projects-MyProject here is that it’s easier to tell the type in the appdata, because this is the name it will be saved under in the users app data. By default though logs are put into the project directory.
[application] config/name="Projects-MyProject" config/description="Test project for scenes etc..." run/main_scene="res://MyScene.tscn" Godot Load project in Godot with godot -e The project should play. F5, ESC to exit Instance a scene from the addons. SwitchOverlay.tscn
Right click in the MyScene node and Instance Child Scene Browse to the addons\\Scenes\\ and select SwitchOverlay.tscn Play the project, although it will play there will be an error, because the script is using C# and it doesn’t have an assembly for the project.
`,description:"",tags:null,title:"PinGodGame Projects - New From Template",uri:"/pingod-addons/tips-guides/create-pingodgame-template/index.html"},{content:`A rundown and overview how we have made the templates and assets for the simulator.
Visual Pinball - Assets Provided with some table examples are assets used to create the graphics for Visual Pinball.
This gives a quick workflow for you if you’re new to working with graphics, exporting, importing to the simulator.
Playfield.svg Use this file with Inkscape. It just consists of separate layers which you can hide/unhide to do different exports.
This helps you keep inside a single file at the same size. All the plastics, lights, playfield can be exported separate quickly and reimported to VP.
Layer Description Wood Base background layer, export with playfield Blueprints Used to see where your objects are placed Playfield main artwork layer Plastics Use a plastic blueprint from vp Inserts Light inserts (optional) think better when using RGB lamps Decals Decals (optional) Exporting Show what you need from the layers then use the Export PNG image set to Page.
WebP If you export the playfield layer from the svg to png then it can be fairly large. You can convert the image to webp that you get from the Tools - Images section.
magick convert vp-playfield.png vp-playfield.webp
Import To Visual Pinball Open the Visual Pinball ImageManager using F3.
First you will use Reimport From on your new image but any changes after that you can just Reimport from the same location.
Blueprints When you export this from visual pinball and replace the blueprint provided in the same folder then your playfield.svg should also update with the latest blueprint.
File > Export > Blueprint
`,description:"",tags:null,title:"Simulator - Assets",uri:"/pingod-addons/game-assets/simulator-assets/index.html"},{content:`Godot Actions - Window inputs Godot uses Actions and we can use the input map to map switches / actions to them. In the projects Input Map add switches to keys like the example below.
sw20-27 are unassigned, but sw19 is set to 1, which is the default start button.
This start button action 19 (sw19) is checked states in the PinGodGame.cs._Input
You will need to add switches you want to use on the window in the WindowActions.tscn scene.
Switch numbers convert to these actions sw{number} and you can assign keyboard to test game switches directly in the window.
You can create any action here and trigger it from a simulator controller. See pause and quit which is assigned to ESC but also used externally.
`,description:"",tags:null,title:"Actions",uri:"/pingod-addons/plugins/machine/adding-godot-actions/index.html"},{content:` Create Script for MyScene This template project will not give any errors if you don’t use a C# script but if using an existing scene that uses a script then we will need to add a C# project to the directory.
As we’ve referenced a C# script from the SwitchOverlay.tscn then we need to make a C# project.
Fortunately that is easy as we just have to make a new script to do this.
Selecting the MyScene node and in the inspector bottom right of the pane is a script section Add new script here selecting C# When you click create, a project will be created and open the script in a Godot window New files Projects-MyProject.csproj Projects-MyProject.sln MyScene.cs External editors With the project open in Godot go to:
Editor > Editor Settings > Mono > External Editor
Visual Studio Code Visual Code is the fastest to open the C# files, if you select from the options and double click MyScript.cs that will open for editing in VsCode.
Visual Studio Godot CSharp Debugging Add the following to the .csproject to launch projects from there but can’t hit breakpoints.
Program C:\\Godot\\Godot.exe --path C:\\Path\\To\\Godot\\Project\\ Debugging Extension Difficult to get working…
An Visual Studio extension can be used for debugging Godot. Download and run the .vsix.
godot-csharp-visualstudio/releases/tag/v2.0.0
Godot4 better for this as you can use .net6 by default.
Godot 4.x changes to use .NET [Core] Runtime rather than Mono, so you can attach to a running Godot game with the VS debugger.
`,description:"",tags:null,title:"PinGodGame Projects - Add a C# Script",uri:"/pingod-addons/tips-guides/create-pingodgame-template-script/index.html"},{content:`🗒 MachineConfig Class Reference
Ball Search Machine Items Recording Playback Switch Window Trough PlungerLane BallSaver `,description:"",tags:null,title:"Plugin - Machine",uri:"/pingod-addons/plugins/machine/index.html"},{content:` Scenes using this Most modes rely on this being loaded by using GetNode<PinGodGame>("/root/PinGodGame") from the script.
This scene doesn’t house any other scenes, it’s autoloaded with the game and you can add your own and override its script if need to.
The BasicGame uses its own script based on PinGodGame named CustomPinGodGame which is autoloaded with the plugin.
PinGodGame.tscn Scene Guide PinGodGame.cs Class Definitions `,description:"",tags:null,title:"Plugin - PinGodGame",uri:"/pingod-addons/plugins/pingodgame/index.html"},{content:`The Resources plugin is a preloader of assets and scenes which are called Packed Scenes.
We can get a resource from this autoload and instance scenes with it.
Packed Scenes These scenes will be loaded with a game. By default the BasicGame loads the following:
Bonus.tscn ScoreEntry.tscn Tilt.tscn Multiball.tscn Attract.tscn Once these scenes and all resources have been loaded then we add a scene like the Attract.tscn.
Resources (Key, Path) Add resources that will be pre loaded in this dictionary by key , path.
Key: nameForScript , Value res://assets/yourasset.ogv.
Key: nameForScript2 , Value res://myotherassets/asset.tscn.
Resource Packs By default the collection includes the export packs in the previous section. pingod.gfx.pck and pingod.snd.pck
You can add with the scene inspector or directly in the tscn file it saves to.
`,description:"",tags:null,title:"Plugin - Resources",uri:"/pingod-addons/plugins/resources/index.html"},{content:`This scene overrides from the default PinGodGame.tscn from autoload/PinGodGame.tscn.
In Godots scene inspector there are some developer debugging options that can be set:
Option Default Description Proc Delay 10 Delays the proc loop, setting 10 here helps with simulation Delete Db On Init off Deletes the database when the game is run. This is useful if you add switches to the .sql import file when developing Simulated on Game is simulated and will use a FakePinProc LogLevel Verbose Log level setting for PinGodGame and NetProc. Autoload This scene will only be run when the pingodgame plugin is enabled in the ProjectSettings. If disabled you should see a message in the logging window like "PinGodGameProc no ProcScene found."
In some cases you may want to turn all plugins off and develop scenes on their own. For instance if you disable this from the autoload plugins menu you will still see the attract mode. We would usually leave the window actions plugin enabled though to be able to quickly close the window with the ESC key.
You can also run scenes on their own without the attract from the scene you’re on by running with F6 in godot.
Script The scene uses a new script based on the PinGodGame.cs named game/PinGodGameProc.cs. Some notes on what this main script does.
The script creates the P-ROC device and GameController created as a PinGodProcGame in the script.
Applies window settings that are saved in the database (you can set your display defaults in the SQL file)
AutoLoaded and can be retrieved from /root/PinGodGame the same as a normal PinGodGame. To access the PROCGame from here to access the PROC from a scene would be like so, _pinGodProc.PinGodProcGame
Waits for all resources to load from the game you have set and then will it start the PinGodProcGameController.
`,description:"",tags:null,title:"PinGodGamePROC",uri:"/pingod-addons/pingodnetproc/pingodgame.tscn/index.html"},{content:`Helper class for audio.
SfxAssets VoiceAssets MusicAssets Class Reference note: if you want the finished signal on audio to work when the file is an ogg, the loop must be unchecked then reimported from the import tab
_EnterTree Initializes the AudioStreamPlayers. Loads sound pack resources and adds any assets found into the provided dictionaries. Will try and load a pingod.snd.pck asset file, tries res://pingod.snd.pck first, then the working directory Methods PinGodGame has wrapped some of the common methods so you can use a PinGodGame reference to do audio controls, playing audio, music, voices.
`,description:"",tags:null,title:"Plugin - AudioManager",uri:"/pingod-addons/plugins/audiomanager/index.html"},{content:`This class writes coil, lamp and led states to memory. It also read switch states from memory.
It is used in conjunction with the controller for COM to sync with the simulator.
You only need to enable this when you intend to run it with a simulator.
The image below shows your options.
Option Description Is Enabled Enable/Disable, this allows to have plug-in enabled but disable it here Write Delay Delay for the loop to write to memory, 10 is a good default, less cpu, responsive Read Delay Delay for the loop to read from memory, 10 is a good default, less cpu, responsive MapName should be left, the controller uses this name Coil Total If you need to increase coil count, it needs to match the VP script Led Total If you need to increase led count, it needs to match the VP script Lamp Total If you need to increase lamp count, it needs to match the VP script Switch Total If you need to increase switch count, it needs to match the VP script MutexName should be left, the controller uses this name `,description:"",tags:null,title:"Plugin - MemoryMap",uri:"/pingod-addons/plugins/memorymap/index.html"},{content:`You can ignore this if you have installed with chocolatey as this will be added for you on install. You should be able to run godot in any terminal.
Set Environment path to Godot How to add to environment (Windows) After any one of these steps you choose is completed then you can load Godot from anywhere using godot.
Windows UI - (Option 1) Push windows key and type path, this will filter to Edit the System Variables Push enter to open the System Properties window, from here select Environment Variables In the Environment Variables - System Variables (bottom window pane) find variable named Path and open this Add in the godot path in this section, OK out of the windows `,description:"",tags:null,title:"Godot - Configuration",uri:"/pingod-addons/getting-started/configuration/index.html"},{content:`This is the main GameController for P-ROC which is based from a Database Game Controller in NetProc.
Here you will find the main event methods for a game and get access to the machine items from the .sql
When the game is created a database will be created for the game which holds settings, saves audits. All of this is taken care of but you can add your own audits / adjustments to the .sql
Run Loop This class overrides the existing GameControllers RunLoop method. This run loop is slightly different to cater for simulation, but will work the same when not simulated.
For simulation, the states are written to memory. Coils, Lamps and LEDS.
Initiliazing When the PinGodGameProc.cs script is fully loaded then MachineResourcesReady is called on the game. In this method we create new modes and reset the game.
Reset This links ball saves with the trough and adds all of our base starting modes to the p-roc Game.Modes.
Coils You will be using coils inside modes and the game. Just access the Coils dictionary from the game like:
Game.Coils["saucerEject"].pulse(40)
You should only use Enable if using a simulation.
Database Calls You can add your own adjustments and audits in the sql file:
GetTopScores() // get top scores in database. GetAdjustment("DISP_MODE") // gets an adjustment value GetAudit("CREDITS") // gets an audit value SetAdjustment("DISP_MODE", 2) // gets an adjustment value SetAudit("CREDITS", 50) // gets an audit value`,description:"",tags:null,title:"PinGodProc GameController",uri:"/pingod-addons/pingodnetproc/pingod-procgame-controller/index.html"},{content:`The Trough.tscn handles the ball trough, ball saving, trough switches, early save switches, shooter lane switch.
Included in the examples PinGodGame.tscn tree.
Most cases you will leave the default settings here, but you can add more switches from this.
Trough / Ball Save Options Trough switche names Ball save seconds Ball save lamp name Ball save multiball seconds Early Ball Save switches Number of Balls to Save Trough Solenoid(coil) Plunger lane switch name Set what to do on ball started Ball search options Enabled Ball Search coils to pulse (usually saucers, scoops not the trough) Search stop switches = Switches that stop the search Search wait time = Time to wait before pulsing coils Trough Class Reference `,description:"",tags:null,title:"Ball Trough (Scene)",uri:"/pingod-addons/plugins/machine/trough/index.html"},{content:`Trough This is loaded with the game controller. All you have to do is tag your switches correctly in the .sql machine configuration and it will handle the rest.
Make sure you have trough tags for switches and eject coil. Plunger lane, early save switches if using. These can be found in the example .sql.
Other modes In the proc/PinGodProcGameController.cs script when the game resources have fully loaded and called MachineResourcesReady, then some modes are created.
Ball Save When this is created a lamp / led name is passed in and a switch to active the ball save.
After the switch to activate the ball search is opened the ball save is started, in this case the plungerLane.
Ball Search When this is created it runs through our switches for stop and reset values and adds any coils tags with search.
MachineSwitchHandlerMode This is just a simple mode that we can handle some base switches like the coin door.
We have tagged all of our door switches with door so when this mode is created, it adds a handler to HandleDoorSwitch for every switch tagged with door, then the switch is dealt with in this single method.
Currently it is just adding credits but is setup for a coin door and service menu.
`,description:"",tags:null,title:"Mode - Base Modes",uri:"/pingod-addons/pingodnetproc/proc-basemodes/index.html"},{content:`Using Git to clone the repositories
(Option 1) Clone Create a directory on machine where you are developing named pingod, this could be anything but we need a directory to clone into.
Browse into the pingod directory just created and use keypress ALT+D then type cmd and enter, to open windows cmd prompt in that directory.
Clone the pingod-addons with git clone https://github.com/FlippingFlips/pingod-addons
You could also use your own fork and clone that.
(Option 2) Downloading zipped repo pingod-addons
`,description:"",tags:null,title:"BasicGame - Git Clone",uri:"/pingod-addons/getting-started/basicgame-setup-git/index.html"},{content:`To run the pingod-basigame project in the examples folder you will need the addons directory linked or copied.
These addons contain base files and pinball framework that a game will use.
Link the AddOns You can copy the addons directory to your project (pingame-basicgame in this example), but it’s better to use a symbolic link to the addons so we are not duplicating files and you can keep the same addons across projects.
Running the _link_addons.bat launcher file will symbolic link the addons. A shortcut addons folder will be added in the Godot project.
This will just flash up and disappear, but you will see a new folder shortcut created called addons
Opening shortcut If you open the new addons shortcut it will show you the files that are in pingod-addons\\addons.
This shortcut can be removed without harming the files where the directory is linked from.
`,description:"",tags:null,title:"BasicGame - Linking addons",uri:"/pingod-addons/getting-started/basicgame-setup-addons/index.html"},{content:`Use Alt+D in the project directory, type godot -e then enter to load project
You can use the .bat files in the projects root directory to load a project with . _godot_editor.bat
Load projects into Editor by adding --path:
godot -e --path pingod-basicgame/BasicGameGodot
Add on linking file what it does _link_addons.bat Links the pingod-addons the epositories addons directory into the BasicGameGodot project `,description:"",tags:null,title:"BasicGame - Project.Godot",uri:"/pingod-addons/getting-started/basicgame-project/index.html"},{content:` Build The MainScene should be visible and you can build the project (top right button).
If build success, you can re-enable the plugin if you need to, but it isn’t needed to run the game for now.
Play Hit F5 to run the game. You can use F2 on the window to show border.
Output window Quit ESC to exit the window
`,description:"",tags:null,title:"BasicGame - Godot Project Loaded Run Game",uri:"/pingod-addons/getting-started/basicgame-godot/index.html"},{content:`Window controls key action F1 Settings Menu F2 Toggle Window Border F8 Quit? Settings Saved to users app_data directory. See Game Save Data
Audio Display Fullscreen FPS Game Some basic game settings PinGod Read/Write states can be disabled when not running with a simulator. `,description:"",tags:null,title:"BasicGame - Window Settings",uri:"/pingod-addons/getting-started/basicgame-window/index.html"},{content:`In the appdata directory under the game name. %AppData%\\Godot\\app_userdata\\BasicGameGodot\\Logs.
Rolling logs, with the godot.log is always being the latest.
Log location In the appdata directory under the game name, logs .log are created. Use the open_app_data.bat to open directory to open the data directory.
The image above has the users game directory open with Sublime Text.
SublimeText - Game Data Folder The steps to add this directory and save project:
Find the games appdata folder with bat file _godot_appdata_dir.bat Copy the path and use Open Folder in Sublime and open the user path Save this as a project in sublime then reopen it later for a quicker way to view logs, look at your settings etc for this game. You could do similar with a Visual Studio code workspace or something else, this is just one example of getting back to viewing this directory.
Log example: Godot Engine v3.4.2.stable.mono.official.45eaa2daf - https://godotengine.org OpenGL ES 3.0 Renderer: Radeon RX 570 Series OpenGL ES Batching: ON PinGodGame:_EnterTree. getting cmd args PinGodGamecmd line available: 0 GameSettings:loaded:Ok PinGodGame:_EnterTree. log level: Debug PinGodGame:window: size:766x478 pos:403,221, onTop: True PinGodGame:window: project settings size: 1024x600 PinGodGame:looking for game patches. res://patch/patch_{patchNum}.pck . From 1. patch_1.pck, patch_2.pck Script Usage Game logging Class Reference
Use from a pinGod reference or the static Logger in script like Logger.Debug
void Debug(params object[] what); void Info(params object[] what); void Warning(string message = null, params object[] what); void Error(string message = null, params object[] what); PinGodLogLevel LogLevel { get; }`,description:"",tags:null,title:"Logging Games",uri:"/pingod-addons/getting-started/logging/index.html"},{content:`In the modes directory you’ll find AttractMode.cs. This is a P-ROC mode script using the base PinGodProcMode because we need visible layers here.
This needs a reference to a scene in res://scenes/AttractMode/AttractProc.tscn.
This scene is set in the Resources.tscn to pre load it so we can just create instances when we need to.
Scene and Script The script gets the top scores from the database and populates the base Attract.cs high score lists. Usually this is done from the Adjustments in a normal PinGodGame.
There is no change to the scene file, only it uses a new script to get scores from a different source.
P-ROC Mode Start / Stop When the Mode is started then this scene is retrieved from the Resources and added as a child.
When the Mode is stopped then the scene instance we created is freed and removed.
P-ROC Switch Handlers This mode starts the game and this is done by using the switch handler bool sw_start_active(NetProc.Domain.Switch sw)
You can name your handlers like this or use sw_start_active_for_1s or sw_start_active_for_1000ms for held down handlers.
When they are made like this the ScanSwitchHandlers from the NetProc looks for these methods in the mode.
`,description:"",tags:null,title:"Mode - AttractMode",uri:"/pingod-addons/pingodnetproc/attractmode/index.html"},{content:`In the appdata directory under the game name. %AppData%\\Godot\\app_userdata\\BasicGameGodot\\settings.save
You can open this directory with the _godot_appdata_dir.bat file
settings.save and gamedata.save Settings.save is json text, readable and editable.
This stores
display settings game settings audio settings memory map options You don’t need to edit these by hand. Any of your added settings will go in this file. Recordings See Recordings / Playback
`,description:"",tags:null,title:"User Game setting / data",uri:"/pingod-addons/getting-started/game-save-data/index.html"},{content:`In the modes directory you’ll find ScoreDisplayProcMode.cs. This is a P-ROC mode script using the base PinGodProcMode because we need visible layers here.
Uses a reference to a scene in res://scenes/ScoreMode/ScoreModeProc.tscn. This scene is also set in the Resources.tscn. To pre load it so we can just create instances when we need to.
Scene and Script The change here lies within the ScoreModeProc.cs which is based off the existing PinGodGame ScoreMode.cs.
We just override where the scores are coming from, in this case coming from the P-ROC game players and ball information.
P-ROC Mode Start / Stop When the Mode is started then this scene is retrieved from the Resources and added as a child.
When the Mode is stopped then the scene instance we created is freed and removed.
P-ROC Switch Handlers None
🔗 Base ScoreMode Scene
`,description:"",tags:null,title:"Mode - ScoreDisplayProcMode",uri:"/pingod-addons/pingodnetproc/scoredisplayprocmode/index.html"},{content:`An overview of the parts used to create the BasicGame with PinGod.
These should give you a good idea of how to use these in your game and a good starting base.
Main Scene
Attract Scene
Game Scene
Score Mode Scene
Base Mode Scene
Bonus Mode Scene
Score Entry Scene
Tilt Scene
`,description:"",tags:null,title:"BasicGame - Scenes Overview",uri:"/pingod-addons/base-scenes/index.html"},{content:`Game Window Switches Switches need to be added to _gameWindowSwitches in PinGodGame.tscn.
Keys action 5 Credits Q,W,E,R Trough Switches 1-4 1 Start Game T Plunger Lane Y inlane_l Simulating with keypress actions Add credits (5), hold all the trough switches down (QWER) then push start (1).
Add some points with Y = switch 22 inlane_l
Simulate ball drain by activating all the trough switches.
Use T plunger lane switch and drain to activate a ball save
`,description:"",tags:null,title:"BasicGame - Game",uri:"/pingod-addons/getting-started/basicgame-game/index.html"},{content:`Now that the game is building and running it can be launched with Visual Pinball.
Table can be run from any location in Visual Pinball.
The controller will load the game when the vp player is launched.
Use the _vp_edit.bat or _vp_play.bat to open the table easier
Provided you have the controller setup and have built the godot project this will play.
COM Controller setup We need to register the COM object before it can be used by other applications such as a simulator.
Register controller Download the latest at pingod-controller-com/releases Extract the download anywhere on your system (if following this guide it may pay to create a new folder pingod-controller-vp inside pingod) Run the PinGod.VP.Setup.exe to unregister / register the COM object If you move these files then you would have to register it again. Simulator Scripts Copy the scripts directory to your visual pinball directory. This would be something like C:\\Visual Pinball\\Scripts Script What For? core_c_sharp.vbs a copy of core.vbs but with slightly different ways to invoke methods in C# PinGod.vbs a machine definition file Debugging When debugging you can load the game direct from the BasicGameGodot directory as you would in the previous section only with the controller.
This table script is set to Debug = True and to look for ..\\, which is a level up in this repository.
These settings will run the game debug with the Godot editor, show display and also a console window.
`,description:"",tags:null,title:"BasicGame - Visual Pinball",uri:"/pingod-addons/getting-started/basicgame-game-vp/index.html"},{content:`In the modes directory you’ll find MyMode.cs. This will be loaded when the default game is run.
It’s is a P-ROC mode script using the base PinGodProcMode but this mode doesn’t dislay any scene, it runs as mode to and just access game controller.
P-ROC Mode Start / Stop When this mode starts it uses the Trough to launch a ball in play.
P-ROC Switch Handlers switch Description start adds more players if ball=1 , credits and players < 4 slingL add 100 points slingR add 100 points inlaneL add 100 points inlaneR add 100 points outlaneL add 100 points outlaneR add 100 points saucer when active for 1 second it will add 250 then eject the ball bumperHit All bumpers call this for 150 points targetHit All targets tagged with targetBank add 200 points Creating Modes `,description:"",tags:null,title:"Mode - MyMode",uri:"/pingod-addons/pingodnetproc/mymode/index.html"},{content:` Using in game scene c# scripts Connect to the Machines SwitchCommand. Only needs to be done once in _EnterTree or _Ready.
public override void _Ready() { base._Ready(); if (HasNode("/root/PinGodGame")) { pinGod = GetNode("/root/PinGodGame") as IPinGodGame; //use the switch command on machine through the game as we're in a game pinGod.MachineNode.SwitchCommand += OnSwitchCommandHandler; } else { Logger.WarningRich(nameof(BaseMode), "[color=red]", ": no PinGodGame found", "[/color]"); } }Create handler.
/// <summary> /// Switch handlers for lanes and slingshots /// </summary> /// <param name="name"></param> /// <param name="index"></param> /// <param name="value"></param> private void OnSwitchCommandHandler(string name, byte index, byte value) { if (value <= 0) return; switch (name) { case "outlaneL": case "outlaneR": game.AddPoints(100); break; case "inlaneL": case "inlaneR": game.AddPoints(50); break; case "slingL": case "slingR": game.AddPoints(50); break; default: break; } }Check a switch state bool switchOn = Machine.Switches["plungerLane"].IsEnabled //or switchOn = pinGod.IsSwitchEnabled(swName);`,description:"",tags:null,title:"Using Switches C#",uri:"/pingod-addons/plugins/machine/usingswitches/index.html"},{content:`BasicGame-PinGod.vbs A script with the same name is included with the table. This will be used with the visual pinball script editor.
This can make it easier to edit outside of table, keep track of changes etc. At the top of this image it shows MODIFYING EXTERNAL.
PinGod.VP.Controller The first section marked in red is the PinGod.VP.Controller initialization.
Game Project Location When debugging, if the games Godot project is relative to the table (the same as this repo), then you won’t have to adjust the script for the GameDirectory.
But if you need to, you can change the default Const GameDirectory = "..\\BasicGameGodot" to:
Const GameDirectory = "C:\\Yourfilepath\\BasicGameGodot"
Variable does what? IsDebug Set controller in debug launch project directories GameDirectory Game Project to launch, see above UseSolenoids Set controller to check for states UsePdbLeds PROC led states. This project is set to leds for the ball save light UseLamps Use lamps or leds, or both Variables declared for table objects. Dim bsTrough, bsSaucer
Table Events Getting the controller to act on some table events. Stopping the game window and pause / resume
Table Init Visual pinball Table1_Init controller is Run, how it is run depends on the IsDebug flag set.
Init game This sub routine is much like a PinMame game setup and we use core_c_sharp.vbs which is pretty much the core.vbs.
vpmMapLights AllLamps This runs updated lamps from this collection. The lamp number is in the VP Light objects TimerInterval vpmCreateEvents AllSwitches This runs updated switches from this collection. The switch number is set inside the VP Triggers TimerInterval variable PinMame timers enabled Create Ball Trough set to bsTrough Create Auto Plunger set to plungerIM Create Kicker set to bsSaucer Set vpm nudging Keyboard Both Table1_Key routines use a check if Controller.GameRunning = 0 then Exit Sub 'exit because no display is available
Solenoids Callbacks Callback does what? PinGodAlive if you need to know if the pingod window is alive bsTrough.solOut Event for the trough we created. Kick the ball bsSaucer.solOut Event for the kicker on the table FlippersEnabled enable/disble flippers AutoPlunger Invokes PlungerIM.AutoFire Lampshow1 Runs a light sequence DisableLampShows StopPlay light sequence Display settings Display can be adjusted by using F2 on the window, move and resize and save the position.
.DisplayX = 1920 - 512 .DisplayY = 10 .DisplayWidth = 512 ' 1024 FS .DisplayHeight = 300 ' 600 FS .DisplayAlwaysOnTop = True .DisplayFullScreen = False 'Providing the position is on another display it should fullscreen to window .DisplayLowDpi = False .DisplayNoWindow = False Default Machine Switches These are used by Visual Pinball common switches which are mapped to keyboard. From PinGod.VP/scripts/PinGod.vbs
Cabinet switches Const swCoin1 = 0 Const swCoin2 = 1 Const swCoin3 = 2 Const swCoin4 = 3 Const swCancel = 4 Const swDown = 5 Const swUp = 6 Const swEnter = 7 Const swCoinDoor = 8 Const swLLFlip = 9 Const swLRFlip = 11 Const swULFlip = 13 Const swURFlip = 15 Const swSlamTilt = 16 Const swTilt = 17 Const swStartButton = 19Script Notes / Guide Switch events can be sent with Controller.Switch 20, 1. Usually this is Controller.Switch(20) = 1. `,description:"",tags:null,title:"BasicGame - Visual Pinball Script",uri:"/pingod-addons/getting-started/basicgame-game-vp-script/index.html"},{content:`In the autoload/Machine.tscn you can enable a switch window. This window is embedded and kind of a hack but allows to run multple windows.
If your main display is set to full screen then you would want two displays to show the switch window.
On the window you can push switches in a separate window to the game which is quicker than key presses especially when it comes to the trough as you can toggle all here.
playfield_control.tscn is the default layout but you should make a copy of this for your own.
More help here:
🔗 Switch Window - Playfield Control
`,description:"",tags:null,title:"Simulator - Switch Window",uri:"/pingod-addons/pingodnetproc/simulator-window/index.html"},{content:`Tools are custom add on objects in Godot. You can use the attribute [Tool] and turn a script into a tool.
They will be available to search and select from in Godot so you can add to your scenes.
`,description:"",tags:null,title:"Tools / Controls",uri:"/pingod-addons/tools/index.html"},{content:`There is a demo table provided in the visual_pinball directory.
🔗 Visual Pinball 10 - Releases
Script is almost identical to the PinGodGame.
To run this with Visual Pinball you just need a COM controller setup and scripts for visual pinball then the game can be launched from the .bat files or manually.
🔗 pingod-controller-com - Releases
`,description:"",tags:null,title:"Simulator - Visual Pinball",uri:"/pingod-addons/pingodnetproc/simulator-vp/index.html"},{content:`Anything we find useful to you will go here. Any tips, tutorials.
`,description:"",tags:null,title:"Tip / Guides / Notes",uri:"/pingod-addons/tips-guides/index.html"},{content:`Notes and reminders on some assets and modes.
Video format .Webm is buggy windows? and have had better results with .ogv.
FFMPEG: Converting video examples Using FFMPEG when converting from mp4 this file will be twice the size of the mp4 (6)
ffmpeg -i input.mp4 -c:v libtheora -qscale:v 6 -c:a libvorbis -qscale:a 6 outputname.ogv
Take 10 seconds of video starting at 12 seconds * This lower quality (3) will produce about the same size as the mp4*
ffmpeg -ss 12 -t 10 -i input.mp4 -c:v libtheora -qscale:v 3 -c:a libvorbis -qscale:a 3 outputname.ogv
Join with other audio ffmpeg -i "video.avi" -i "audio.ogg" -map 0:v -map 1:a -c:v copy -codec:v libtheora -qscale:v 6 -codec:a libvorbis -qscale:a 6 output.ogv
Reverse ffmpeg -i inputvideo.ogv -vf reverse outputvideo_reversed.ogv
Join ffmpeg -i "concat:in.ogv|in2.ogv" -codec copy output.ogv
Create image frames ffmpeg -i Whirl.mp4 "%03d.png"
Image to OGV ffmpeg -framerate 25 -i image_%d.png -qscale:v 6 test2.ogv
`,description:"",tags:null,title:"Assets - Video",uri:"/pingod-addons/game-assets/video/index.html"},{content:`Basic way to use a Timer as a Saucer or Kicker, the scene using this control should connect to the switch signals sent from them. See BasicGame
Tool: BallStackPinball derives from a Timer and can be used in scenes and adding them from filtering in the Godot editor.
Exports Coil	= coil sname to pulse Switch	= switch name that activates One Shot	= true Autostart	= false
Godot - Overrides BALL_SAVE_SCENE = “res://addons/PinGodGame/Modes/ballsave/BallSave.tscn”;
_EnterTree Gets reference to PinGodGame to be able to check input actions, ie: switches _Ready Disables input processing if the _switch wasn’t given
_Input Processes the switch name given to this Timer and emits signals SwitchActive or SwitchInActive
Methods SolenoidPulse = Pulse the coil for this Timer
`,description:"",tags:null,title:"BallStackPinball (Timer)",uri:"/pingod-addons/tools/ballstackpinball/index.html"},{content:`You can export the game to be used without the Godot editor. The Build folder will be used to export to.
Godot Export file does? _godot_export_full.bat Exports Windows Desktop which is executable and main pack _godot_export_pck.bat Exports Windows Desktop Pack which updates the main pack. The executable will be the same for all games as it basically is Godot You only need to run _godot_export_full.bat one time to get the executable, this will also build a .pck, when you make changes you can run _godot_export_pck.bat to update the .pck export.
The output directory (../Build) for these exports and other settings are in the export_presets.cfg.
Shrink the executable 40 > 10mb with the _build_upx_shrink_executable.bat in the Build folder. You will need UPX Ultimate Packer for eXecutables.
Simulator Export Preparing a table is straight forward enough by changing the IsDebug in the script and specifying the executable name in the GameDirectory.
The script example above presumes the table is being run alongside this executable.
`,description:"",tags:null,title:"BasicGame - Exporting",uri:"/pingod-addons/getting-started/basicgame-game-exporting/index.html"},{content:`A label that blinks on a timer with optional methods for controlling the blink
`,description:"",tags:null,title:"BlinkingLabel (Label)",uri:"/pingod-addons/tools/blinkinglabel/index.html"},{content:`Also See BasicGame - Simulator Export
Exporting Game Command line Use the bat files export for steps. See Godot Export You only need to build the full export once. The executable will always be the same size, but the pack file will be different. Pack executable size Shrink the executable 40 > 10mb with the _build_upx_shrink_executable.bat in the Build folder.
You will need UPX Ultimate Packer for eXecutables
UPX added to environment path to run the bat file.
Before Exporting Checklists Are the Recordings / Playback / Overlays switched off? Simulator Script VP Script change to Debug=False VP Script change path to ./GameExecutable - If game is packaged with table in the same directory the player only has to run the game in Visual Pinball Helps to have a copy of the release table in the export path to test exports and keeping your debug table
`,description:"",tags:null,title:"Exporting",uri:"/pingod-addons/exporting/exports/index.html"},{content:`Add translations for the game. There are also defaults to cover most standard pinball messages in addons\\assets\\Localization
The default example covers English and French in pindog_default_lang.csv.
ReImport If the text doesn’t update when you run the game:
Find the CSV in Godot file explorer from addons\\assets\\Localization/pingod_default_lang.csv. Select the file, then look in the Import tab (next to scene top left) Click Reimport making sure it’s set as .csv, compressed Files YourGame/local BasicGameLang.csv
keys,en MY_MSG,Add key values for lanuguagesUsage Godot Scripts These can be used in Godot script by using the translation method Tr("EXTRA_BALL")
_defaultText = Tr("BONUS_EOB");
Replacing placeholders CSV
HI_SCORE_ENTRY,PLAYER %d\\r\\nENTER NAME,JOUEUR %d\\r\\nENTRER LE NOM
SCRIPT
playerMessageLabel.Text = Tr("HI_SCORE_ENTRY").Replace("%d", (CurrentPlayer + 1).ToString());
Scene text controls They can also be used in control scenes. See the AudioSettings.tscn scene where SETT_VOL_MUSIC and others are used.
Project.godot default translation [locale] translations=PoolStringArray( "res://addons/assets/Localization/pingod_default_lang.en.translation" ) locale_filter=[ 0, [ ] ]`,description:"",tags:null,title:"Language files",uri:"/pingod-addons/languages/lang-files/index.html"},{content:`Set the exports in the scene from Godot or tscn. and other options.
`,description:"",tags:null,title:"PinballLanesNode (PinGodGameMode:Node)",uri:"/pingod-addons/tools/pinballlanesnode/index.html"},{content:`Handles a bank of targets, the light states and watches for completion
`,description:"",tags:null,title:"PinballTargetsBank (Node)",uri:"/pingod-addons/tools/pinballtargetsbank/index.html"},{content:`Auto Switch handlers (No scripting) In the BasicGame, Visual Pinball table switches are added to a simulator collection named AllSwitches.
Add the switch number to the TimerInterval box in the Misc settings to 20 and add the switch to the AllSwitches collection.
All VP table objects should be covered, so you wouldn’t have to remember to use Spin instead of Hit for a spinner in the collection. (Scripting) Visual Pinball - Switch On / Off Sub sw_plunger_lane_hit() : Controller.Switch 20, 1 : End Sub Sub sw_plunger_lane_unhit() : Controller.Switch 20, 0 : End Sub Sub sw_spinner_spin() : vpmTimer.PulseSw 21: End Sub Bumper001_Hit : vpmTimer.PulseSw 29`,description:"",tags:null,title:"Using Switches (Controller)",uri:"/pingod-addons/plugins/machine/usingswitchescontroller/index.html"},{content:`VideoPlayerPinball (AddOn) This helper node for video has options to set looping and other in the inspector when added to the tree.
Tool: This can be added with Create New Node and searching in godot editor for control VideoPlayerPinball
Adding VideoPlayerPinball to Attract Mode 1. Using a duplicated BasicGame as starter project Open project with Godot Find the Attract.tscn in the addons res://addons/Modes/attract, right click and duplicate and give it a name, CustomAttract.tscn Drag the CustomAttract.tscn to the modes folder in the game project (or move in file computer system) Open MainScene.tscn in text editor and update the attract scene to res://modes/CustomAttract.tscn. Click Godot to Reload the scene. 2. Editing the CustomAttract scene Delete the Background in the scene. This will remove the grey background behind the PUSH START text. Select the top node, Attract then right click > add child Node. Search for VideoPlayerPinball and add one. * If this node isn’t in the search then the addons aren’t enabled. Push the Layout button whilst selected on the VideoPlayer and anchor it to be Full Rect to fill the screen Add your video file to the players Stream dropdown and use the options in the inspector. Check Play When Visible and Loop and run the game / scene `,description:"",tags:null,title:"VideoPlayerPinball (VideoPlayer)",uri:"/pingod-addons/tools/videoplayerpinball/index.html"},{content:` You may want to make a base asset pack to copy and load across multiple games or ship game with separate packs for easier updating.
See Link separate assets project on how to make from linking.
Your assets will keep the same file structure, so to reach one of your assets in script the path would be "res://assets/img/myimage.png.
`,description:"",tags:null,title:"BasicGame - Assets and Pcks",uri:"/pingod-addons/getting-started/basicgame-game-assets-and-packs/index.html"},{content:` In Godot right click assets directory and add a new folder named scenes. Right click the new scenes folder and add new scene and name it test_scene. Select the type of node to be a Sprite by using Other Node for this scenes Root Node. Save the scene Ctrl+S after it opens and the file will be created test_scene.tscn
Find an image on disk and drag it into the Godot path. res://assets/img/. In the example I have dropped in drac_vp.jpg. Now select the Sprite
In the inspector select the image in the texture dropdown. Quickload you will see your image, so select it there.
Use the new sprite in scene:
Open the BaseMode.tscn in Godot
On the BaseNode parent right click to Instance Child Scene
Select the sprite test_scene
Run the game and you’ll see the test_scene on screen.
`,description:"",tags:null,title:"BasicGame - Create first asset scene",uri:"/pingod-addons/getting-started/basicgame-create-asset-scene/index.html"},{content:`This script is autoloaded from the scene when the display window is launched and has many methods.
Modes and other scenes can get access to this from the root tree. GetNode<PinGodGame>("/root/PinGodGame").
Godot - Overrides _EnterTree = Gets user cmd line args, loads data and settings, creates trough, sets up ball search and audio manager
_Ready = Game initialized. Memory map is created here if read and write is enabled. Gets BallSearchOptions, sets up a _lampMatrixOverlay Gets hold of the MainScene to control window size, stretch
_ExitTree = Saves recordings if enabled and runs Quit(bool)
_Input = Listens for any escape action preses. Handles coin switches, adds credits. Toggle border F2 default
_Process = Processes playback events…Processing is disabled if it isn’t enabled or if playback is finished
Signals (Game Events) Signal list BallDrained BallEnded(bool lastBall) BallSaved BallSaveEnded BallSaveStarted BonusEnded CreditAdded GameEnded GamePaused GameResumed GameStarted GameTilted ModeTimedOut(string title) MultiBallEnded MultiballStarted PlayerAdded ScoreEntryEnded ScoresUpdated ServiceMenuEnter ServiceMenuExit VpCommand(byte index) Signals can be found in PinGodGameBase.cs. The game has to have these or at least inherit them to be able to be emitted.
You can connect to signals from a PinGodGame scene reference, See res://addons/PinGodGame/Game/Game.cs in _EnterTree for example.
pinGod.Connect(nameof(PinGodGame.BallDrained), this, nameof(OnBallDrained));
Some example methods Pretty much all can be overridden in this class.
Generate documentation from source see Setup-Generate Docs to view all methods in PinGodGame
AddBonus(long points) = Add bonus to current player
AddCredits(byte amt) = Add game credits
AddPoints(long points) = Add points to current player
BallsInPlay() = Gets balls in play from the _trough
DisableAllLeds, DisableAllLamps = Disable all lights
EnableFlippers	(byte enabled) = enable flippers?
IsSwitch (string[] switchNames,InputEvent input) = Detect if the input isAction found in the given switchNames. Uses SwitchOn
LogInfo,LogWarning,LogError,LogDebug = write to logs
StartMultiBall	(byte numOfBalls, byte ballSaveTime = 20,float pulseTime = 0) = Sets MultiBall running in the trough and Emits MultiballStarted
StopMusic() = Stops the music in audiomanager
SwitchOn(string swName) = Checks a switches input event by friendly name. If the “coin” switch is still held down then will return true
UpdateLamps = Invokes UpdateLamps on all groups marked as Mode within the scene tree. scene tree CallGroup
BasicGame usage CustomPinGodGame The BasicGameGodots CustomPinGodGame class inherits PinGodGame and is set as a script in the PinGodGame.tscn. So while the PinGodGame.tscn scene is set to autoload with a project, then any custom game classes can be by selecting it in the godot editor( safer) or by editing the PinGodGame.tscn file and changing script there.
Overrides the CreatePlayer to create a BasicGamePlayer and doesn’t call the base method.
Overrides the Setup but calls the base method.
Game.cs In this scene overriding the _EnterTree uses pinGod to log messages and connect to some signals.
`,description:"",tags:null,title:"PinGodGame.cs - Script",uri:"/pingod-addons/plugins/pingodgame/pingodgame.cs/index.html"},{content:`The BasicGame project below shows that the first project is Windows Desktop Runnable and is exporting to ..Build/PinGod.BasicGame.exe
This window can be found in Godot Project > Export.
Export option “embed pck” does not work on 32-bit x86 GodotIssues
- Feb 2020
The names here have to be consistent which each other. The exe and the pack.
The BasicGame project for Windows Desktop Pack shows that it’s exporting to ..Build/PinGod.BasicGame.pck
Excluding Assets Added this exclude filter on the Runnable and the Desktop Pack to not add any files in assets/*.
This asset folder is usable in the BasicGameGodot game directly but as the directory is excluded here for exporting from then we can build an asset pack from another project.
See
`,description:"",tags:null,title:"Export Settings - Godot",uri:"/pingod-addons/exporting/export-settings/index.html"},{content:`The file for the previous Godot Export Settings.
Icons, names, exported saves.
Export Presets Create export presets by duplicating a section, changing the preset number.
Add or change the pack names by hand here, export_paths, folders to exclude or just include
The following example is from the BasicGamePck where we are just creating with all resources apart from the audio folder.
[preset.0] name="GfxPack" platform="Windows Desktop" runnable=false custom_features="" export_filter="all_resources" include_filter="" exclude_filter="assets/audio/*" export_path="../Build/pingod.gfx.pck" script_export_mode=1 script_encryption_key="" [preset.0.options] custom_template/debug="" custom_template/release="" binary_format/64_bits=false binary_format/embed_pck=false texture_format/bptc=false texture_format/s3tc=true texture_format/etc=false texture_format/etc2=false texture_format/no_bptc_fallbacks=true codesign/enable=false codesign/identity_type=0 codesign/identity="" codesign/password="" codesign/timestamp=true codesign/timestamp_server_url="" codesign/digest_algorithm=1 codesign/description="" codesign/custom_options=PoolStringArray( ) application/icon="" application/file_version="" application/product_version="" application/company_name="" application/product_name="" application/file_description="" application/copyright="" application/trademarks=""`,description:"",tags:null,title:"Export - export_presets.cfg",uri:"/pingod-addons/exporting/export--settings-export-presets.cfg/index.html"},{content:`If you need to change the icon, which by default PinGod does then you will need RCEdit.
Make icon with all size in one file A. Download RCEdit (Github) and add the path in Godots Editor Settings\\RCedit B. Or use chocolatey choco install rcedit Change icon in the Godots Project Settings/Application/Config See https://docs.godotengine.org/en/3.2/getting_started/workflow/export/changing_application_icon_for_windows.html
`,description:"",tags:null,title:"Export Windows Icons",uri:"/pingod-addons/exporting/export-windows-icon/index.html"},{content:"Overview Links Basic Game Assets & Packs BasicGame Basic Game Resources BasicGame Audio Manager AudioManager Videos and format Videos ",description:"",tags:null,title:"Game Assets",uri:"/pingod-addons/game-assets/index.html"},{content:`This doesn’t apply if you decide to have a single executable and pck, so you can skip this
Note: This could have changed in Godot 4, untested
When a project uses mono C# it includes mono assembilies which are around 29mb.
If you try to export the assets only from this project, then you will always have the c# files.
One workaround method is to use the BasicGamePck/project.godot project. This project is empty, no scenes or C# scripts.
Linking assets
`,description:"",tags:null,title:"C# Pcks Are Large...",uri:"/pingod-addons/exporting/csharp-large-exports/index.html"},{content:`We can reuse the assets and .import directories from BasicGameGodot into the BasicGamePck project.
Use the BasicGamePck project just for exporting assets, scenes, no C# scripts.
Link assets Remove the assets directory if there is one in BasicGamePck
This can be done with _link_assets.bat helper bat file.
Symbolic link the BasicGameGodot\\assets directory into the BasicGamePck directory.
Symbolic link the BasicGameGodot\\.import directory into the BasicGamePck directory.
_link_assets.bat: SET cDir=%cd% cd ../BasicGameGodot SET baseGodotDir=%cd% cd assets SET assetDir=%cd% cd ../.import SET importDir=%cd% cd %cDir% mklink /D assets "%assetDir%" mklink /D .import "%importDir%" When we link the .import, .assets here we should be on par with the BasicGameGodot project all the time so we can just build out packs. `,description:"",tags:null,title:"Link assets into separate assets project",uri:"/pingod-addons/exporting/link-separate-assets-project/index.html"},{content:`Coils can be On, Off or Pulsed. Get a reference to /root/Machine from a scene.
var pingodMachine = GetNode<MachineNode>("/root/Machine"); pingodMachine.CoilPulse("trough", 225);`,description:"",tags:null,title:"Using Coils (C#)",uri:"/pingod-addons/plugins/machine/usingcoils/index.html"},{content:`Visual Pinball SolCallbacks When Visual Pinball detects changed coils the callbacks are invoked by coil number.
The script example to handle coil 35 would be SolCallback(35) = "Lampshow2". This will invoke the LampShow2 sub routine (if you have one)
SolCallback(1) = "bsTrough.solOut" ' The trough which makes the ball pop out.... SolCallback(2) = "FlippersEnabled" ' Enable and disable flippers, when ball drains, tilt etcSome standard callbacks for default game
`,description:"",tags:null,title:"Using Coils (Controller)",uri:"/pingod-addons/plugins/machine/usingcoilscontroller/index.html"},{content:` Script editing / Editing .tscn files outside of Godot editor It’s not great to use the Godot editor for editing C# scripts. We prefer to use Visual Studio, but with any text editor you can build changes made in Godot.
Visual Studio Bigger install, full ide, code completion
Visual Studio Code Smaller, quicker to load a project directory, code completion, has a terminal.
Sublime Text Smaller text editing, this is good for using saved directories and viewing the contents of the games app_data and logs.
Godot Editor Editor Settings > Mono > Editor
`,description:"",tags:null,title:"BasicGame - Script Editing Setup",uri:"/pingod-addons/getting-started/basicgame-script-editing/index.html"},{content:`Quick guide on how to record and playback a game.
Recording a game can speed up your game development for game logic.
They would usually start off by using trough switches then adding credits and starting a game.
Any events after this to test any recorded sequence like clearing a target bank, draining your ball.
Testing modes and testing the complete game.
Recording Recording files are saved in the user://recordings/recordname.record
Example game dir: C:\\Users\\UserName\\AppData\\Roaming\\Godot\\app_userdata\\BasicGameGodot\\recordings
You can get to the app_data directory for the game using app_data bat file
Record Step by Step You can enable a switch window from the inspector options when running to record switches from.
Enable record in the autoload/Machine.tscn godot inspector or direct in file Run the game. Hold down Q,W,E,R (Activate Trough switches) and push start 1 You can release the trough switches Activate all the trough switches again to end the ball. ESC to close the game window A playback filename will be logged to the output window Playback Enable playback in the autoload/Machine.tscn inspector Add the filename in the inspector user://recordings/recordname.record Run the game to playback the recorded events Logs will display when finished Tips / Notes Records could also be loaded from the project directory res:// . Some files are not visible in the FileSystem but can still be accessed. Rename and keep good records to use for automation res://recordings/drain_3_balls.record `,description:"",tags:null,title:"Recording / Playback",uri:"/pingod-addons/plugins/machine/recordings-playback/index.html"},{content:`How to setup localization to have any language in your game.
`,description:"",tags:null,title:"Languages",uri:"/pingod-addons/languages/index.html"},{content:"Godot and Simulator Export (BasicGame) Exporting, Icons ",description:"",tags:null,title:"Exporting / Publishing",uri:"/pingod-addons/exporting/index.html"},{content:`Using PinGodGame and NetProc to run with a P-ROC board.
This version of PinGod is quite similar to the example without a P-ROC, only this uses modes and handles machine items differentley.
Games can be simulated and developed away from the machine with a simulator (visual pinball) or using the playfield window.
Overview Overview of what we get from using NetProc:
PinProc Controller (send data to board USB) Game Controller Modes (with or without CanvasLayers) Databased machine items, adjustments and audits Trough Management (Mode) Ball Search, Ball Save (Modes) P-ROC Links 🔗 multimorphic.com P-ROC hardware-control-system 🔗 P3-ROC (v1) Specifications 🔗 P3-ROC (v2) Specifications 🔗 sw-16 boards 🔗 pd-16 boards 🔗 pd-led boards Newer setup with power module distributing power:
Basic setup of sw-16 board with a single switch:
`,description:"",tags:null,title:"P-ROC - PinGod NetProc",uri:"/pingod-addons/pingodnetproc/index.html"},{content:`Pinball game framework
`,description:"",tags:null,title:"About PinGod",uri:"/pingod-addons/about/index.html"},{content:` What is it? PinGod is for creating pinball game logic with graphics display window.
A pinball machine or a simulator can trigger the game and vice versa.
Why that name? The Godot game engine was used so it’s Pinball + Godot = PinGod.
PinGo or PinGodot? We know who the penguin is and PinGodot doesn’t ring well. This isn’t a holy framework but you can makes games faster like a Pin God so we can rest the case on that decision.
Is it difficult to learn and use? Not really. Godot doesn’t have a steep learning curve and you don’t need to know everything to be able to finish a game or make scenes.
There are full examples in the src with basic to advanced scripted games which will give you an idea of C# sripting tasks and how things were done.
You should be able to lean back on PinGod doing the machine work for you while you compose scenes and do some simple C# scripting.
Advantages? Framework base that covers standard pinball machine features used across games C# scripting (or GScript) Easy to make 2D, 3D graphics scenes Collaborating with others on Scenes / Scripts Localization Recording/Playback of games saves much time in testing More to be added when comes to mind..
Class Reference Documentation Class References - Script Documentation
`,description:"Documentation for PinGod games",tags:null,title:"Home",uri:"/pingod-addons/index.html"},{content:"",description:"",tags:null,title:"Categories",uri:"/pingod-addons/categories/index.html"},{content:"",description:"",tags:null,title:"Tags",uri:"/pingod-addons/tags/index.html"}]