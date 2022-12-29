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

## Before Exporting Checklists

- Are the `Recordings / Playback / Overlays` switched off?

### Simulator Script
---

- VP Script change to `Debug=False`
- VP Script change path to `./GameExecutable` - If game is packaged with table in the same directory the player only has to run the game in Visual Pinball

*Helps to have a copy of the release table in the export path to test exports and keeping your debug table*
