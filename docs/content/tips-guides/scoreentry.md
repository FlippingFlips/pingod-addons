---
title: "Creating custom Score Entry scene"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 5
---

1. Filter Godot files by `ScoreEntry`
2. Duplicate the `ScoreEntry.tscn` and move to your `res://autoload` directory
3. Add the packed scene location, replacing any existing in the `res:autoload/Resources.tscn`
4. Add the new scene into the `Game.tscn` so it knows where your version is
5. Edit away without effecting the `addons` and script. You can take it further than cosmetic from here with a script.

Things you can try in the scene:

1. Setting a theme on the top control `ScoreEntry`, which will set all in the scene, provided you have made custom theme entries like `Label` themes.
2. Changing the labels font and colors, sizes, outlines, shadows.
3. If you change the size you can `Select Char Margin` to adjust the distance between the characters.