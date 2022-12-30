---
title: "Logging Games"
date: 2022-12-26T15:26:15Z
draft: false
weight: 25
---

[Game logging Class Reference](/pingod-addons/html/interfaceIPinballLogger.html)

---
## Log location
---

In the appdata directory under the game name, logs `.log` are created. Use the `open_app_data.bat` to open directory to open the data directory.

![image](../../images/basicgame-userdata-sublime.jpg)

The image above has the users game directory open with Sublime Text.

---
SublimeText - Game Data Folder
---

The steps to add this directory and save project:

1. Find the games appdata folder with bat file `_godot_appdata_dir.bat`
2. Copy the path and use `Open Folder` in Sublime and open the user path
3. Save this as a project in sublime then reopen it later for a quicker way to view logs, look at your settings etc for this game.

You could do similar with a Visual Studio code workspace or something else, this is just one example of getting back to viewing this directory.


---
Log example:
---

```
Godot Engine v3.4.2.stable.mono.official.45eaa2daf - https://godotengine.org
OpenGL ES 3.0 Renderer: Radeon RX 570 Series
OpenGL ES Batching: ON
 
PinGodGame:_EnterTree. getting cmd args
PinGodGamecmd line available: 0
GameSettings:loaded:Ok
PinGodGame:_EnterTree. log level: Debug
PinGodGame:window: size:766x478 pos:403,221, onTop: True
PinGodGame:window: project settings size: 1024x600
PinGodGame:looking for game patches. res://patch/patch_{patchNum}.pck . From 1. patch_1.pck, patch_2.pck
```

---
## Script Usage
---

Use from a pinGod reference or the static Logger in script like Logger.Debug

```
void Debug(params object[] what);

void Info(params object[] what);
	
void Warning(string message = null, params object[] what);

void Error(string message = null, params object[] what);

PinGodLogLevel LogLevel { get; }
```