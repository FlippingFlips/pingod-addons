using Godot;

public partial class AdjustmentsScript : Node
{
    public Adjustments _adjustments;

    [Export] Resource _test;
    public override void _EnterTree()
    {
        base._EnterTree();
        Logger.Debug(nameof(AdjustmentsScript), ": loading standard adjustment model.");
        LoadAdjustments<Adjustments>();
        //Adjustments.Load();
    }

    public override void _ExitTree()
    {
        Adjustments.Save(_adjustments);
    }

    /// <summary>
    /// Loads or create adjustments. Override this to add custom
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="r"></param>
    public virtual void LoadAdjustments<T>() where T : Adjustments
    {
        Logger.Debug(nameof(AdjustmentsScript), ": ", nameof(LoadAdjustments));
        _adjustments = Adjustments.Load<T>();
    }

    /// <summary>
    /// Loads custom adjustments type as Adjustments
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="r"></param>
    public virtual void SaveAdjustments<T>() where T : Adjustments
    {
        Logger.Debug(nameof(AdjustmentsScript), ": ", nameof(SaveAdjustments));
        Adjustments.Save<T>((T)_adjustments);
    }
}
