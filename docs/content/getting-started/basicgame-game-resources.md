---
title: "BasicGame - Adding Resources and Resource packs"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 41
---

In the `PinGodGame.tscn` is a `ResourcePreloader` node.

When this is loaded with the scene it looks for the packs you have set in the `Resource Packs`.

![image](../../images/pingodgame-resources-tree.jpg)

---
### Resource Packs
---

By default the collection includes the export packs in the previous section. `pingod.gfx.pck` and `pingod.snd.pck`

You can add with the scene inspector or directly in the `tscn` file it saves to.

![image](../../images/pingodgame-resources-inspector.jpg)

---
### Resources (Key, Path)
---

Add resources that will be pre loaded in this dictionary by key , path.

Key: `nameForScript` , Value `res://assets/yourasset.ogv`.

Key: `nameForScript2` , Value `res://myotherassets/asset.tscn`.
