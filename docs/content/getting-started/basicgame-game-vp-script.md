---
title: "BasicGame - Visual Pinball Script"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 31
---

#### BasicGame-PinGod.vbs

A script with the same name is included with the table. This will be used with the visual pinball script editor.

This can make it easier to edit outside of table, keep track of changes etc. At the top of this image it shows MODIFYING EXTERNAL.

### PinGod.VP.Controller

The first section marked in red is the `PinGod.VP.Controller` initialization.

![image](../../images/basicgame-vp-script-a.jpg)

### Game Project Location
---

When debugging, if the games Godot project is relative to the table (the same as this repo), then you won't have to adjust the script for the `GameDirectory`.

But if you need to, you can change the default `Const GameDirectory = "..\BasicGameGodot"` to:

`Const GameDirectory = "C:\Yourfilepath\BasicGameGodot"`

![image](../../images/basicgame-vp-script-b.jpg)

| Variable  | does what? |
| ------------- | ------------- |
| IsDebug  | Set controller in debug launch project directories |
| GameDirectory  | Game Project to launch, see above |
| UseSolenoids  | Set controller to check for states |
| UsePdbLeds  | PROC led states. This project is set to leds for the ball save light |
| UseLamps  | Use lamps or leds, or both |

Variables declared for table objects. `Dim bsTrough, bsSaucer`

### Table Events

Getting the controller to act on some table events. Stopping the game window and pause / resume

![image](../../images/basicgame-vp-script-c.jpg)

### Table Init
---

Visual pinball `Table1_Init` is a little different here. This is where the controller is `Run`, how it is run depends on the `IsDebug` flag set.

A timer interval is added to the `LeftFlipper` which checks to see if the `Controller.GameRunning` is enabled. When it is enabled then `InitGame` is invoked.

![image](../../images/basicgame-vp-script-d.jpg)

You can set more on the Controller like these [display settings](#display-settings)

### Init game
---

This sub routine is much like a PinMame game setup and we use `core_c_sharp.vbs` which is pretty much the `core.vbs`.

- `vpmMapLights AllLamps` This runs updated lamps from this collection. The lamp number is in the VP Light objects `TimerInterval`
- `vpmCreateEvents AllSwitches` This runs updated switches from this collection. The switch number is set inside the VP Triggers `TimerInterval` variable
- PinMame timers enabled
- Create Ball Trough set to `bsTrough`
- Create Auto Plunger set to `plungerIM`
- Create Kicker set to `bsSaucer`
- Set vpm nudging

![image](../../images/basicgame-vp-script-e.jpg)

### Keyboard
---

Both `Table1_Key` routines use a check `if Controller.GameRunning = 0 then Exit Sub 'exit because no display is available`

### Solenoids Callbacks
---

| Callback  | does what? |
| ------------- | ------------- |
| PinGodAlive  | if you need to know if the pingod window is alive |
| bsTrough.solOut  | Event for the trough we created. Kick the ball |
| bsSaucer.solOut  | Event for the kicker on the table |
| FlippersEnabled  | enable/disble flippers |
| AutoPlunger  | Invokes PlungerIM.AutoFire|
| Lampshow1  | Runs a light sequence |
| DisableLampShows  | StopPlay light sequence |

![image](../../images/basicgame-vp-script-f.jpg)

#### Display settings
---

Display can be adjusted by using `F2` on the window, move and resize and save the position.

```
.DisplayX = 1920 - 512
.DisplayY = 10
.DisplayWidth = 512 ' 1024 FS
.DisplayHeight = 300 ' 600  FS
.DisplayAlwaysOnTop = True
.DisplayFullScreen = False 'Providing the position is on another display it should fullscreen to window
.DisplayLowDpi 	= False
.DisplayNoWindow = False
```	

### Default Machine Switches
---

These are used by Visual Pinball common switches which are mapped to keyboard. From `PinGod.VP/scripts/PinGod.vbs`

```
Cabinet switches
Const swCoin1  = 0
Const swCoin2  = 1
Const swCoin3  = 2
Const swCoin4  = 3
Const swCancel = 4
Const swDown   = 5
Const swUp     = 6
Const swEnter  = 7
Const swCoinDoor  = 8
Const swLLFlip = 9
Const swLRFlip = 11
Const swULFlip = 13
Const swURFlip = 15
Const swSlamTilt = 16
Const swTilt = 17
Const swStartButton = 19
```

### Script Notes / Guide
---

- Switch events can be sent with `Controller.Switch 20, 1`. Usually this is `Controller.Switch(20) = 1`.