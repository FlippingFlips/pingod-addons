---
title: "Plugin - Resources"
date: 2022-12-26T15:26:15Z
draft: false
weight: 13
---

The Resources plugin is a preloader of assets and scenes which are called Packed Scenes.

We can get a resource from this autoload and instance scenes with it.

---
### Packed Scenes
---

These scenes will be loaded with a game. By default the BasicGame loads the following:

- Bonus.tscn
- ScoreEntry.tscn
- Tilt.tscn
- Multiball.tscn
- Attract.tscn

Once these scenes and all resources have been loaded then we add a scene like the Attract.tscn.

---
### Resources (Key, Path)
---

Add resources that will be pre loaded in this dictionary by key , path.

Key: `nameForScript` , Value `res://assets/yourasset.ogv`.

Key: `nameForScript2` , Value `res://myotherassets/asset.tscn`.

---
### Resource Packs
---

By default the collection includes the export packs in the previous section. `pingod.gfx.pck` and `pingod.snd.pck`

You can add with the scene inspector or directly in the `tscn` file it saves to.

![image](../images/pingodgame-resources-inspector.jpg)


