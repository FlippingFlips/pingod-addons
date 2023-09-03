---
title: "Using Switches (Controller)"
date: 2022-12-26T15:26:15Z
draft: false
weight: 40
---

### Auto Switch handlers (No scripting)
---

In the BasicGame, Visual Pinball table switches are added to a simulator collection named `AllSwitches`.

Add the switch number to the `TimerInterval` box in the Misc settings to 20 and add the switch to the `AllSwitches` collection.

* All VP table objects should be covered, so you wouldn't have to remember to use `Spin` instead of `Hit` for a spinner in the collection.

![image](../../../images/vp-auto-switches-1.jpg)

### (Scripting) Visual Pinball - Switch On / Off

```
Sub sw_plunger_lane_hit() : Controller.Switch 20, 1 :  End Sub   
Sub sw_plunger_lane_unhit() : Controller.Switch 20, 0 :  End Sub
Sub sw_spinner_spin() : vpmTimer.PulseSw 21:  End Sub

Bumper001_Hit : vpmTimer.PulseSw 29
```

