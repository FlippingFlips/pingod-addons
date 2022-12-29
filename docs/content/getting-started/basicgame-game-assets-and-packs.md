---
title: "BasicGame - Assets and Pcks"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 41
---

The `BasicGamePck` directory is an extra project for exporting asset packs. Using this project means we can keep assets from the game separated. 

You may also want to make a base asset pack to copy and load across multiple games. 

You are not limited to media, it could be a scene with assets included.

---

There is an empty folder structure in game which you can add your assets into, then you can run the `_build` bat files to export a `.pck` file for gfx and audio.

See [Link separate assets project](../exporting/link-separate-assets-project/) on how to make from linking.

Your assets will keep the same file structure, so to reach one of your assets in script the path would be `"res://assets/img/myimage.png`.

