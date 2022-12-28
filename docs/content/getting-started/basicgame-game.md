---
title: "BasicGame - Game"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 30
---

### Game default controls
---
| Keys  | action |
| ------------- | ------------- |
| 5  | Credits |
| Q,W,E,R | Trough Switches 1-4 |
| 1  | Start Game |

---

### Simulating with keypress actions

Add credits (5), hold all the trough switches down (QWER) then push start (1).

![image](../../images/basicgame-controlgame.jpg)

- Add some points with `Y` = switch 22  `inlane_l`

- Simulate ball drain by activating all the trough switches.

- Use `T` plunger lane switch and drain to activate a ball save

### Dev overlays

In `PinGodGame.tscn` scene unspector switch on overlay enabled.

The following image displays controlling trough switches in green.

![image](../../images/basicgame-overlays.jpg)

### Game Logs

`_godot_appdata_dir.bat`

Opens the `app_userdata` for the project. Logs go here, `gamedata.save`, `settings.save` and `recordings`
