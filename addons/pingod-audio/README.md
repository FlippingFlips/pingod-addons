# PinGod-Audio - Player and Assets
When this plugin is enabled it creates an Autoload singleton.

---
### AutoLoad
The `IAudioManager` interface or `AudioManager` class can be accessed from the root tree under `root/AudioManager`.
`IPinGodGame` uses this interface if available.

---
### Scenes
AudioManager.tscn scene is autoloaded loaded with the script. Audio Stream Players are visible in the tree when you open the scene in Godot.

---
### Exports
Exports are properties exported from script that you can change in the scene.
- Add audio stream resources to the `*Assets` collections. `MusicAssets`, `VoiceAssets`.
- Set default BGM. Background music default
- Enable different buses `MusicEnabled`

---
### IAudioManager
Interface to control media players and add assets with keys. See [IAudioManager](Node/IAudioManager.cs) for methods, properties.

This is enough for a basic game but you can always add more players, more buses.

---
### Mixer Layout
The layout `tres` file [audio_bus_layout.tres](audio_bus_layout.tres) should be loaded into the mixer or default mixer file in project settings

