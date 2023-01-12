---
title: "Create New Project"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 11
---

In the examples directory is a folder named `!template`. This is bare bones minimum. If on windows just copy and paste this folder in the examples and rename it to your name.

---
## What's in the folder?
---
### Properties
---
This contains a `launchSettings.json` for launching the project inside Visual Studio. These are launch profiles and you can add your own in this file.

---
### _link_addons.bat
---
This file is explained in previous here but it will link the `addons` folder into the game so you are not duplicating files, just reusing the addons.

---
### project.godot
---
This is the Godot project for the game. It uses the `.csproj` and `.sln` to build mono c#.

You can edit the `project.godot` before launching it with Godot and change the name in there to save time under `config/name`.

Under the [dotnet] section you can change the assembly name from PinGod to your name. If you do this just delete the `PinGod.csproj` and `PinGod.sln` files then when you run it in Godot create a new C# which will create the solution and project.

---
## Loading project first time
---

1. Run the addons link so you have the `addons` enabled and have a shortcut to it.
2. Run project with godot with the editor flag `-e`

After loading you should see the addons directory. If you go to `Project Settings > Input Map` you can see some inputs to control the window.

![image](../../images/godot4-newproject/first-launch-inputmapping.jpg)

---
## Playing first time
---

If you try to play the project then it will ask for a scene. Just create any old scene name it `MainScene` and set that as the start up. You can right click any scene to set as startup or run a scene with F6.

Play the project and you should be greeted with a grey screen window, push ESC to close but doesn't close. You need to wire ESC to close the window or we can use the plugin `WindowActions`.

---
## Enabling a plugin
---

Go to `Project Settings > Plugins` and you will see a list of plugins that have been loaded from the `addons` folder.

Enable the WindowActions. What this will do is add an Autoload entry and autoload when a game is run. Even if you are running a scene on it's own, these autoload also load there.

---
You maybe greeted with an error here, this could be because you're missing a `csproj` and `solution`.

To fix this, create a new C# script in the root inside godot, this will trigger it to create those files.

A build button should now be available (top right of Godot), build the project then enable the `WindowActions`.

---

![image](../../images/godot4-newproject/enable-plugin-windowactions.jpg)

---
## Using the WindowActions plugin
---

Load the game again and use ESC, which should now close the game. F2 will also be available to toggle the window border

---
## Enabling all plugins
---

Enable all plugins and you should be greeted with a similar log to this.

![image](../../images/godot4-newproject/enable-all-plugins.jpg)

---
## Using the addons default startup - MainScene
---

Duplicate the `MainScene.tscn` into the root and set that to run first scene.

In this scene is an Attract mode, Pause and settings setup and can run the default game scene.

![image](../../images/godot4-newproject/mainscene-default.jpg)

---

If you run the scene with all plug-ins enabled you will be able to add credits and play a complete game, that is, start it, drain balls, show bonus until a game over.

To troubleshoot, push F1 and change the logging level to verbose or debug in the PinGod tab.

You can reset the game with F3, move the window by toggling the border with F2.


---
## Enable a Switch window
---
A switch window can send switches to the machine which is much faster than using key presses.

To enable this go to your duplicated `autoload/Machine.tscn` and in the inspector `Switch Window`.

Enable it and `QuickLoad` the window scene by filtering for playfield. Select `PlayfieldWindow`.

Run the game and you will have a window to push switches. To customize see the [Playfield Switch Window](../playfield-switch-window)

![image](../../images/godot4-newproject/machine-inspector.jpg)