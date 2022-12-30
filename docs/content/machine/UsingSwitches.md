---
title: "Using Switches C#"
date: 2022-12-26T15:26:15Z
draft: false
weight: 30
---

---
### Using in game scene c# scripts
---

Connect to a SwitchCommand. Only needs to be done once in EnterTree

```
//Godot Enter Tree Override. Get pingod and connect to a SwitchCommand signal on it
public override void _EnterTree()
{
    pinGod = GetNode("/root/PinGodGame") as PinGodGame;		
    pinGod.Connect(nameof(SwitchCommand), this, nameof(SwitchCommandHandler));
}
```

Create handler.

```
//do stuff on switches. this example just acts when switch is on
private void SwitchCommandHandler(string swName, byte index, byte value)
{
    if (!pinGod.GameInPlay || pinGod.IsTilted) return;
    if (value > 0)
    {
        switch (swName)
        {
            case "inlane_l":					
            case "inlane_r":
            case "bumper_l":
            case "bumper_r":
                AddPointsPlaySound(SMALL_SCORE);
                break;
            case "outlane_r":
            case "outlane_l":
                AddPointsPlaySound(MED_SCORE);
                break;
            case "sling_l":
            case "sling_r":
            case "spinner":
                AddPointsPlaySound(MIN_SCORE);
                break;
            case "top_left_target":
                AddPointsPlaySound(LARGE_SCORE);
                break;
            default:
                break;
        }
    }		
}
```

#### Check a switch state

```
bool switchOn = Machine.Switches["plunger_lane"].IsEnabled

//or
switchOn = pinGod.IsSwitchEnabled(swName);
```