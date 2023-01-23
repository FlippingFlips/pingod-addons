---
title: "Getting Started"
date: 2022-12-26T15:26:15Z
draft: false
weight: 5
---

We'll use the template project we built for PinGod P-ROC for this guide / overview. `examples/pingod-netproc-pdb`

We will focus on the P3-ROC / PDB board. We do intend to add the same for the original P-ROC board, it's just untested.

This uses a `IFakeProcDevice`, but that can be swapped with a single `simulated` flag when the game is created in the `ProcPinGodGame.cs`.

---

You will want more or less the setup from the beginning guide.

- pingod-addons
- dotnet sdk 6+
- Godot 4
- Visual Studio (Debugging / Scripting)

---
## pingod-netproc-pdb
---

🔗 Examples Project: [PinGod-AddOns--pingod-netproc-pdb](https://github.com/FlippingFlips/pingod-addons/examples/pingod-netproc-pdb)

This is a similar game setup to a PinGodGame but with added scripts and references to use and work with `NetProc`.

---
### NetProc library - Project References
---

The `csproj` in this directory has references to `NetProc` which are installed from the `nuget` packages folder. 

The folder for the packages is set in the `nuget.config`. You won't need to worry about this unless you update the `NetProc`, which is just a case of adding packages to the `nuget` folder and updating to a newer version in the `csproj`.

Note: *When running a real board you need to build as `x86`. If the game is not set to simulated then it will load the pinproc library*
