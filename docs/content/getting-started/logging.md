---
title: "Logging Games"
date: 2022-12-26T15:26:15Z
draft: false
weight: 25
---

---
## Log location
---

In the appdata directory under the game name under logs `.log` are created. Use the `open_app_data.bat` to open directory to open the data directory.

---
## Usage
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
