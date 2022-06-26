using System.Runtime.Versioning;

namespace App;

[SupportedOSPlatform("windows")]
sealed class MyApp : App<WindowsTerminal, RayTracer, World>
{
    protected override WindowsTerminal Canvas { get; } = new(95, 70, "ConsoleRayTracer");
    protected override World Drawable { get; } = World.Default;

    protected override void OnFrameUpdate(float dt)
    {
        Canvas.Refresh();
        Drawable.Update(Win32.KeyPressed(), dt, (float)Canvas.Width / Canvas.Height);
    }
}
