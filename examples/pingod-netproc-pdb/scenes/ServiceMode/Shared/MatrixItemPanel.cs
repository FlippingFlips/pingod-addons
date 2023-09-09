using Godot;

/// <summary>
/// This panel belongs to the scene MatrixItemPanel. This is to display a machine item with Name, Number and wire colours. <para/>
/// When in a switch matrix or similar then the background can be changed to show pushed switches.
/// </summary>
public partial class MatrixItemPanel : Panel
{
	private Label _nameLbl;
	private Label _numLbl;
	private ColorRect _wireL;
	private ColorRect _wireR;

	[Export] public string Name { get; set; }
	[Export] public int Number { get; set; } = -1;
	[Export] public Color WireColorL { get; set; }
	[Export] public string WireNameL { get; set; }
	[Export] public Color WireColorR { get; set; }
	[Export] public string WireNameR { get; set; }

	private StyleBoxFlat _defaultStyle;

	/// <summary>
	/// When scene enters first time
	/// </summary>
	public override void _Ready()
	{
		//get labels from scene tree
		_nameLbl = GetNode<Label>("%NameLabel");
		_numLbl  = GetNode<Label>("%NumLabel");

		//wire color boxes that contain a label
		_wireL = GetNode<ColorRect>("%WireLColorRect");
		_wireR = GetNode<ColorRect>("%WireRColorRect");

		if (!string.IsNullOrWhiteSpace(Name))
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

	public void SetWireL(string[] colours)
	{
		if (colours?.Length <= 0) return;

		//get first colour and set the colour
		var color = colours[0];
		_wireL.Color = ServiceModePinGod.ServiceModeColours[color].Color;
		_wireL.GetNode<Label>("Label").Text = color;
		_wireL.Visible = true;

		//add extra wire colour for a 2 colour wire
		if (colours.Length == 2)
		{
			var color2 = colours[1];
			var cRect = _wireL.GetNode<ColorRect>("ColorRect");
			_wireL.GetNode<Label>("Label").Text += $",{color2}";
			cRect.Visible = true;
			cRect.Color = ServiceModePinGod.ServiceModeColours[color2].Color;			
		}
	}

	public void SetWireR(string[] colours)
	{
		if (colours?.Length <= 0) return;

		//get first colour and set the colour
		var color = colours[0];
		_wireR.Color = ServiceModePinGod.ServiceModeColours[color].Color;
		_wireR.GetNode<Label>("Label").Text = color;
		_wireR.Visible = true;

		//add extra wire colour for a 2 colour wire
		if (colours.Length == 2)
		{
			var color2 = colours[1];
			var cRect = _wireR.GetNode<ColorRect>("ColorRect");
			cRect.Color = ServiceModePinGod.ServiceModeColours[color2].Color;
			_wireR.GetNode<Label>("Label").Text += $",{color2}";
			cRect.Visible = true;
		}
	}
}
