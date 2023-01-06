using Godot;

public partial class PlungerLane : Node
{
    [Export] public bool _isEnabled = true;
    [Signal] public delegate void AutoPlungerFiredEventHandler();

    private PinGodGame pinGod;
    private PinGodMachine machine;
    private BallSaver ballSaver;        

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;

        if (!_isEnabled)
        {
            Logger.Info(nameof(PlungerLane), nameof(_EnterTree), ": removing plunger lane plugin");
            this.QueueFree();
            return;
        }

        Logger.Debug(nameof(PlungerLane), nameof(_EnterTree));

        if (HasNode("/root/PinGodGame"))
        {
            pinGod = GetNode<PinGodGame>("/root/PinGodGame");
        }
        if (HasNode("/root/Machine"))
        {
            machine = GetNode<PinGodMachine>("/root/Machine");
            ballSaver = machine._ballSaver;
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
            if(ballSaver?.IsBallSaveActive() ?? false) // || pinGod.IsMultiballRunning
            {
                AutoFire();
            }

            //if (pinGod != null)
            //{
            //    //auto plunge the ball if in ball save or game is tilted to get the balls back
            //    if (pinGod.BallSaveActive || pinGod.IsMultiballRunning)
            //    {
            //        AutoFire();
            //    }
            //}
            //else if(machine != null)
            //{
            //    Logger.Verbose(nameof(PlungerLane), ": plunger lane switch active. No pinGod game running.");
            //}
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
                        
                        ballSaver.StartSaver();
                        //TODO: set ball saver on here
                        //var saveStarted = StartSaver(TroughOptions.BallSaveSeconds);
                        //if (saveStarted)
                        //{
                        //    pinGod.EmitSignal(nameof(PinGodGame.BallSaveStarted));
                        //}
                    }
                }
            }
        }
    }

    public void AutoFire()
    {
        if (IsSwitchActive())
        {
            machine?.CoilPulse(_auto_plunge_solenoid);            
            EmitSignal(nameof(AutoPlungerFired));
            Logger.Debug(nameof(PlungerLane), ":auto plunger saved");
        }
        else { Logger.Debug(nameof(PlungerLane), ": can't AutoFire when plunger lane is inactive."); }
    }

    public bool IsSwitchActive() => Machine.IsSwitchOn((_plunger_lane_switch));
}
