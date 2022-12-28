---
title: "Godot - Environment Configuration"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 20
---

### Set Environment path to Godot
---

![image](../../images/godot_cmd.jpg)

---
### How to add to environment (Windows)
---

After any one of these steps you choose is completed then you can load Godot from anywhere using `godot`. Powershell (Option 2) is the simplest.

---
#### Windows UI - (Option 1)
---

1. Push windows key and type `path`, this will filter to `Edit the System Variables`
2. Push enter to open the `System Properties` window, from here select `Environment Variables`
3. In the `Environment Variables` - `System Variables (bottom window pane)` find variable named `Path` and open this
4. Add in the godot path in this section, OK out of the windows

---
#### Powershell (Option 2)
---

1. Use keypress `Win+X` to bring up menu

2. Select `Windows Powershell Admin`

3. Change the path here and run `setx path "%PATH%;C:\GodotBinaries" /m`

![image](../../images/godot-env-powershell.jpg)

---
#### Windows Command Prompt (Option 3)
---

1. Run a command prompt as an administrator. Windows Key, search "cmd", right click as admin

2. Change the path here and run `setx path "%PATH%;C:\GodotBinaries" /m`

![image](../../images/cmd-admin.jpg)

---