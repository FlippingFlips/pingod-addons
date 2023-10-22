# pingod-resources

This module can preload your scenes and `.pck` for Godot.

---
### AutoLoad
The `Resources` class can be accessed from the root tree under `root/Resources`.

---
### Scenes

Open the `pingod-resources/Resources.tscn` in Godot and the scene will display a simple Loading screen which will update scenes, packs as they are loaded.

In the inspector you have the option to `Load Packed Scenes On Load` which will load any scenes added to this array.

Your main scene should wait for this to be loaded by using the resource loaded.

---
### Resource Loaded Event

Act when all resources have loaded. In the `_ready` method for example you can get the resource singleton from the root.

```
	_resources = GetNode("/root/Resources") as Resources;
	_resources.ResourcesLoaded += _resources_ResourcesLoaded;
```

---
### Pack Scenes
By default some scenes are used from `pingod-modes`. `Attract.tscn, Bonus.tscn, Multiball.tscn, Tilt.tscn, ScoreEntry.tscn`.
This list in the scene can be removed, added to.

---
#### Resource Packs
Add packs to the array to load these. `pck` can be made from Godot like DLC or to contain assets on their own.

