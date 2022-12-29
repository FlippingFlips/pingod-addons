using Godot;

/// <summary>
/// Resources, graphic packs. Set to AutoLoad with a Resources.tsn in ProjectSettings. <para/>
/// 
/// </summary>
public class Resources : ResourcePreloader
{
    [Export] Godot.Collections.Dictionary<string, string> _resources = new Godot.Collections.Dictionary<string, string>();

    /// <summary>
    /// A list of packs loaded as resources
    /// </summary>

    [Export] Godot.Collections.Array<string> _resourcePacks = new Godot.Collections.Array<string>() { "pingod.gfx.pck", "pingod.snd.pck" };

    /// <summary>
    /// Working directory to load pingod.gfx.pck
    /// </summary>
    public static string WorkingDirectory = string.Empty;

    /// <summary>
    /// Loads gfx resource packs from a packs directory.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();

        if (!Engine.EditorHint)
        {
            if(_resourcePacks?.Count > 0)
            {
                var exePath = OS.GetExecutablePath();
                WorkingDirectory = System.IO.Path.GetDirectoryName(exePath);
                foreach (var resourcePack in _resourcePacks)
                {
                    if (!LoadResourcePack("res://" + resourcePack))
                    {
                        LoadResourcePack(System.IO.Path.Combine(WorkingDirectory,resourcePack));
                    }
                }

                LoadResources();
            }                  
        }
    }

    /// <summary>
    /// Loads a resource pack with <see cref="ProjectSettings.LoadResourcePack"/>
    /// </summary>
    /// <param name="filePath"></param>
    private static bool LoadResourcePack(string filePath)
    {
        if (ProjectSettings.LoadResourcePack(filePath))
        {
            Logger.Info(nameof(Resources), "resource pack loaded:", filePath);
            return true; 
        }
        else { Logger.Warning(nameof(Resources),"failed to load resource pack: " + filePath); return false; }
    }

    /// <summary>
    /// Scans directory for *gfx.pck files and loads them as resource packs
    /// </summary>
    /// <param name="pckDir"></param>
    /// <returns></returns>
    private static bool LoadResourcePackDirectory(string pckDir)
    {
        var d = new Godot.Directory();
        if (d.DirExists(pckDir))
        {
            var packsDir = d.Open(pckDir);
            if (packsDir == Error.Ok)
            {
                Logger.Info(nameof(Resources), ":found packs directory");
                d.ListDirBegin(true);
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
    /// Invokes GD.Load on every resource found
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
}
