using System.Diagnostics;

namespace RayTracer;

class App<T, R> where T : ITerminal where R : IRenderer
{
    private readonly Window<T, R> _window;

    public App(T terminal, R renderer)
    {
        _window = new(terminal, renderer);
    }

    public void StartMainLoop(Action<Window<T, R>, float> action)
    {
        var stopwatch = Stopwatch.StartNew();
        var lastFrame = 0L;
        while (true)
        {
            var now = stopwatch.ElapsedMilliseconds;
            var dt = now - lastFrame;
            lastFrame = now;
            action(_window, dt);
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

    public void Draw<E, C>(E entity, C camera)
        where E : IEntity
        where C : ICamera
    {
        var scaleX = 1f / _terminal.Width;
        var scaleY = 1f / _terminal.Height;

        Parallel.For(0, _terminal.Height, y =>
        {
            Parallel.For(0, _terminal.Width, x =>
            {
                var s = x * scaleX;
                var t = y * scaleY;
                var pixelColor = _renderer.PixelColor(s, t, entity, camera);
                _terminal.SetPixel(x, y, pixelColor);
            });
        });

        _terminal.Draw();
    }

    public ConsoleKey? KeyPressed() => _terminal.KeyPressed();
}
