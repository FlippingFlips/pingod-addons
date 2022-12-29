---
title: "Logging Games"
date: 2022-12-26T15:26:15Z
draft: false
weight: 25
---

---
## Log location
---

In the appdata directory under the game name, logs `.log` are created. Use the `open_app_data.bat` to open directory to open the data directory.

![image](../../images/basicgame-userdata-sublime.jpg)

The image has the users game directory open with Sublime Text.

The steps to add this directory and save project:

1. Find the games appdata folder with bat file `_godot_appdata_dir.bat`
2. Copy the path and use `Open Folder` in Sublime and open the user path
3. Save this as a project in sublime then reopen it later for a quicker way to view logs, look at your settings etc for this game.

You could do similar with a Visual Studio code workspace or something else, this is just one example of getting back to viewing this directory.

---
## Script Usage
---

Use from a pinGod reference or the static Logger in script

```
void LogDebug(params object[] what);

void LogInfo(params object[] what);
	
void LogWarning(string message = null, params object[] what);

void LogError(string message = null, params object[] what);

PinGodLogLevel LogLevel { get; }
```

Game logging

[Class Reference](/pingod-addons/html/interfaceIPinballLogger.html)
