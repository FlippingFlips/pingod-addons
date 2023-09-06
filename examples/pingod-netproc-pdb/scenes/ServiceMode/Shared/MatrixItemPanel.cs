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
			{ "opto_no", "f4c200"},
			{ "opto_nc", "866500"},
			{ "switch_no", "1c00c5"},
			{ "switch_nc", "0c0075"},
			{ "unused", "404040"},
		};
}

public partial class MatrixItemPanel : Panel
{
	private Label _nameLbl;
	private Label _numLbl;

	[Export] public string Name { get; set; }
	[Export] public int Number { get; set; } = -1;
	[Export] public Color WireColorL { get; set; }
	[Export] public string WireNameL { get; set; }
	[Export] public Color WireColorR { get; set; }
	[Export] public string WireNameR { get; set; }

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

		//GetNode<ColorRect>("ColorRect").Color = WireColorL;
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

	public void SetWireL(string colour, string name)
	{
		GetNode<ColorRect>("ColorRect").Color = Color.FromHtml(colour);
	}

	public void SetWireR(string colour, string name)
	{
		GetNode<ColorRect>("ColorRect2").Color = Color.FromHtml(colour);
	}
}
