---
title: "Recording / Playback"
date: 2022-12-26T15:26:15Z
draft: false
weight: 60
---

Quick guide on how to record and playback a game. 

Recording a game can speed up your game development for game logic.

They would usually start off by using trough switches then adding credits and starting a game.

Any events after this to test any recorded sequence like clearing a target bank, draining your ball.

Testing modes and testing the complete game.

## Recording

Recording files are saved in the user://recordings/recordname.record

Example game dir: *C:\Users\UserName\AppData\Roaming\Godot\app_userdata\BasicGameGodot\recordings*

{{% panel status="primary" title="Note" icon="far fa-lightbulb" %}}
You can get to the app_data directory for the game using app_data bat file
{{% /panel %}}

### Record Step by Step
---

You can enable a switch window from the inspector options when running to record switches from.

---

- Enable record in the `autoload/Machine.tscn` godot inspector or direct in file
- Run the game. Hold down `Q,W,E,R` (Activate Trough switches) and push start `1`
- *You can release the trough switches*
- Activate all the trough switches again to end the ball.
- `ESC` to close the game window
- A playback filename will be logged to the output window

## Playback

- Enable playback in the `autoload/Machine.tscn` inspector
- Add the filename in the inspector `user://recordings/recordname.record`
- Run the game to playback the recorded events
- Logs will display when finished

## Tips / Notes

- Records could also be loaded from the project directory `res://` . Some files are not visible in the FileSystem but can still be accessed.
- Rename and keep good records to use for automation `res://recordings/drain_3_balls.record`