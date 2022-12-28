---
title: "Godot - Environment Configuration"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 20
---

### Set Environment path to Godot
---

You should rename the Godot executable to `godot` and add to Environment paths on your system for access ease and scripts to use.

After you've done this you can run `godot` from the environment, command line. 

This is helpful to load projects with the `godot` command and using the .bat files provided requires this name.

![image](../../images/godot_cmd.jpg)

---

### How to add to environment (Windows)
---

#### Quick Way (Option 1)
---

1. Open a cmd.exe window. `Ctrl + R` for a run box, then type `cmd` enter

2. On command line anywhere run set Path, change the directory from `C:\your\path\here\` to your Godot path: 

`set PATH=%PATH%;C:\your\path\here\`

3. You won't see anything happen here but you should be able to run `godot` in this same command window as it is now in your system paths

---


#### Manual Way (Option 2)
---

1. Push windows key and type path, which will filter to `Edit the System Variables`
2. Push enter to open the `System Properties` window, from here select `Environment Variables`
3. In the `Environment Variables` - `System Variables (bottom window pane)` find variable named `Path` and open this
4. Add in the godot path in this section, OK out of the windows then test the command line can run the renamed file `godot`