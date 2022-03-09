namespace ConsoleRayTracer;

class App<T, R> where T : ITerminal where R : IRenderer
{
    private readonly T _terminal;
    private readonly R _renderer;

    public App(T terminal, R renderer)
    {
        _terminal = terminal;
        _renderer = renderer;
    }

    public void StartMainLoop(Action<App<T, R>, int> action)
    {
        for (var frame = 0; ; frame++)
        {
            action(this, frame);
        }
    }

    public void Render<H, C, L>(H hittable, C camera, L light)
        where H : IHittable
        where C : ICamera
        where L : ILight
    {
        Parallel.For(0, _terminal.Height, y =>
        {
            Parallel.For(0, _terminal.Width, x =>
            {
                var s = (float)x / _terminal.Width;
                var t = (float)y / _terminal.Height;
                var pixelColor = _renderer.RenderPixel(s, t, hittable, camera, light);
                _terminal.SetPixel(x, y, pixelColor);
            });
        });

        _terminal.Draw();
    }
}
