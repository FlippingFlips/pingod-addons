# pingod-machine

Represents a pinball machine and holds all the machine items like switches, coils, lights.

---
### AutoLoad
The `Resources` class can be accessed from the root tree under `root/Machine`.

- Hooks onto the memory map plugin if found
- Handles Ball searching

---
### Scenes
Open the `Machine.tscn` in Godot and see Inspector for options:
- Ball Search Options
- Recording / Playback
- Switch Window
- Machine Items

### Default Switches
```
_switches = {
"coin1": 0,
"coin2": 1,
"coin3": 2,
"coinDoor": 3,
"enter": 4,
"down": 5,
"up": 6
"exit": 7,
"start": 8,
"tilt": 9,
"slamTilt": 10,
"flipperLwL": 16,
"flipperLwR": 18,
"outlaneL": 20,
"inlaneL": 21,
"slingL": 22,
"inlaneR": 23,
"slingR": 24,
"outlaneR": 25,
"plungerLane": 26,
"trough0": 27,
"trough1": 28,
"trough2": 29,
"trough3": 30,
"mballSaucer": 31
}
```

### Default Coils
```
_coils = {
"trough": 0,
"flippers": 2,
"auto_plunger": 3,
"mballSaucer": 4,
}
```
