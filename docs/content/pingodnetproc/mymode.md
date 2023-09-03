---
title: "Mode - MyMode"
date: 2022-12-26T15:26:15Z
draft: false
weight: 30
---

In the `modes` directory you'll find `MyMode.cs`. This will be loaded when the default game is run.

It's is a P-ROC mode script using the base `PinGodProcMode` but this mode doesn't dislay any scene, it runs as mode to and just access game controller.

---
### P-ROC Mode Start / Stop
---

When this mode starts it uses the `Trough` to launch a ball in play.

---
### P-ROC Switch Handlers
---

|switch|Description|
|---|---|
|start|adds more players if ball=1 , credits and players < 4|
|slingL|add 100 points|
|slingR|add 100 points|
|inlaneL|add 100 points|
|inlaneR|add 100 points|
|outlaneL|add 100 points|
|outlaneR|add 100 points|
|saucer|when active for 1 second it will add 250 then eject the ball|
|bumperHit|All bumpers call this for 150 points|
|targetHit|All targets tagged with targetBank add 200 points|

### Creating Modes
---

