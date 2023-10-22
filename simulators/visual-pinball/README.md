# pingodaddons-demo - Visual Pinball
A [Visual Pinball 10.8](https://github.com/vpinball/vpinball) demo table and script that can launch a games directory containing a `project.godot` or by an exported executable.

*Note: 'Windows Only'. This table requires a COM object installed on the system for the tables script to interop with the game.*

This demo works with the default `PinGod.vbs` machine configuration shipped with the controller which covers the cabinet switches, flippers.


* [pingodaddons-demo.vbs](pingodaddons-demo.vbs) - Table Script
* [pingodaddons-demo.vpx](pingodaddons-demo.vpx) - Table File

---
## How To Install / Run?
You need visual pinball and the `PinGod.VP.Controller` COM object install to run this demo.
1. Download a release of [Visual Pinball](https://github.com/vpinball/vpinball/releases)
2. Download and run the [pingod-controller-com](https://github.com/FlippingFlips/pingod-controller-com) setup from releases to install the COM controller `.dll`
3. Copy the scripts from the setup to your VP Scripts directory
4. Open the table in visual pinball and set the `GameDirectory` in the script and launch the game.
5. Launching the game will load the `project.godot` alongside visual pinball. Make sure that you have the `MemoryMap` enabled in the game project.

---
## Game Directory
The default `GameDirectory` in the script is set to a relative path `../../`. That can load the project from this repository if you load the table how it is here. 

The table's script is provided with the vpx table file, [pingodaddons-demo.vbs](pingodaddons-demo.vbs), the table will use this if next to it with the same name so it's easier to edit `GameDirectory` if you need to outside of VP.

---
## Display options
See the script section `With Controller` for setting custom display properties to override the project default. See also [pingod-controller script -> override-display-settings](https://github.com/FlippingFlips/pingod-controller-com/tree/dev#override-display-settings)

---
## Hints
See the `pingod-controller` read me for more in depth how the script works and the tables script here is documented well enough to get started.

