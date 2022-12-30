---
title: "Using Coils (C#)"
date: 2022-12-26T15:26:15Z
draft: false
weight: 50
---

Coils can be On, Off or Pulsed. Get a reference to `pinGod` and use methods with name.

```
pinGod.SolenoidPulse("trough", 225);

pinGod.SolenoidOn("trough", 1);

pinGod.SolenoidOn("trough", 0);

pinGod.SolenoidPulse("auto_plunger")// Pulse the auto plunger

pinGod.SolenoidOn("flippers", 1) // enable the flippers
```