using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
/// Resources, graphic packs. Set to AutoLoad with a Resources.tsn in ProjectSettings. <para/>
/// 
/// </summary>
public partial class Resources : Node
{
    /// <summary>
    /// Working directory to load pingod.gfx.pck
    /// </summary>
    public static string WorkingDirectory = string.Empty;

    Control _loadingControl;
    private Label _label2;
    [ExportCategory("Packed Scenes")]
    [Export] bool _loadPackedScenesOnLoad = true;

    /// <summary>
    /// Set scenes that will load when resources _Ready
    /// </summary>
    [Export(PropertyHint.Dir, "index,scene")] Godot.Collections.Array<string> _preloadPackedScenes = new Godot.Collections.Array<string>();

    private string _resourceLoadingPath;
    [Export] Godot.Collections.Array<string> _resourcePacks = new Godot.Collections.Array<string>() { "pingod.gfx.pck", "pingod.snd.pck" };
    /// <summary>
    /// A list of packs loaded as resources
    /// </summary>
    ResourcePreloader _resourcePreloader = new ResourcePreloader();

    [Export] Godot.Collections.Dictionary<string, string> _resources = new Godot.Collections.Dictionary<string, string>();
    private Queue<string> _resourcesLoading = new();

    /// <summary>
    /// Loads gfx resource packs from a packs directory.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        SetProcess(false);
        
        if (!Engine.IsEditorHint())
        {
            _loadingControl = GetNodeOrNull<Control>("LoadingControl");
            _label2 = _loadingControl?.GetNodeOrNull<Label>("Label2");
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _resourcesLoading?.Clear();
        SetProcess(false);
    }

    /// <summary>
    /// delay time. give 1 second initial then 0.2 when loading each scene. The delay is for checking if the scene has loaded in background and is complete
    /// </summary>
    double time = 1.0;
    public override void _Process(double delta)
    {
        if (_resourcesLoading?.Count > 0)
        {
            time -= delta;
            if(time < 0)
            {
                var queItem = _resourcesLoading.Peek();
                _label2.Text = queItem;
                var status = ResourceLoader.LoadThreadedGetStatus(queItem);
                if (status == ResourceLoader.ThreadLoadStatus.Failed || status == ResourceLoader.ThreadLoadStatus.InvalidResource)
                {
                    Logger.WarningRich("[color=red]", nameof(Resources), $": {queItem} failed to load.", "[/color]");
                    _resourcesLoading.Dequeue();
                }
                else if (status == ResourceLoader.ThreadLoadStatus.Loaded)
                {
                    var res = ResourceLoader.LoadThreadedGet(queItem) as PackedScene;
                    AddResource(queItem.GetBaseName(), res);
                    Logger.Debug(nameof(Resources), $": loading complete threaded: {queItem} . Added to resources.");
                    _resourcesLoading.Dequeue();
                }                
            }            
        }
        else { SetProcess(false); ShowLoading(false); time = 0.5; }
    }

    public override void _Ready()
    {
        SetProcess(false);
        base._Ready();
        if (!Engine.IsEditorHint())
        {
            if (_resourcePacks?.Count > 0)
            {
                var exePath = OS.GetExecutablePath();
                WorkingDirectory = System.IO.Path.GetDirectoryName(exePath);
                foreach (var resourcePack in _resourcePacks)
                {
                    if (!LoadResourcePack("res://" + resourcePack))
                    {
                        LoadResourcePack(System.IO.Path.Combine(WorkingDirectory, resourcePack));
                    }
                }

                LoadResources();
            }

            Logger.Info(nameof(Resources), ": load packed scenes? :", _loadPackedScenesOnLoad);
            if (_loadPackedScenesOnLoad)
            {                
                RunPreloadScenes();
            }                
        }            
    }
    public virtual void AddResource(string name, Resource res) => _resourcePreloader.AddResource(name, res);

