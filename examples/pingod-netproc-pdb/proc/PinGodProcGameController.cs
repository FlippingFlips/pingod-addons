using NetProc.Domain;
using NetProc.Domain.PinProc;
using NetProc.Game;

public class PinGodProcGameController : GameController
{
    public readonly IFakeProcDevice ProcFake;

    public PinGodProcGameController(MachineType machineType, ILogger logger, bool simulated = false) : base(machineType, logger, simulated) 
    {
        ProcFake = PROC as IFakeProcDevice;
    }
}
