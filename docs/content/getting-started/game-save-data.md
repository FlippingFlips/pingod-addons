---
title: "User Game setting / data"
date: 2022-12-26T15:26:15Z
draft: false
weight: 25
---

In the appdata directory under the game name. `%AppData%\Godot\app_userdata\BasicGameGodot\settings.save` 

You can open this directory with the `_godot_appdata_dir.bat` file

---
## settings.save and gamedata.save
---

Settings.save is json text, readable and editable.

This stores

- display settings
- game settings
- audio settings
- memory map options

![image](../../images/basicgame/basicgame-settings-save.jpg)


- You don't need to edit these by hand.
- Any of your added settings will go in this file.

---
## Recordings
---

See [Recordings / Playback](../../pingodgame/recordings-playback)