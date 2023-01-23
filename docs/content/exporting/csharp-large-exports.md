---
title: "C# Pcks Are Large..."
date: 2022-12-26T15:26:15Z
draft: false
weight: 50
---

{{% panel status="primary" title="" icon="far fa-lightbulb" %}}

This doesn't apply if you decide to have a single executable and pck, so you can skip this

{{% /panel %}}

*Note: This could have changed in Godot 4, untested*

---

When a project uses mono C# it includes mono assembilies which are around 29mb.

If you try to export the assets only from this project, then you will always have the c# files.

![image](../../images/basicgame-exported-pck-test.jpg)

One workaround method is to use the `BasicGamePck/project.godot` project. This project is empty, no scenes or C# scripts.

[Linking assets](./link-separate-assets-project)

