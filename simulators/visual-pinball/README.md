# pingodaddons-demo - Visual Pinball
A [Visual Pinball 10.8](https://github.com/vpinball/vpinball) demo table and script that can launch a project directory containing a `project.godot`
or by an exported executable.

*Note: 'Windows Only'. This table requires a COM object installed on the system for the tables script to interop with the game.*

*Note: If running desktop with the display on top of visual pinball then this version `VPinballX_GL64.exe` won't stay on top.
If using 2 displays this isn't an issue but works with normal VP 10*

* [pingodaddons-demo.vbs](pingodaddons-demo.vbs) - Table Script
* [pingodaddons-demo.vpx](pingodaddons-demo.vpx) - Table File

---
## How To Install / Run?
1. Download a release of 🗄[Visual Pinball](https://github.com/vpinball/vpinball/releases)🗄.
`C:\Visual Pinball` is a good place to put this install, along with tables scripts.
2. Download and install the 🗄[pingod-controller-com](https://github.com/FlippingFlips/pingod-controller-com)🗄.
Use setup installer to install or uninstall.
This is a windows memorymap to interop when machine items change in the game.
3. Copy the scripts from the setup to your VP Scripts directory.
Copy the `_scripts/vp` contents into `VP/Scripts` for updated machine config
4. Open the table in visual pinball from where you extracted the demo to.
This demo is set to run the `win.exe` two directories up when you launch the table.
## Developing
1. Change the `GameDirectory` in the script and the `Debug` flag
It doesn't have to be full path, it can point to where the `project.godot` file is to load it.
## Game Directory
The default `GameDirectory` in the script is set to a relative path `../../`.
## Game Script
The table's script is provided with the vpx table file, [pingodaddons-demo.vbs](pingodaddons-demo.vbs).
There are other scripts that can be copied from `_scripts`.
This script is used when the game is launched from the text file in the editor and game

- nopingod = is just a table with script that is flippable
- pingod = launches a godot project with the table
- release = launches an executable export. (mainly for source control to copy)

---
## Display options
See the script section `With Controller` for setting custom display properties to override the project default. See also [pingod-controller script -> override-display-settings](https://github.com/FlippingFlips/pingod-controller-com/tree/dev#override-display-settings)

---
## Hints
See the `pingod-controller` read me for more in depth how the script works and the tables script here is documented well enough to get started.

