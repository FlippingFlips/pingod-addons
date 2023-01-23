---
title: "Mode - ScoreDisplayProcMode"
date: 2022-12-26T15:26:15Z
draft: false
weight: 26
---

In the `modes` directory you'll find `ScoreDisplayProcMode.cs`. This is a P-ROC mode script using the base `PinGodProcMode` because we need visible layers here.

Uses a reference to a scene in `res://scenes/ScoreMode/ScoreModeProc.tscn`. This scene is also set in the `Resources.tscn`. To pre load it so we can just create instances when we need to.

![image](../../images/p-roc/score-proc.jpg)

---
### Scene and Script
---

The change here lies within the `ScoreModeProc.cs` which is based off the existing PinGodGame `ScoreMode.cs`.

We just override where the scores are coming from, in this case coming from the P-ROC game players and ball information.

---
### P-ROC Mode Start / Stop
---

- When the Mode is started then this scene is retrieved from the Resources and added as a child.

- When the Mode is stopped then the scene instance we created is freed and removed.

---
### P-ROC Switch Handlers
---

None

---

🔗 [Base ScoreMode Scene](../../modes/score-mode/)
