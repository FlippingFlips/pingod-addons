using Godot;
using NetProc.Domain.MachineConfig;
using System;
using System.Linq;

public partial class MatrixSwitchesGridContainer : GridContainer
{
	private PinGodGameProc _pinGodProcGame;

	public override void _Ready()
	{
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
		
		var scene =
			ResourceLoader.Load<PackedScene>("res://scenes/ServiceMode/Shared/MatrixItemPanel.tscn");

		byte boardNum = 0;
		int switchCount = _pinGodProcGame.PinGodProcGame.Switches.Count;
		//draw top column rows
		for (int i = 0; i < (8 * 16); i++)
		{
			var newScene = scene.Instantiate() as MatrixItemPanel;

			//newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["column"]);

			var switchNum = i;

			this.AddChild(newScene);

			if (i < switchCount && (_pinGodProcGame?.PinGodProcGame.Switches.ContainsKey((ushort)switchNum) ?? false))
			{
				
				//game switch
				var sw = _pinGodProcGame.PinGodProcGame.Switches[(ushort)switchNum];
				newScene.SetName(sw.Name);

				//config switch with more values
				var cfgSwitch = _pinGodProcGame.PinGodProcGame.Config.PRSwitches.FirstOrDefault(x => x.Name == sw.Name);

				//set switch state depending on the type. TODO: opto switches
				if (sw.Type == NetProc.Domain.PinProc.SwitchType.NO)
				{
					if(sw.IsClosed())
						newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
					else
					{
						if (cfgSwitch.ItemType == "opto") newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["opto_no"]);
						else newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_no"]);
					}                        
				}
				else if (sw.Type == NetProc.Domain.PinProc.SwitchType.NC)
				{
					if (sw.IsOpen())
						newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
					else
					{
                        if (cfgSwitch.ItemType == "opto") newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["opto_nc"]);
						else newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_nc"]);
					}                        
				}

				//TODO: set wire colour. wire colour isn't in database it is 3 charachter colour names like. BLK, BRN, BLU. 
				//newScene.SetWireL(cfgSwitch.WireLColour, cfgSwitch.WireLColourName);
				//newScene.SetWireR(cfgSwitch.WireRColour, cfgSwitch.WireRColourName);
			}
			else
			{
				newScene.SetName("UNUSED");
			}

			newScene.SetNum(switchNum);
			
		}
	}

	public void OnSwitch(string swName, ushort swNum, bool isClosed)
	{
		var cfgSwitch = _pinGodProcGame.PinGodProcGame.Config.PRSwitches.FirstOrDefault(x => x.Name == swName);
		UpdateSwitch(cfgSwitch);
	}

	private void UpdateSwitch(SwitchConfigFileEntry sw)
	{
		if (sw != null)
		{
			//get the game switch for it's state
			var gameSw = _pinGodProcGame.PinGodProcGame.Switches[sw.Name];

			//find the item in the grid
			var item = this.GetChild(gameSw.Number) as MatrixItemPanel;

			if (sw.Type == NetProc.Domain.PinProc.SwitchType.NO)
			{
				if (gameSw.IsClosed())
					item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
				else
				{
                    if (sw.ItemType == "opto") item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["opto_no"]);
					else item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_no"]);
				}
			}
			else if (sw.Type == NetProc.Domain.PinProc.SwitchType.NC)
			{
				if (gameSw.IsOpen())
					item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
				else
				{
                    if (sw.ItemType == "opto") item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["opto_nc"]);
					else item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_nc"]);
				}
			}
		}                
	}
}
