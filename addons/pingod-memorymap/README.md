# pingod-memorymap

Windows Memory mapped file to read/write machine states. Used by the PinGod.VP controller to read/write

- This plugin is an AutoLoad singleton which can be accessed in game from `/root/MemoryMap`.
- Sends a `SwitchCommand` signal when memory mapping state changes

---
## How to use
---
- Create an empty node scene in the `autoload` folder named `MemoryMap` and use the `PinGodMemoryMapNode` base script
- Enable the plugin which enables / disables auto loading

