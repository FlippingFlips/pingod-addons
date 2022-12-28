---
title: "BasicGame - Exporting"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 40
---

You can export the game to be used without the Godot editor. The Build folder will be used to export to.

## Godot Export

| file  | does? |
| ------------- | ------------- |
| _godot_export_full.bat  | Exports `Windows Desktop` which is executable and main pack |
| _godot_export_pck.bat  | Exports `Windows Desktop Pack` which updates the main pack. The executable will be the same for all games as it basically is Godot |

You only need to run `_godot_export_full.bat` one time to get the executable, this will also build a `.pck`, when you make changes you can run `_godot_export_pck.bat` to update the `.pck` export.

The output directory (`../Build`) for these exports and other settings are in the `export_presets.cfg`.

Shrink the executable 40 > 10mb with the `_build_upx_shrink_executable.bat` in the Build folder. You will need [UPX Ultimate Packer for eXecutables](https://upx.github.io/).

## Simulator Export

Preparing a table is straight forward enough by changing the `IsDebug` in the script and specifying the executable name in the `GameDirectory`.

![image](../../images/basicgame-vp-script-export.jpg)

The script example above presumes the table is being run alongside this executable.
