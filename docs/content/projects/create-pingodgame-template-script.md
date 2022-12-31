---
title: "PinGodGame Projects - Add a C# Script"
date: 2022-12-26T15:26:15Z
draft: false
weight: 12
---
---
## Create Script for MyScene
---

This template project will not give any errors if you don't use a C# script but if using an existing scene that uses a script then we will need to add a C# project to the directory.

As we've referenced a C# script from the `SwitchOverlay.tscn` then we need to make a C# project.

Fortunately that is easy as we just have to make a new script to do this.

1. Selecting the `MyScene` node and in the inspector bottom right of the pane is a script section
2. Add new script here selecting C#

![image](../../images/projects-using-pingod/project-new-script.jpg)

3. When you click create, a project will be created and open the script in a Godot window

![image](../../images/projects-using-pingod/project-new-script-open.jpg)

---
### New files
---

- Projects-MyProject.csproj
- Projects-MyProject.sln
- MyScene.cs

---
### External editors
---

With the project open in Godot go to:

`Editor > Editor Settings > Mono > External Editor`

---
#### Visual Studio Code
---

Visual Code is the fastest to open the C# files, if you select from the options and double click `MyScript.cs` that will open for editing in `VsCode`.

---
#### Visual Studio
---
[Godot CSharp](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html#visual-studio-windows-only)
---
##### Debugging
---

Add the following to the `.csproject` to launch projects from there but can't hit breakpoints.

<PropertyGroup>
    <StartAction>Program</StartAction>
    <StartProgram>C:\Godot\Godot.exe</StartProgram>
    <StartArguments>--path C:\Path\To\Godot\Project\</StartArguments>
</PropertyGroup>

---
##### Debugging Extension
---

Difficult to get working...

An Visual Studio extension can be used for debugging Godot. Download and run the `.vsix`. 

[godot-csharp-visualstudio/releases/tag/v2.0.0](https://github.com/godotengine/godot-csharp-visualstudio/releases/tag/v2.0.0)

---
##### Godot 4 .
---

Godot 4.x changes to use .NET [Core] Runtime rather than Mono, so you can attach to a running Godot game with the VS debugger,

https://downloads.tuxfamily.org/godotengine/4.0/beta1/mono/


