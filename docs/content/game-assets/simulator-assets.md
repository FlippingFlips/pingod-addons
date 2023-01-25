---
title: "Simulator - Assets"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 11
---

A rundown and overview how we have made the templates and assets for the simulator.

---
## Visual Pinball - Assets
---

Provided with some table examples are assets used to create the graphics for Visual Pinball.

This gives a quick workflow for you if you're new to working with graphics, exporting, importing to the simulator.

---
### Playfield.svg
---

Use this file with `Inkscape`. It just consists of separate layers which you can hide/unhide to do different exports.

This helps you keep inside a single file at the same size. All the plastics, lights, playfield can be exported separate quickly and reimported to VP.

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
#### Exporting

Show what you need from the layers then use the `Export PNG image` set to `Page`.

#### WebP

If you export the playfield layer from the `svg` to `png` then it can be fairly large. You can convert the image to webp that you get from the `Tools - Images` section.

`magick convert vp-playfield.png vp-playfield.webp`

---
#### Import To Visual Pinball

Open the Visual Pinball `ImageManager` using `F3`. 

First you will use `Reimport From` on your new image but any changes after that you can just `Reimport` from the same location.


---
### Blueprints
---

When you export this from visual pinball and replace the blueprint provided in the same folder then your `playfield.svg` should also update with the latest blueprint.

`File > Export > Blueprint`