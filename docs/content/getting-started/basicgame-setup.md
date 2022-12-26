---
title: "BasicGame - Setup / AddOns"
date: 2017-10-17T15:26:15Z
lastmod: 2019-10-26T15:26:15Z
draft: false
weight: 23
---

## BasicGame Template
---

Clone the repository into same directory the `pingod-addons` is:

 `git clone https://github.com/horseyhorsey/pingod-basicgame`

You will have two directories `/pingod/pingod-basicgame`, `/pingod/pingod-addons`, this is so the basicgame only has to go up one directory to link the `addons`

## Setup
---

To run the `BasicGameGodot` project you will need the `addons` directory linked or copied.

These `addons` contain base files and pinball framework that a game will use.

#### Link the PinGod.VP.AddOns

You can copy the `addons` directory to your project (`BasicGameGodot` in this example), but it's better to use a `symbolic link` to the addons so we are not duplicating files and you can keep the same addons across projects.

The `_link_addons.bat` file that you can use in the projects root directory can do this for you when running it, see image:

A shortcut `addons` folder will be added in the Godot project.

![image](../../images/basicgame-project-files.jpg)
