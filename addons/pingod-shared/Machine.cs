using System.Collections.Generic;

/// <summary>
/// Static machine class which holds the machine item <see cref="PinStates"/> like lamps, leds, coils, switches 
/// </summary>
public static class Machine
{
    /// <summary>
    /// Coil Pin States
    /// </summary>
    public static readonly Coils Coils = new Coils() { };    
    /// <summary>
    /// Lamp Pin States
    /// </summary>
    public static readonly Lamps Lamps = new Lamps() { };
    /// <summary>
    /// LED Pin States
    /// </summary>
    public static readonly Leds Leds = new Leds() { };
    /// <summary>
    /// Switch Pin States
    /// </summary>
    public static readonly Switches Switches = new Switches() { };

    public static void SetCoil(string name, byte state) => Coils[name].State = state;
    public static void SetLamp(string name, byte state) => Lamps[name].State = state;
    public static void SetLed(string name, int color) => Leds[name].Color = color;
    public static void SetSwitch(string name, byte state) => Switches[name].SetSwitch(state>0);

    public static void DisableAllLamps()
    {
        if (Lamps?.Count > 0)
        {
            foreach (var lamp in Lamps)
            {
                lamp.Value.State = 0;
            }
        }
    }
    public static void DisableAllLeds()
    {
        if (Leds?.Count > 0)
        {
            foreach (var led in Leds)
            {
                led.Value.State = 0;
            }
        }
    }

    /// <summary>
    /// <param name="name"></param>
    /// <returns>True id <see cref="Switch.IsEnabled"/></returns>
    public static bool IsSwitchOn(string name) => Switches[name].IsEnabled;
}

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Coils : PinStates { }

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Switches : Dictionary<string, Switch> { }

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Lamps : PinStates { }

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Leds : PinStates { }