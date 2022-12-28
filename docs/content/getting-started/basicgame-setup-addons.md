---
title: "BasicGame - Linking addons"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 24
---

To run the `BasicGameGodot` project you will need the `addons` directory linked or copied.

These `addons` contain base files and pinball framework that a game will use.

#### Link the PinGod.VP.AddOns

You can copy the `addons` directory to your project (`BasicGameGodot` in this example), but it's better to use a `symbolic link` to the addons so we are not duplicating files and you can keep the same addons across projects.

---

1. Running the `_link_addons.bat` launcher file will symbolic link the `addons`. A shortcut `addons` folder will be added in the Godot project.

2. This will just flash up and disappear, but you will see a new folder shortcut created called `addons`

![image](../../images/basicgame-project-files.jpg)

#### Opening shortcut

If you open the new `addons` shortcut it will show you the files that are in `pingod-addons\addons`.

![image](../../images/basicgame-addons.jpg)


This shortcut can be removed without harming the files where the directory is linked from.