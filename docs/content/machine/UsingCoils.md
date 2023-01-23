---
title: "Using Coils (C#)"
date: 2022-12-26T15:26:15Z
draft: false
weight: 50
---

Coils can be On, Off or Pulsed. Get a reference to `/root/Machine` from a scene.

```
var pingodMachine = GetNode<MachineNode>("/root/Machine");
pingodMachine.CoilPulse("trough", 225);
```
