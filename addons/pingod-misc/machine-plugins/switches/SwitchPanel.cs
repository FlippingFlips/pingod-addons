using Godot;
using PinGod.EditorPlugins;

public partial class SwitchPanel : Control
{
    private SwitchOverlay _switchOverlay;
    private Button _troughActivateBtn;

    public override void _Ready()
	{
		_switchOverlay = GetNodeOrNull<SwitchOverlay>("Panel/CenterContainer/VBoxContainer/"+nameof(SwitchOverlay));
        _troughActivateBtn = GetNodeOrNull<Button>("Panel/CenterContainer/VBoxContainer/" + nameof(Button));

        _troughActivateBtn.ButtonUp += _troughActivateBtn_ButtonUp;
    }

    private void _troughActivateBtn_ButtonUp()
    {
        _switchOverlay?.ActivateTrough();
    }
}
