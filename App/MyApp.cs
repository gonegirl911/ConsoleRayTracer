using System.Runtime.Versioning;

namespace App;

[SupportedOSPlatform("windows")]
class MyApp : App<WindowsTerminal, RayTracer>
{
    private readonly World _world = World.Default;
    private readonly Crosshair _crosshair = new();
    private readonly Tutorial _tutorial = new();

    protected override WindowsTerminal Canvas { get; } = new(95, 70, "ConsoleRayTracer");
    protected override RayTracer Renderer { get; } = new();

    protected override void OnFrameUpdate(float dt)
    {
        var key = WindowsTerminal.KeyPressed();
        Canvas.Refresh();
        _world.Update(key, dt, (float)Canvas.Width / Canvas.Height);
        _tutorial.Update(key);
    }

    protected override void OnFrameUpdated()
    {
        Canvas.Draw(_world, Renderer);
        Canvas.Draw(_crosshair, Renderer);
        Canvas.Draw(_tutorial, Renderer);
        Canvas.Commit();
    }
}
