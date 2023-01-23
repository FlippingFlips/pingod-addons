---
title: "PinGodGame Projects - New From Template"
date: 2022-12-26T15:26:15Z
draft: false
weight: 11
---
---
## Duplicate Template
---

1. Copy the `pingod-projects-template` and paste in same directory, by default windows adds `- Copy` to the directory
2. Rename the copy to `pingod-projects-myproject`
3. You will need to link the `addons` again. Delete addons directory then run the `_link_addons.bat`
---
## project.godot file
---

You can add a project name to the `project.godot` file before running the project. These are found under `[application]` in the `config` section. Yours will be empty. 

The reason why we've added `Projects-MyProject` here is that it's easier to tell the type in the `appdata`, because this is the name it will be saved under in the users app data. By default though logs are put into the project directory.

```
[application]

config/name="Projects-MyProject"
config/description="Test project for scenes etc..."

run/main_scene="res://MyScene.tscn"
```
---
## Godot
---
1. Load project in Godot with `godot -e`
2. The project should play. `F5`, `ESC` to exit

Instance a scene from the addons. `SwitchOverlay.tscn`

1. Right click in the MyScene node and `Instance Child Scene`
2. Browse to the `addons\Scenes\` and select `SwitchOverlay.tscn`

Play the project, although it will play there will be an error, because the script is using C# and it doesn't have an assembly for the project.
