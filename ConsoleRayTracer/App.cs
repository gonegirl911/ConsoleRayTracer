namespace ConsoleRayTracer;

readonly struct App<T, R> where T : ITerminal where R : IRenderer
{
    private readonly Window<T, R> _window;

    public App(T terminal, R renderer)
    {
        _window = new(terminal, renderer);
    }

    public void StartMainLoop(Action<Window<T, R>, float> action)
    {
        var lastFrame = DateTime.Now;
        while (true)
        {
            var now = DateTime.Now;
            var dt = now - lastFrame;
            lastFrame = now;
            action(_window, (float)dt.TotalMilliseconds);
        }
    }
}

readonly struct Window<T, R> where T : ITerminal where R : IRenderer
{
    private readonly T _terminal;
    private readonly R _renderer;

    public Window(T terminal, R renderer)
    {
        _terminal = terminal;
        _renderer = renderer;
    }

    public void Draw<H, C, L>(H hittable, C camera, L light)
        where H : IHittable
        where C : ICamera
        where L : ILight
    {
        var window = this;
        Parallel.For(0, _terminal.Height, y =>
        {
            Parallel.For(0, window._terminal.Width, x =>
            {
                var s = (float)x / window._terminal.Width;
                var t = (float)y / window._terminal.Height;
                var pixelColor = window._renderer.PixelColor(s, t, hittable, camera, light);
                window._terminal.SetPixel(x, y, pixelColor);
            });
        });

        _terminal.Draw();
    }

    public ConsoleKey KeyPressed() => _terminal.KeyPressed();
}
