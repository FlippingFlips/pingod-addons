using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class PlayfieldControl : Control
{
	[Export] Json machinejson;

    /// <summary>20.25x42" default</summary>
    [Export] Vector2 playfieldSizeMm = new Vector2(514.350f, 1066.800f);

    [Signal] public delegate void switch_activeEventHandler(string name, byte state);

    List<Button> _troughBtns;

    [Export] bool skipNotUsedSwitches = true;

    Control _buttonsCanvas;
    ProcMachineSwitches SwitchConfig;    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if(machinejson != null)
		{
			var json = Json.Stringify(machinejson.Data);
            SwitchConfig = JsonSerializer.Deserialize<ProcMachineSwitches>(json);

            _troughBtns = new List<Button>();

			if(SwitchConfig?.PRSwitches?.Any() ?? false )
			{
				_buttonsCanvas = GetNode<Control>("Buttons");

                var troughButton = GetNode<Button>("AllTroughButton");
                troughButton.Pressed += _on_trough_pressed;

                try
				{
                    foreach (var item in SwitchConfig.PRSwitches)
                    {
                        if (skipNotUsedSwitches && item.Name.Contains("not_used"))
                            continue;

                        var btn = new Button()
                        {
                            Name = item.Name,
                            ToggleMode = true,
                            TooltipText = item.Name,
                            Size = new Vector2(15, 15)
                        };

                        if (item.XPos.HasValue && item.YPos.HasValue)
                        {
                            var posX = Mathf.Remap(item.XPos.Value, 0, playfieldSizeMm.X, 0, this.Size.X);
                            var posY = Mathf.Remap(item.YPos.Value, 0, playfieldSizeMm.Y, 0, this.Size.Y);

                            btn.Position = new Vector2(posX + (btn.Size.X / 2), posY + (btn.Size.Y));
                        }
                        
                        _buttonsCanvas.AddChild(btn);

                        btn.Connect("pressed", Callable.From(() => _on_pressed(btn)));

                        var name = btn.Name.ToString();
                        if (name.StartsWith("trough"))
                            _troughBtns.Add(btn);                            


                    }
                }
				catch (System.Exception ex)
				{
                    Logger.Error(ex.ToString());    
				}
			}
        }
	}

    /// <summary>sends 0 or 1 with the switch name depending on if the button is pressed</summary>
    /// <param name="button"></param>
    void _on_pressed(Button button) => 
        EmitSignal(nameof(switch_active), button.Name, button.ButtonPressed ? 1 : 0);

    /// <summary>sends a switch state of 2</summary>
    /// <param name="button"></param>
    void _on_pulse_pressed(Button button) => EmitSignal(nameof(switch_active), button.Name, 2);

    /// <summary>Sends an <see cref="InputEventAction"/> to reset the game</summary>
    void _on_reset_pressed()
    {
        var evt = new InputEventAction();
        evt.Action = "reset";
        evt.Pressed = true;
        Input.ParseInputEvent(evt);
    }

    /// <summary>sends switch named _record and the index</summary>
    /// <param name="index"></param>
    void _on_record_item_selected(int index) => EmitSignal(nameof(switch_active), "_record", index);

    void _on_trough_pressed()
    {
        foreach (var item in _troughBtns)
        {
            item.SetPressedNoSignal(true);
            EmitSignal(nameof(switch_active), item.Name, 1);
        }
    }
}
