---
title: "Godot - Installation"
date: 2022-12-26T15:26:15Z
draft: false
weight: 10
---

### Download Godot 4 editor
---

PinGod is using Godot4 Mono (C#). Godot is portable (no installation required) just the download.

On windows the easiest way is to install through `chocolatey` package manager using the command `choco install godot-mono`.

This particular package installs both `x86` and `x64` versions but uses `x64` by default. This becomes an issue when you are wanting to run P-ROC because there's currently no dll for `x64` pinproc. Most users are on the `x86` build of visual pinball and if the game was made specifically for that use in a simulator then it's easier to set a flag when installing from chocalatey with the following in an admin powershell or terminal to force default x86:

`choco install godot-mono --forceX86 --version=4.0.1` = 4.0.3 32bit is usable on x64 systems, 20/8/2023 later versions do not work, subject to change

When download is complete it will extract the 32 bit version. `Extracting 32-bit ....`

Now when you run `godot` anywhere then it will use this 32bit mono version.

#### Setup chocolatey godot console
---

Godot Console debug window was removed by default so it has to be launched with a godot console executable. Make a shim to this from the choco tools directory: `C:\ProgramData\Chocolatey\tools`

```.\shimgen.exe -o ..\bin\godot_console.exe -p ..\lib\godot-mono\tools\Godot_v4.0.3-stable_mono_win32\Godot_v4.0.3-stable_mono_win32_console.exe -i C:\ProgramData\chocolatey\bin\readelf.exe```

### Uninstall
---

It can be uninstalled with `choco uninstall godot-mono`

---
### Dotnet SDK
---

If you already have a `dotnet sdk` then you can skip this step.

- Install x64:

[sdk-6.0.307-windows-x64-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.307-windows-x64-installer)

- or x86:

[sdk-6.0.307-windows-x86-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.307-windows-x86-installer)

---
### Git
---

Use Git to pull the repositories from Github, you don't need a Github account just the software to do this.

- [Git For Windows - Download](https://gitforwindows.org/)

```
Once the installer has started, follow the instructions as provided in the Git Setup wizard screen until the installation is complete.
Open the windows command prompt (or Git Bash).
```

Type `git version` to verify Git was installed.

---
