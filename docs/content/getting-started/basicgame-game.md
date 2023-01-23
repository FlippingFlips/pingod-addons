---
title: "BasicGame - Game"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 30
---

### Game Window Switches

Switches need to be added to `_gameWindowSwitches` in `PinGodGame.tscn`.

---
| Keys  | action |
| ------------- | ------------- |
| 5  | Credits |
| Q,W,E,R | Trough Switches 1-4 |
| 1  | Start Game |
| T  | Plunger Lane |
| Y  | inlane_l |

---
### Simulating with keypress actions
---

Add credits (5), hold all the trough switches down (QWER) then push start (1).

![image](../../images/basicgame-controlgame.jpg)

- Add some points with `Y` = switch 22  `inlane_l`

- Simulate ball drain by activating all the trough switches.

- Use `T` plunger lane switch and drain to activate a ball save
