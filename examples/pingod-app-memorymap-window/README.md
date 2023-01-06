# pingod-app-memorymap-window

Template to send switches into memory which can be picked up from a running game. This allows for multi window tooling like switch overlays.

This example requires another game running with MemoryMap enabled to read the switch states sent from this app.

It will generate your buttons if you use your game Machine.tscn.

---

You need just 3 plugins for this to work

- Window actions
- Machine
- MemoryMap

---
## SwitchSenderMemMap.tscn (Main Scene)
---

A simple panel but derived from the old switch overlay I was using and changed the way they are loaded and hook up event handler (OnToggle) to it to then write state to memory map.

---
## How it was setup
---

- Enabling WindowActions, MemoryMap, Machine
- Duplicating the MemoryMap.tscn and putting into `autoload` directory
- Switched the Read Delay to -1 (disable). And `IsEnabled` to enable it.
- Copy the `autoload/Machine.tscn` from basic game and disabled any record/playback options. Just wanted this to show as an example, as you could be using a table with lots of switches.
- Disabled the machine items like Trough, PlungerLane, ballsaver. We don't need them running at all here.