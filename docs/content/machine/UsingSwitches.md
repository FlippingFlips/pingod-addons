---
title: "Using Switches C#"
date: 2022-12-26T15:26:15Z
draft: false
weight: 30
---

---
### Using in game scene c# scripts
---

Connect to the Machines SwitchCommand. Only needs to be done once in _EnterTree or _Ready.

```
public override void _Ready()
{
    base._Ready();
    if (HasNode("/root/PinGodGame"))
    {
        pinGod = GetNode("/root/PinGodGame") as IPinGodGame;
        //use the switch command on machine through the game as we're in a game
        pinGod.MachineNode.SwitchCommand += OnSwitchCommandHandler;
    }
    else { Logger.WarningRich(nameof(BaseMode), "[color=red]", ": no PinGodGame found", "[/color]"); }
}
```

Create handler.

```
/// <summary>
/// Switch handlers for lanes and slingshots
/// </summary>
/// <param name="name"></param>
/// <param name="index"></param>
/// <param name="value"></param>
private void OnSwitchCommandHandler(string name, byte index, byte value)
{
    if (value <= 0) return;
    switch (name)
    {
        case "outlaneL":
        case "outlaneR":
            game.AddPoints(100);
            break;
        case "inlaneL":
        case "inlaneR":
            game.AddPoints(50);
            break;
        case "slingL":
        case "slingR":
            game.AddPoints(50);
            break;
        default:
            break;
    }
}
```

#### Check a switch state

```
bool switchOn = Machine.Switches["plungerLane"].IsEnabled

//or
switchOn = pinGod.IsSwitchEnabled(swName);
```