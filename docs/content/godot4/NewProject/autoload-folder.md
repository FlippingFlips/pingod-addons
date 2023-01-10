---
title: "res://autoload - The local autoload folder"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 41
---

The autoload folder is when you want to override the AutoLoad scenes. If you wanted more switches to the MachineNode then you would want your own Machine.tscn.

In Godot you can duplicate like so:

1. In the `FileSystem` pane filter files. Filter it for `machine`
2. The one we want is `Machine.tscn`. There will be just a few results.
3. Right click Duplicate and suffix it with 2 or anything like `Machine2.tscn`
4. You will see it in the FileSystem and you can right click `MoveTo`, move to the `res://autoload` folder
5. With the FileSystem still open, on the moved file, `F2` and rename back to `Machine.tscn`
6. Now you can open the `res://Machine.tscn` and change some settings, add items.
7. Disable then Enable the plugin in ProjectSettings for the plugin to pick up your AutoLoad scene

---

To find what scene the AutoLoad is using by default go to Project Settings > AutoLoad, in the list is the path to the scene.

You can also click to take you there. Note: You will need the plugins activated to see them in that pane.

---

If you're making game from scratch and you intend it to be full then at the start it would be easier to copy each scene from the AutoLoad directory.