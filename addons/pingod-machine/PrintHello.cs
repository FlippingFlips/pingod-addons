#if TOOLS
using Godot;

[Tool]
public partial class PrintHello : Button
{
    private void OnPrintHelloPressed()
    {
        GD.Print("Hello from the main screen plugin!");
    }
}
#endif