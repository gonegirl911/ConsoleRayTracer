namespace ConsoleRayTracer;

class App<T, R> where T : ITerminal where R : IRenderer
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

class Window<T, R> where T : ITerminal where R : IRenderer
{
    private readonly T _terminal;
    private readonly R _renderer;

    public Window(T terminal, R renderer)
    {
        _terminal = terminal;
        _renderer = renderer;
    }

    public void Draw<E, C, L>(E entity, L light, C camera)
        where E : IEntity
        where L : IEntity
        where C : ICamera
    {
        Parallel.For(0, _terminal.Height, y =>
        {
            Parallel.For(0, _terminal.Width, x =>
            {
                var s = (float)x / _terminal.Width;
                var t = (float)y / _terminal.Height;
                var pixelColor = _renderer.PixelColor(s, t, entity, light, camera);
                _terminal.SetPixel(x, y, pixelColor);
            });
        });

        _terminal.Draw();
    }

    public ConsoleKey? KeyPressed() => _terminal.KeyPressed();
}
