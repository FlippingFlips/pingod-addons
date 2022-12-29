---
title: "BasicGame - Create first asset scene"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 41
---

1. In Godot right click assets directory and add a new folder named `scenes`.

![image](../../images/create-scene-scenes-folder.jpg)

- Right click the new `scenes` folder and add new scene and name it `test_scene`.

2. Select the type of node to be a `Sprite` by using `Other Node` for this scenes Root Node.

![image](../../images/create-scene-scenes-node-type.jpg)

Save the scene `Ctrl+S` after it opens and the file will be created `test_scene.tscn`

3. Find an image on disk and drag it into the Godot path. `res://assets/img/`. In the example I have dropped in `drac_vp.jpg`.

![image](../../images/create-scene-scenes-add-image.jpg)

4. Now select the Sprite

5. In the inspector select the image in the texture dropdown. Quickload you will see your image, so select it there.

Use the new sprite in scene:

1. Open the `BaseMode.tscn` in Godot

2. On the BaseNode parent right click to `Instance Child Scene`

3. Select the sprite `test_scene`

4. Run the game and you'll see the `test_scene` on screen.

![image](../../images/create-scene-sprite-ingame.jpg)