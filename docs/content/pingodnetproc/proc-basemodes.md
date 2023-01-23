---
title: "Mode - Base Modes"
date: 2022-12-26T15:26:15Z
draft: false
weight: 22
---

## Trough

This is loaded with the game controller. All you have to do is tag your switches correctly in the `.sql` machine configuration and it will handle the rest.

Make sure you have `trough` tags for switches and eject coil. Plunger lane, early save switches if using. These can be found in the example `.sql`.

---

## Other modes

In the `proc/PinGodProcGameController.cs` script when the game resources have fully loaded and called `MachineResourcesReady`, then some modes are created.

---
### Ball Save
---

When this is created a lamp / led name is passed in and a switch to active the ball save.

After the switch to activate the ball search is opened the ball save is started, in this case the `plungerLane`.

---
### Ball Search
---

When this is created it runs through our switches for `stop` and `reset` values and adds any coils tags with search.

---
### MachineSwitchHandlerMode
---

This is just a simple mode that we can handle some base switches like the coin door.

We have tagged all of our door switches with `door` so when this mode is created, it adds a handler to `HandleDoorSwitch` for every switch tagged with `door`, then the switch is dealt with in this single method.

Currently it is just adding credits but is setup for a coin door and service menu.
