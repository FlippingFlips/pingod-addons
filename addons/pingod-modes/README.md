# pingod-modes

General base modes and scenes for pinball. Most "modes" are a single scene and script to use as a base. Games can extend from or use as starting point.

---
| Name |  Description 
| --- | --- |
| [attract](attract) | Simple attract mode. Add scenes to `AttractLayers` and cycle with flipper switches
| [ballsave](ballsave)  | Displays then removes a scene when a ball is saved
| [bonus](bonus) | Bonus display screen for end of ball 
| [highscores](highscores) | Uses a Godot `RichTextLabel` to display high scores. Used by `Attract`?
| [mode-timer](mode-timer) | timed mode display
| [multiball](multiball)| A scene that runs and frees. Uses the game plugin `IPinGodGame.StartMultiBall`, `ModeTimer`
| [pause-settings](pause-settings) | 
| [plunger-lane](plunger-lane)  | |
| [scoreentry](scoreentry) | Basic score entry for initals / name. | 
| [scoremode](scoremode) | Basic score mode for 4 players. Add labels in the scene to `ScoreLabels`. 
| [servicemenu](servicemenu) | Basic menu, coin door service buttons event handler. Uses the Machines `OnSwitchCommand` to act on `up,down,enter,exit` switches
| [tilt](tilt) | 
| [trough](trough) | 
