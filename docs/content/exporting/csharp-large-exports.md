---
title: "C# Pcks Are Large..."
date: 2022-12-26T15:26:15Z
draft: false
weight: 50
---

{{% panel status="primary" title="" icon="far fa-lightbulb" %}}

This doesn't apply if you decide to have a single executable and pck, so you can skip this

{{% /panel %}}

When a project uses mono C# it includes mono assembilies which are around 29mb.

If you try to export the assets only from this project, then you will always have the c# files.

![image](../../images/basicgame-exported-pck-test.jpg)

---
# Link the assets into BasicGamePck
---

One method is to use the `BasicGamePck/project.godot` project. This project is empty, no scenes or scripts.

We can reuse the assets and .imports directories from `BasicGameGodot` with the `BasicGamePck` project, by linking again.

Then use the `BasicGamePck` project just for exporting assets, scenes, no C#, with very small outputs

This allows us to use the `BasicGameGodot` `project.godot` and skip assets or whatever it is exporting.

---
## Link Directories
---

Remove the assets directory if there is one in `BasicGamePck`

1. Symbolic link the `BasicGameGodot\assets` directory into the `BasicGamePck` directory.

2. Symbolic link the `BasicGameGodot\.import` directory into the `BasicGamePck` directory.

3. When we link the `.import`, `.assets` here we should be on par with the `BasicGameGodot` project.
