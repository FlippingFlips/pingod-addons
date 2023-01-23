---
title: "PinGodProc GameController"
date: 2022-12-26T15:26:15Z
draft: false
weight: 20
---

This is the main GameController for P-ROC which is based from a Database Game Controller in NetProc.

Here you will find the main event methods for a game and get access to the machine items from the `.sql`

When the game is created a database will be created for the game which holds settings, saves audits. All of this is taken care of but you can add your own audits / adjustments to the `.sql`

---
## Run Loop
---

This class overrides the existing GameControllers `RunLoop` method. This run loop is slightly different to cater for simulation, but will work the same when not simulated.

For simulation, the states are written to memory. Coils, Lamps and LEDS.

---
## Initiliazing
---

When the `PinGodGameProc.cs` script is fully loaded then `MachineResourcesReady` is called on the game. In this method we create new modes and reset the game.

---
## Reset
---

This links ball saves with the trough and adds all of our base starting modes to the p-roc `Game.Modes`.

---
## Coils
---

You will be using coils inside modes and the game. Just access the `Coils` dictionary from the game like:

`Game.Coils["saucerEject"].pulse(40)`

*You should only use `Enable` if using a simulation.*

---
## Database Calls
---

You can add your own adjustments and audits in the sql file:

```
GetTopScores() // get top scores in database. 
GetAdjustment("DISP_MODE") // gets an adjustment value
GetAudit("CREDITS") // gets an audit value
SetAdjustment("DISP_MODE", 2) // gets an adjustment value
SetAudit("CREDITS", 50) // gets an audit value
```
