---
title: "Godot - Installation"
date: 2022-12-26T15:26:15Z
draft: false
weight: 10
---

### Download Godot 4 editor
---

PinGod will be using Godot4 beta. Beta 14, is current as of writing this document.

Godot4 plays better with C# and uses dotnet 6.0. After doing some testing this is the version we prefer.


* Download the beta editor at https://downloads.tuxfamily.org/godotengine/4.0//
* Godot is portable (no installation required)
* It's up to you where you extract the files to but for simplicity sake `C:\Godot\`

You should rename the Godot executable to `godot.exe` and add to the environment.

After you've added to environment then you can just run `godot` from the environment or from debugger.

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
