---
title: "Creating custom score mode scene"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 44
---

Start with duplicating the `ScoreMode.tscn` file from the addons. 

Filter the files in godot for `score` and duplicate it, rename it to `MygameScoreMode` or anything like that. You want to move the file after it's created to somewhere in your root like `res://scenes`.

The following image is the steps above and loaded.

![image](../../images/godot4-newproject/custom-score-mode-a.jpg)

If you select the `ScoreMode` node and look in the inspector these are the options from that script.

Focusing on the `Score Labels` this is where you can select the scores for the players. If you had a 10 player game you could add some more labels and select them here.

![image](../../images/godot4-newproject/custom-score-mode-b.jpg)

---
### Customizing
---

It's up to you what you do here but you are not limited to anything but imagination.

I have made several different score displays all using this as a base which are all a bit different.

You can extend the script, so this script can cover most scoring, so all you worry about here is making it nice with Godot and the game will handle the scores.

---
### Extend ScoreMode.cs
---
If you click on the script dropdown in the ScoreMode inspector window, as 2nd image shows then you can extend from here.

Click extend then C#, the name should be the same as your scene but with the `.cs` extension.

Godot will asign this script after extending but the base class is incorrect. Change this from `Node` to `ScoreMode`.

```
using Godot;
using PinGod.Modes;
using System;

public partial class MyTemplateScoreMode : ScoreMode
{
}
```

Now you can do what you like here too if you need something extra in the mode. 

An example would be overriding the `OnScoresUpdated` and updating other things like other player stats in the game that you have in your scene.