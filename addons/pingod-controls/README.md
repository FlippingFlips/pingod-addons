# pingod-controls

### Tools

These controls can be added to scene from the godot editor add node.

|Name|Type|Desc|
|-|-|-|
|[BlinkingLabel](label/BlinkingLabel.cs)|Label|Label that blinks|
|[Bumper](node/Bumper.cs)|Node|Act when a bumper is active with audio stream. Set switch, uses machine and pingodgame|
|[CreditsLabel](label/CreditsLabel.cs)|Label|Uses an IPinGodGame to update from credit (coin) events|
|[Saucer](timer/Saucer.cs)|Timer|Act like a saucer, scoop. Add to scene. Add `coil` and `switch` in inspector and hook onto `SwitchActive` or `SwitchInActive` |
|[TargetsBank](node/TargetsBank.cs)|Node|Handles a bank of targets, the light states and watches for completion|
|[VideoPlayerPinball](videoplayer/VideoPlayerPinball.cs)|VideoStreamPlayer|Handles looping video and other events|

