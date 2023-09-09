using Godot.Collections;

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
