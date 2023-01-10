# display-plug-ins / scripts

---
## ScreenLayerSlideTweener
---
Simple full screen control slider like older DMD pinball games. `Slide in Time > Pause Center Screen Time > Slide out Time`

A good way to use this would be to create a scene of type Control. The demo scene uses a ColorRect (which is a control) for a black background:

1. Copy the default demo scene for this script in `display-plugins/layers`
2. Leave the Label controls intact, set text empty if you don't need them.
3. Test some slide ins. When happy with the scene then save a new version.

Now in your game script, load the scene you saved as a packed scene, now you can make instances of the slider when you need to.

---
## SimpleModeScene
---

The default setup consists of:

- A Background, Foreground and two text labels. `LabelLg` and `LabelSm`
- A timer that will clear the display if set to
- An animated sprite which can go under your BG and FG layers.

In the inspector properties is an added `Animations`. Select a saved animated sprite scene.

These scenes would be a single AnimatedSprite2D but with multiple animation with names that you can set.

```
_simpleModeScene.PlaySequence("man_talk", "MAN AT ARMS", "DUNCAN SHOT COMPLETED");

//with added delay set, defaults to 2
_simpleModeScene.PlaySequence("man_talk", "MAN AT ARMS", "DUNCAN SHOT COMPLETED", 3);

//show indefinitely
_simpleModeScene.PlaySequence("man_talk", "MAN AT ARMS", "DUNCAN SHOT COMPLETED", 0);

//show points with titles
_simpleModeScene.PlaySequence("skillshot", "SKILLSHOT", $"SCOOP {_player.SkillshotScoopAward.ToScoreString()}");
```

