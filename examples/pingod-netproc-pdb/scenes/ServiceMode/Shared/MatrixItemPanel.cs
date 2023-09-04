using Godot;
using Godot.Collections;
using System.Security;

public static class PinballMatrixConstants
{
	public static readonly Dictionary<string, string> BackgroundColours = 
		new Dictionary<string, string> 
		{
			{ "column", "00000000"},
			{ "active", "017f01"},
			{ "error", "bd0000"},
			{ "opto_no", "404104"},
			{ "opto_nc", "404104"},
			{ "switch_no", "150065"},
			{ "switch_nc", "00003e"},
			{ "unused", "404040"},
		};
}

public partial class MatrixItemPanel : Panel
{
	private Label _nameLbl;
	private Label _numLbl;

	[Export] public string Name { get; set; }
	[Export] public int Number { get; set; } = -1;

	private StyleBoxFlat _defaultStyle;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//get labels from scene tree
		_nameLbl = GetNode<Label>("%NameLabel");
		_numLbl  = GetNode<Label>("%NumLabel");

		if(!string.IsNullOrWhiteSpace(Name))
			SetName(Name);

		if (Number > -1)
			SetNum(Number);
	}

	public void SetName(string name) => _nameLbl.Text = name;
	public void SetNum(int num) => _numLbl.Text = num.ToString();

	/// <summary>
	/// Overrides the current themes panel styleBoxFlat BG Color
	/// </summary>
	/// <param name="htmlColor"></param>
	public void ChangePanelBackgroundColour(string htmlColor)
	{
		//get the stylebox & duplicate from this controls themes panel
		if (_defaultStyle == null)            
			_defaultStyle = GetThemeStylebox("panel") as StyleBoxFlat;

		var styleBox = _defaultStyle.Duplicate() as StyleBoxFlat;

		//print color to see what we have
		//GD.Print("background colour panel stylebox: ", styleBox.BgColor);

		//set color from html. Typical RGB values didn't work
		styleBox.BgColor = Color.FromHtml(htmlColor);

		//add the override
		this.AddThemeStyleboxOverride("panel", styleBox);
	}
}
