# Modes - P-ROC

NetProc GameController modes that are added to the NetProc Games Mode Queue. 

Scenes for the game belong in `res://scenes`, for example `res://scenes/AttractMode`

### Mode Scenes
---

Scenes are not required for P-ROC modes but they can be added when the mode starts in `"ModeStarted"` and removed in `"ModeEnded"`.

The scene you send should be the full game path. For the attract it is `"res://scenes/AttractMode/AttractProc.tscn"`

To add a scene on UI thread:

`_pingod.CallDeferred("AddModeScene", ATTRACT_SCENE);`

To remove a scene on UI thread:

`_pingod.CallDeferred("RemoveModeScene", ATTRACT_SCENE);`


### Default Modes
---

- MachineSwitchHandlerMode
- AttractMode
- ScoreDisplayMode
- MyMode
