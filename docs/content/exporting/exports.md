---
title: "Exporting"
date: 2022-12-26T15:26:15Z
draft: false
weight: 40
---

This would be your final step but this isn't necessary for developing / debugging games. These exports can be used without Godot.

See the `export_presets.cfg` file for icons, names, exporting.

---
## Before Exporting Checklists

### Project settings
---

- Are the `Recordings / Playback / Overlays` switched off?

### Simulator Script
---

- VP Script change to `Debug=False`
- VP Script change path to `./GameExecutable` - If game is packaged with table in the same directory the player only has to run the game in Visual Pinball

*Helps to have a copy of the release table in the export path to test exports and keeping your debug table*

## [BasicGame - Simulator Export](../../getting-started/basicgame-game-exporting/#simulator-export)

---
## Exporting
---


### Command line

- Use the bat files export for steps. See [Godot Export](../../getting-started/basicgame-game-exporting/#godot-export)
- You only need to build the full export once. The executable will always be the same size, but the pack file will be different.
- Shrink the executable 40 > 10mb with the `_build_upx_shrink_executable.bat` in the Build folder. You will need [UPX Ultimate Packer for eXecutables](https://upx.github.io/).

## Changing Windows Icons

- Make icon with all size in one file
- Download [RCEdit (Github)](https://github.com/electron/rcedit/releases) and add the path in Godots `Editor Settings\RCedit`
- Change icon in the Godots `Project Settings/Application/Config`

See https://docs.godotengine.org/en/3.2/getting_started/workflow/export/changing_application_icon_for_windows.html
