---
title: "Link assets into separate assets project"
date: 2022-12-26T15:26:15Z
draft: false
weight: 50
---

We can reuse the assets and .import directories from `BasicGameGodot` into the `BasicGamePck` project.

Use the `BasicGamePck` project just for exporting assets, scenes, no C# scripts.

---
### Link assets
---

Remove the assets directory if there is one in `BasicGamePck`

This can be done with `_link_assets.bat` helper bat file.

1. Symbolic link the `BasicGameGodot\assets` directory into the `BasicGamePck` directory.

2. Symbolic link the `BasicGameGodot\.import` directory into the `BasicGamePck` directory.

#### _link_assets.bat:

```
SET cDir=%cd%
cd ../BasicGameGodot
SET baseGodotDir=%cd%
cd assets
SET assetDir=%cd%
cd ../.import
SET importDir=%cd%
cd %cDir%
mklink /D assets "%assetDir%"
mklink /D .import "%importDir%"
```

3. When we link the `.import`, `.assets` here we should be on par with the `BasicGameGodot` project all the time so we can just build out packs.

![image](../../images/exports-link-assets-import.jpg)
