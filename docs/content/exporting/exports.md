---
title: "Exporting"
date: 2022-12-26T15:26:15Z
draft: false
weight: 40
---

Also See [BasicGame - Simulator Export](../../getting-started/basicgame-game-exporting/#simulator-export) 

---
## Exporting Game
---

### Command line
---

- Use the bat files export for steps. See [Godot Export](../../getting-started/basicgame-game-exporting/#godot-export)
- You only need to build the full export once. The executable will always be the same size, but the pack file will be different.

![image](../../images/basicgame-exported-game.jpg)

---
### Pack executable size
---

- Shrink the executable 40 > 10mb with the `_build_upx_shrink_executable.bat` in the Build folder. 

- You will need [UPX Ultimate Packer for eXecutables](https://upx.github.io/) 

- UPX added to environment path to run the bat file.

---
### Export Settings
---

The BasicGame project shows that the first project is runnable and is exporting to `..Build/PinGod.BasicGame.exe`

This can be found in `Project > Export`. These settings can also be changed and added to in `export_presets.cfg`

Export option "embed pck" does not work on 32-bit x86 [GodotIssues](https://github.com/godotengine/godot/issues/35830)

![image](../../images/basicgame-project-export-exe.jpg) - Feb 2020

{{% panel status="danger" title="Godot Naming" icon="far fa-lightbulb" %}}
The names here have to be consistent which each other. The exe and the pack.
{{% /panel %}}

The BasicGame project for `Windows Desktop Pack` shows that it's exporting to `..Build/PinGod.BasicGame.pck`

![image](../../images/basicgame-project-export-pck.jpg)

See the `export_presets.cfg` file for icons, names, exported saves.

### Changing Windows Executable Icon

- Make icon with all size in one file
- Download [RCEdit (Github)](https://github.com/electron/rcedit/releases) and add the path in Godots `Editor Settings\RCedit`
- Change icon in the Godots `Project Settings/Application/Config`

![image](../../images/exports-icons-rcedit.jpg)

See https://docs.godotengine.org/en/3.2/getting_started/workflow/export/changing_application_icon_for_windows.html

---

## Before Exporting Checklists

- Are the `Recordings / Playback / Overlays` switched off?

### Simulator Script
---

- VP Script change to `Debug=False`
- VP Script change path to `./GameExecutable` - If game is packaged with table in the same directory the player only has to run the game in Visual Pinball

*Helps to have a copy of the release table in the export path to test exports and keeping your debug table*
