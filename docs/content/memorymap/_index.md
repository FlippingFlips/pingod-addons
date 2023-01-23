---
title: "Plugin - MemoryMap"
date: 2022-12-26T15:26:15Z
draft: false
weight: 15
---

This class writes coil, lamp and led states to memory. It also read switch states from memory.

It is used in conjunction with the controller for COM to sync with the simulator.

*You only need to enable this when you intend to run it with a simulator.*

---

The image below shows your options.

|Option|Description|
|-|-|
|Is Enabled|Enable/Disable, this allows to have plug-in enabled but disable it here
|Write Delay| Delay for the loop to write to memory, 10 is a good default, less cpu, responsive
|Read Delay| Delay for the loop to read from memory, 10 is a good default, less cpu, responsive
|MapName| should be left, the controller uses this name
|Coil Total|If you need to increase coil count, it needs to match the VP script
|Led Total|If you need to increase led count, it needs to match the VP script
|Lamp Total|If you need to increase lamp count, it needs to match the VP script
|Switch Total|If you need to increase switch count, it needs to match the VP script
|MutexName| should be left, the controller uses this name

![image](../images/plugin-memorymap.jpg)

