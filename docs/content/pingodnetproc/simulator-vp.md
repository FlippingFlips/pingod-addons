---
title: "Simulator - Visual Pinball"
date: 2022-12-26T15:26:15Z
draft: false
weight: 32
---

There is a demo table provided in the `visual_pinball` directory. 

🔗 [Visual Pinball 10 - Releases](https://github.com/vpinball/vpinball/releases)

Script is almost identical to the PinGodGame.

---

To run this with Visual Pinball you just need a COM controller setup and scripts for visual pinball then the game can be launched from the `.bat` files or manually.

🔗 [pingod-controller-com - Releases](https://github.com/FlippingFlips/pingod-controller-com/releases)

---
## Visual Pinball - Assets
---

Provided with this table example are assets used to create the graphics for VP.

This gives a quick workflow for you if you're new to working with graphics, adding exporting, importing to the simulator.

---
### Playfield.svg
---

We used open source `Inkscape` to work on this file. It just consists of separate layers which you can hide/unhide on different exports.

This helps you keep inside a single file at the same size. All the plastics, lights, playfield can be exported separate quickly and reimported to VP.

To export, show what you need from the layers, then use the `Export PNG image`. If you did the `Playfield and Wood` layers then export the image `Page` size and replace the `playfield.png` provided.

Visual Pinball `ImageManager` using `F3`. The images in here are named `decals_inserts` and `decals_lamps` you can replace these. First changes you can `Reimport From`, but any changes after that you can just `Reimport`.

---

|Layer|Description|
|---|---|
|Wood|Base background layer, export with playfield|
|Blueprints|Used to see where your objects are placed|
|Playfield|main artwork layer|
|Plastics|Use a plastic blueprint from vp|
|Inserts|Light inserts (optional) think better when using RGB lamps|
|Decals|Decals (optional)|

![image](../../images/visual-pinball/inkscape-playfield.jpg)

---
### Visual Pinball Export Blueprint
---

When you export this from visual pinball and replace the blueprint provided in the same folder then your `playfield.svg` should also update with latest blueprint