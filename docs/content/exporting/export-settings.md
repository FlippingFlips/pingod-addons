---
title: "Export Settings - Godot"
date: 2022-12-26T15:26:15Z
draft: false
weight: 43
---

The BasicGame project below shows that the first project is `Windows Desktop Runnable` and is exporting to `..Build/PinGod.BasicGame.exe`

This window can be found in Godot `Project > Export`. 

Export option "embed pck" does not work on 32-bit x86 [GodotIssues](https://github.com/godotengine/godot/issues/35830)

![image](../../images/basicgame-project-export-exe.jpg) - Feb 2020

{{% panel status="danger" title="Godot Naming" icon="far fa-lightbulb" %}}
The names here have to be consistent which each other. The exe and the pack.
{{% /panel %}}

The BasicGame project for `Windows Desktop Pack` shows that it's exporting to `..Build/PinGod.BasicGame.pck`

![image](../../images/basicgame-project-export-pck.jpg)

---
## Excluding Assets
---

Added this exclude filter on the Runnable and the Desktop Pack to not add any files in `assets/*`.

![image](../../images/basicgame-project-export-assets.jpg)

This asset folder is usable in the `BasicGameGodot` game directly but as the directory is excluded here for exporting from then we can build an asset pack from another project.

See 
