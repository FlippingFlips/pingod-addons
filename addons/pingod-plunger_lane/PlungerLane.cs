using Godot;

public partial class PlungerLane : Node
{
    private PinGodGame pinGod;
    private PinGodMachine machine;
    private BallSaver ballSaver;

    [Signal] public delegate void AutoPlungerFiredEventHandler();

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;

        Logger.Debug(nameof(PlungerLane), nameof(_EnterTree));

        if (HasNode("/root/PinGodGame"))
        {
            pinGod = GetNode<PinGodGame>("/root/PinGodGame");
        }
        if (HasNode("/root/Machine"))
        {
            machine = GetNode<PinGodMachine>("/root/Machine");
            machine.SwitchCommand += OnPlungerSwitchHandler;
        }
        if (GetParent().HasNode("BallSaver"))
        {
            ballSaver = GetParent().GetNode<BallSaver>(nameof(BallSaver));
            machine.SwitchCommand += OnPlungerSwitchHandler;
        }
    }

    private void OnPlungerSwitchHandler(string name, byte index, byte value)
    {
        if (name != _plunger_lane_switch) return;
        Logger.Debug(nameof(PlungerLane), nameof(OnPlungerSwitchHandler), $": {index}={value}");

        if (!pinGod?.GameInPlay ?? false) return;
        if (pinGod?.IsTilted ?? true) return;

        //switch on
        if (value > 0)
        {
            if (pinGod != null)
            {
                //auto plunge the ball if in ball save or game is tilted to get the balls back
                if (pinGod.BallSaveActive || pinGod.IsMultiballRunning)
                {
                    machine?.CoilPulse(_auto_plunge_solenoid);
                    Logger.Verbose(nameof(PlungerLane), ":auto plunger saved");
                    EmitSignal(nameof(AutoPlungerFired));
                }
            }
            else if(machine != null)
            {
                Logger.Verbose(nameof(PlungerLane), ": plunger lane switch active. No pinGod game running.");
            }
        }
        //switch off
        else
        {
            if (pinGod != null)
            {
                //start a ball saver if game in play
                if (pinGod.GameInPlay && !pinGod.BallStarted && !pinGod.IsTilted && !pinGod.IsMultiballRunning)
                {
                    if (_set_ball_started_on_plunger_lane)
                        pinGod.BallStarted = true;

                    if (_set_ball_save_on_plunger_lane)
                    {
                        ballSaver.StartBallSaver();
                        //TODO: set ball saver on here
                        //var saveStarted = StartBallSaver(TroughOptions.BallSaveSeconds);
                        //if (saveStarted)
                        //{
                        //    pinGod.EmitSignal(nameof(PinGodGame.BallSaveStarted));
                        //}
                    }
                }
            }
        }
    }

}
