# pingod-memorymap-win
Windows Memory mapped file to read/write machine states. This class writes coil, lamp and led states to memory. It also read switch states from memory.

It is used with the controller for COM to sync with the simulator over shared memory map. *You only need to enable this plugin when you intend to run it with a simulator like Visual Pinball.*

🔗 [pingod-controller-com](https://github.com/FlippingFlips/pingod-controller-com)

---
### AutoLoad
The `MemoryMap` singleton can be accessed from the root tree under `root/MemoryMap`.

---
### How it works?
The MemoryMapNode will use `CreateMemoryMap` to create a MemoryMapping. If successful then the `MemorySwitchEventHandler` is used from the Node. Any switch events coming in will emit a signal `MemorySwitchSignal`.

Note: *The Machine plugin hooks onto the `MemorySwitchSignal` already if available, `_pinGodMemoryMapNode.MemorySwitchSignal += OnSwitchCommand`.*

---
### Maps and Sizes
Mappings and mutex are created from whoever runs first, sim or pingod. A COM controller will create the mutex then run the game.

Set the amount of each item you have in the VP script which will map out arrays. This side also needs to be the same size.

The default mappings should be enough for most games but you could also shorten the collections as well as increase if need to.

---
### Exports
|Option|Description|
|-|-|
|Is Enabled|Enable/Disable, this allows to have plug-in enabled but disable it here
|Write Delay| Delay for the loop to write to memory, 10 is good to use less, responsive
|Read Delay| Delay for the loop to read from memory, 10 is good to use less cpu, responsive
|Coil Total|If you need to increase coil count, it needs to match the VP script
|Led Total|If you need to increase led count, it needs to match the VP script
|Lamp Total|If you need to increase lamp count, it needs to match the VP script
|Switch Total|If you need to increase switch count, it needs to match the VP script
|MapName| Mapping name shouldn't be changed unless you also change the map name the COM controller uses
|MutexName| Mutex name shouldn't change to match the mutex on the controller
