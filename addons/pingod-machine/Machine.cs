using PinGod.Base;
using PinGod.Core;
using System.Collections.Generic;
using System.Linq;

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

    public static void SetAction(string name, byte state) => Switches[name].SetSwitch(state);

    public static void SetCoil(string name, byte state) => Coils[name].State = state;

    public static void SetCoil(string name, bool state) => Coils[name].State = (byte)(state == true ? 1 : 0);
    public static PinStateObject SetLamp(string name, byte state)
    {
        var lamp = Lamps[name];
        lamp.State = state;
        return lamp;
    }
    public static void SetLed(string name, int color) => Leds[name].Color = color;
    public static void SetSwitch(string name, byte state) => Switches[name].SetSwitch(state);

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
    /// <returns>True id <see cref="GodotSwitch.IsEnabled"/></returns>
    public static bool IsSwitchOn(string name) => Switches[name].IsEnabled();

    public static void SetLed(string name, byte state, System.Drawing.Color? colour)
    {
        var c = colour.HasValue ?
            System.Drawing.ColorTranslator.ToOle(colour.Value) : Leds[name].Color;
        SetLed(name, state, c);
    }

    public static void SetLed(string name, byte state, byte r, byte g, byte b)
    {
        var c = System.Drawing.Color.FromArgb(r, g, b);
        var ole = System.Drawing.ColorTranslator.ToOle(c);
        SetLed(name, state, ole);
    }

    public static PinStateObject SetLed(string name, byte state, int color)
    {
        if (Leds.ContainsKey(name))
        {
            var led = Leds[name];
            led.State = state;
            led.Color = color > 0 ? color : led.Color;
            return led;
        }

        Logger.Error($"No LED found for: {name}");
        return null;
    }

    /// <summary>
    /// Sets led from RGB
    /// </summary>
    /// <param name="name"></param>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    public static void SetLedState(string name, byte state, byte r, byte g, byte b)
    {
        var c = System.Drawing.Color.FromArgb(r, g, b);
        var ole = System.Drawing.ColorTranslator.ToOle(c);
        SetLed(name, state, ole);
    }
}

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Coils : PinStates { }

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Switches : Dictionary<string, Switch>
{
    public Switch GetSwitch(string name)
    {
        TryGetValue(name, out Switch sw);
        return sw;
    }

    public Switch GetSwitch(int num) => Values.FirstOrDefault(x => x.Num == num);
}

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Lamps : PinStates { }

/// <summary>
/// Collection of string, <see cref="PinStateObject"/>
/// </summary>
public partial class Leds : PinStates { }