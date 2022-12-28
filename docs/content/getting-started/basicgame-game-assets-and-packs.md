---
title: "BasicGame - Assets and Packs"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 41
---

The `BasicGamePck` directory is an extra exporter for asset packs, doing it this way means we can keep assets from the game packs separated. One example is that if the assets never change then you don't need to update those files. You may also want to make a base asset pack to copy and load across multiple games. You are not limited to media, it could be a scene with assets included.

There is an empty folder structure where you can add your assets into, then you can run the `_build` bat files to export a `.pck` file for gfx and audio.

For in game scripts and scenes your assets will keep the same file structure, so to reach one of your assets the path would be `"res://assets/img/myimage.png`.

## Add a new asset

1. Duplicate the image from `BasicGamePck\assets\img\pingod-logo-test.png` and rename it to `my-image-asset.png`

![image](../../images/basicgame-add-asset.jpg)

2. In `BasicGamePck` run the export with `_build_gfx.bat`.

3. In `BasicGamePck` run the export with `_build_sfx.bat`.

4. Check the build folder for your exports. `pingod.gfx.pck` and `pingod.snd.pck`

![image](../../images/basicgame-add-asset-packs.jpg)
