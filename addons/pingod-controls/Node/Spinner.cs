using Godot;

public partial class Spinner : PinGodMachineNode
{
    [Export] string SwitchName;
    /// play sound on switch
    [Export] bool playsound;
    [Export] AudioStream _AudioStream = null;
    [Export] AudioStreamPlayer player = new();

    [Signal] public delegate void OnSpinnerActiveEventHandler(byte value);

    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
            base._EnterTree();

            if (string.IsNullOrEmpty(SwitchName))
            {
                Logger.Log(PinGod.Base.LogLevel.Error, Logger.BBColor.red,
                    "no switch set on the spinner");
                this.QueueFree();
            }

            if (playsound && _AudioStream != null)
            {
                player = new AudioStreamPlayer();
                player.Stream = _AudioStream;
                this.AddChild(player);
            }

            _machine.SwitchCommand += _machine_SwitchCommand;
        }        
    }

    /// <summary>Plays a sound when spinner active. emits signal both open/closed</summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void _machine_SwitchCommand(string name, byte index, byte value)
    {
        if (name == SwitchName) 
        {
            if (playsound && value > 0 && player?.Stream != null)
                player.Play();

            EmitSignal(nameof(OnSpinnerActive), value);
        }
    }
}
