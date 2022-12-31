---
title: "PinGodGame Projects - Create Template"
date: 2022-12-26T15:26:15Z
draft: false
weight: 10
---
---

## Empty Project
---
Steps below to create new empty project
1. Create new project folder, add empty text file named `project.godot`
2. Symbolic link the `addons` to the project so you can reuse it, remove it etc.
---
## Symbolic Link
---
If we're in `pingod\PinGodDev` and `pingod\pingod-addons` is a directory above the current then we can link the addons

```
SET cDir=%cd%
cd ../pingod-addons/addons
SET addDir=%cd%
cd %cDir%
mklink /D addons "%addDir%"
```
---
## Open project in Godot
---
3. Open the project in Godot (godot -e) and you'll see the directory which you have linked
4. Playing the game will ask you to define a scene to load

---
## Add Scene
---
1. Add new Node2D
2. Rename the node and save it as `MyScene`
3. Save in the root directory and not the `addons` directory
4. The new scene should be at `res://MyScene.tscn`

![image](../../images/projects-using-pingod/project-add-empty-scene.jpg)

Here is the project after adding `MyScene.tscn` with the `addons` linked

![image](../../images/projects-using-pingod/project-added-myscene.jpg)

---
## Run project
---
Hit play in Godot and run project. Select this scene as the current scene to load.
