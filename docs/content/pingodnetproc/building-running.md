---
title: "Building / Debug"
date: 2022-12-26T15:26:15Z
draft: false
weight: 6
---

---
### Building Project
---

You should be able to build the project with `dotnet build` providing you have linked or added the `addons` directory.

It can be built inside Godot 4 by running the project editor `godot -e`.

---
### Running
---

You can run from command line with `godot` no debug

---
### Debugging
---

Visual Studio is very good here for debugging. Use the `Properties/launchSettings.json` to adjust launch profiles.

|Option|Description|
|---|---|---|
|Godot4 (Debug with Console)|Uses the godot_console.exe. doesn't debug with breakpoints but gives you a console window|
|Godot4 (Verbose with Console)|Uses the godot_console.exe. doesn't debug with breakpoints but gives you verbose a console output|
|Godot (Debug)|Uses the godot.exe and can hit breakpoints in visual studio|
|Godot (Editor)|Launches the Godot Editor. Usually run without debug to just launch the godot project faster|
|Export Release (Windows Desktop)|Runs an export|