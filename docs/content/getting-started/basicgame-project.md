---
title: "BasicGame - Project.Godot"
date: 2017-10-17T15:26:15Z
lastmod: 2019-10-26T15:26:15Z
draft: false
weight: 24
---

The `project.godot` can be loaded into the Godot editor by using `godot -e`.

![image](../../images/basicgame-project-files-addons.jpg)

{{% panel status="primary" title="Tip" icon="far fa-lightbulb" %}}
Use `Alt+D` in the project directory, type `godot -e` then enter to load project

{{% /panel %}}

{{% panel status="primary" title="Tip" icon="far fa-lightbulb" %}}
You can use the `.bat` files in the projects root directory to load a project with . `_godot_editor.bat` 
{{% /panel %}}

![image](../../images/basicgame-bats.jpg)

| file  | what it does |
| ------------- | ------------- |
| _godot_appdata_dir.bat  | Opens the user directory for the game. |
| _godot_editor.bat  | Opens the user BasicGameGodot project.godot in Godot editor. |
| _godot_export_full.bat  | Exports to the `Windows Desktop` config BasicGameGodot project |
| _godot_export_pck.bat  | Exports to the `Windows Desktop Pack` config BasicGameGodot |
| _link_addons.bat  | Links the addons repository `addons` into BasicGameGodot |
| _vp_edit.bat  | Loads table file `PinGodVp-BasicGame-VPX10.7.vpx` from `BasicGameVisualPinball` into editor. |
| _vp_play.bat  | Loads table file `PinGodVp-BasicGame-VPX10.7.vpx` from `BasicGameVisualPinball` and plays |
| _build_upx_shrink_executable.bat  | Shrinks the export executable |