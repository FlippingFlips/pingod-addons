# pingod-machine

Represents a pinball machine and holds all the machine items.

- This plugin is an AutoLoad singleton which can be accessed in game from `/root/Machine`.
- Hooks onto the memory map plugin if found
- Handles Ball searching

---
## How to use
---
- Create an empty node scene in `autoload` folder and use the `PinGodMachine` base script
- You will see after adding the script that you can add your machine items to the machine in the Godot inspector

---
## Tools
---
There are added (godot) tools also here.

---
### Bumper
---

- Create new scene node and find the bumper in the editor
- Name your bumper to whatever you like
- Assign Switch number
- Assign Coil number
- Assign sound to played when activated

#### Events

BumperHitEventHandler = hook up to this event in your script to act upon when the bumper is hit



---
TODO: link this to the documentation
