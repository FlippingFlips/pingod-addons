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
