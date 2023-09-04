using Godot;
using System;

public partial class MatrixSwitchesGridContainer : GridContainer
{
	private PinGodGameProc _pinGodProcGame;

	public override void _Ready()
	{
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
		
		var scene =
			ResourceLoader.Load<PackedScene>("res://scenes/ServiceMode/Shared/MatrixItemPanel.tscn");

		byte boardNum = 0;
		//draw top column rows
		for (int i = 0; i < (8 * 16); i++)
		{
			var newScene = scene.Instantiate() as MatrixItemPanel;

			//newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["column"]);

			var switchNum = i;

			this.AddChild(newScene);
			if (_pinGodProcGame?.PinGodProcGame.Switches.ContainsKey((ushort)switchNum) ?? false)
			{
				var sw = _pinGodProcGame.PinGodProcGame.Switches[(ushort)switchNum];
				newScene.SetName(sw.Name);

				if (sw.Type == NetProc.Domain.PinProc.SwitchType.NO && sw.IsClosed())
				{
					newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
				}
				else
				{
					newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_no"]);
				};

				//else if (sw.Type == NetProc.Domain.PinProc.SwitchType.NC && !sw.IsClosed())
				//{
				//                   newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
				//               }
				//else
				//{
				//                   newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["inactive"]);
				//               }
			}
			else
			{
				newScene.SetName("UNUSED");
			}

			newScene.SetNum(switchNum);

			//column squares 17 rows
			//if (i % 17 == 0)
			//{
			//	var res = PinballMatrixConstants.BackgroundColours["column"];
			//	newScene.ChangePanelBackgroundColour(res);
			//	this.AddChild(newScene);

			//	newScene.SetName("Board:" + boardNum);
			//	newScene.SetNum(boardNum);

			//	boardNum++;
			//}
			//else if (i > 16)
			//{


			//}
			//else
			//{
			//	var res = PinballMatrixConstants.BackgroundColours["column"];
			//	newScene.ChangePanelBackgroundColour(res);
			//	this.AddChild(newScene);

			//	if(i > 0)
			//	{
			//		if (i < 9)
			//		{
			//			newScene.SetName("BANK_A");
			//			newScene.SetNum(i-1);
			//		}
			//		else
			//		{
			//			newScene.SetName("BANK_B");
			//			newScene.SetNum(i - 9);
			//		}
			//	}				
			//}
		}
	}

	public void OnSwitch(string swName, ushort swNum, bool isClosed)
	{		
		UpdateSwitch(swNum);
	}

	private void UpdateSwitch(ushort swNum)
	{
		if (_pinGodProcGame.PinGodProcGame.Switches.ContainsKey(swNum))
		{
			var item = this.GetChild(swNum) as MatrixItemPanel;

			var sw = _pinGodProcGame.PinGodProcGame.Switches[swNum];

			if (sw.Type == NetProc.Domain.PinProc.SwitchType.NO && sw.IsClosed())
			{
				item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
			}
			else
			{
				item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_no"]);
			};
		}                
	}
}