    public virtual Resource GetResource(string name) => _resourcePreloader.GetResource(name);

    public virtual string[] GetResourceList() => _resourcePreloader.GetResourceList();

    public virtual bool HasResource(string name) => _resourcePreloader.HasResource(name);

    public void LoadResourceThreaded(string path, bool subThreads = true, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Ignore)
    {
        if (!_resourcesLoading.Contains(path))
        {
            //path = path.GetBaseName();
            var error = ResourceLoader.LoadThreadedRequest(path, useSubThreads: subThreads, cacheMode: cacheMode);
            if (error == Error.Ok)
            {
                Logger.Info(nameof(Resources), ": loading threaded resource: ", path);
                _resourcesLoading.Enqueue(path);
                SetProcess(true);
                ShowLoading(true);
            }
        }        
    }
    public virtual void RemoveResource(string name) => _resourcePreloader.RemoveResource(name);

    /// <summary>
    /// Gets resource from the ResourcePreloader
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Resource Resolve(string name)
    {
        if (HasResource(name))
            return GetResource(name);
        else
        {
            Logger.Warning(nameof(Resources), "resource not found. " + name);
            return null;
        }
    }

    /// <summary>
    /// Runs the multi threaded load resource from scenes specified. This should be called start of app
    /// </summary>
    public void RunPreloadScenes()
    {
        Logger.Debug(nameof(Resources), ":", nameof(_Ready), ": ** loading packed scenes. Put scenes here to load in process background and add to resources for quick access **");
        foreach (var scene in _preloadPackedScenes)
        {
            if (HasResource(scene)) continue;
            //load resource in background process thread, 1 at a time, but fast.
            //if the message to load a scene takes too long then could be issues with scene
            //having label settings set on a label was making a scene take 10 seconds. Removing tat and using the theme overrides fixed that
            LoadResourceThreaded(scene);
        }
    }

    public void ShowLoading(bool show)
    {
        if (_loadingControl != null) { _loadingControl.Visible = show; }
    }

    /// <summary>
    /// Loads a resource pack with <see cref="ProjectSettings.LoadResourcePack"/> into the resource system, res://
    /// </summary>
    /// <param name="filePath"></param>
    private static bool LoadResourcePack(string filePath)
    {
        if (ProjectSettings.LoadResourcePack(filePath))
        {            
            Logger.Info(nameof(Resources), ": resource pack loaded:", filePath);
            return true; 
        }
        else { Logger.Warning(nameof(Resources),": failed to load resource pack: " + filePath); return false; }
    }

    /// <summary>
    /// Scans directory for *gfx.pck files and loads them as resource packs
    /// </summary>
    /// <param name="pckDir"></param>
    /// <returns></returns>
    private static bool LoadResourcePackDirectory(string pckDir)
    {
        //https://docs.godotengine.org/en/latest/classes/class_diraccess.html
        var d = DirAccess.Open(pckDir);
        if (d !=null)
        {
            if (DirAccess.GetOpenError() == Error.Ok)
            {
                Logger.Info(nameof(Resources), ":found packs directory");
                d.ListDirBegin();
                var path = string.Empty;
                while ((path = d.GetNext()) != string.Empty)
                {
                    if (path.Contains(".gfx.pck"))
                    {
                        var filePath = $"{pckDir}/{path}";
                        LoadResourcePack(filePath);
                    }
                }

                return true;
            }
            else { return false; }
        }

        return false;
    }

    /// <summary>
    /// Invokes GD.Load on every resource found in resources
    /// </summary>
    private void LoadResources()
    {
        Logger.Debug("pre loading resources");
        foreach (var res in _resources)
        {
            var loaded = GD.Load(res.Value);
            AddResource(res.Key, loaded);
        }

        if (GetResourceList().Length > 0)
            Logger.Debug(string.Join(",", GetResourceList()));
        else
            Logger.Debug("no resources found in Resources.tscn");
    }
}
