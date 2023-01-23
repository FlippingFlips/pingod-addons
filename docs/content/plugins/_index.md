---
title: "Plugins - Autoload"
date: 2022-12-26T15:26:15Z
draft: false
weight: 10
---

Most PinGod Godot plugins are included in the `addons/AutoLoad` directory.

These plug-ins (addons) are used to enable certain features or disable them completely for when you want to develop scenes on their own for instance.

These should work in any order you enable them. When you enable one then this is added to the Autoload in Project Settings.

|Plugin|Description|
|---|---|
|WindowActions|Helper to use keys with window, switches, should always be enabled to close with ESC and F2 to move window|
|Adjustments|Global adjustments so your scene scripts can use global variables or settings|
|Resources|async loading of scenes, stores packed scenes and loads packs etc|
|Machine|Plugin to hold all the machine items and do other things like switch handling|
|MemoryMap|Windows memory mapped file for sharing states with a simulator or other applications|
|AudioManager|Load audio assets and play|
|PinGodGame|Main game framework|

![image](../images/plugins.jpg)

---
## Overriding the autoload scenes
---

The autoload folder is used when you want to override the AutoLoad scenes. 

If you wanted more switches to the  plugin then you would want your own Machine.tscn.

---

In Godot you can duplicate the default `Machine.tscn` like so:

1. In the `FileSystem` pane filter files. Filter it for `machine`
2. The one we want is `Machine.tscn`. There will be just a few results.
3. Right click Duplicate and suffix it with 2 or anything like `Machine2.tscn`

![image](../images/godot4-newproject/filter-and-duplicate.jpg)

4. You will see it in the FileSystem and you can right click `MoveTo`, move to the `res://autoload` folder
5. With the FileSystem still open, on the moved file, `F2` and rename back to `Machine.tscn`

![image](../images/godot4-newproject/machine-duplicated.jpg)

6. Now you can open the `res://Machine.tscn` and change some settings, add items.

#### 7. Disable then Enable the plugin in ProjectSettings for the plugin to pick up your AutoLoad scene

---
## Extra info
---

To find what scene the AutoLoad is using by default go to `Project Settings > AutoLoad`, in the list is the path to the scene.

You can also click to take you there. Note: You will need the plugins activated to see them in that pane.

---