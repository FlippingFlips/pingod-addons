# pingod-modes-tilt

This script uses the `Adjustments.TiltWarnings` and hooks onto the machines `OnSwitchCommand`. See the following overridable methods.

- SetText
- OnTilt
- OnSlamTilt
- OnSwitchCommand
- ShowWarning

Signals: `GameTiltedEvent`

Plugins: `root/Machine`, `root/Trough`, `root/PinGodGame`