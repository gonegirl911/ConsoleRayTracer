using System.Diagnostics;

namespace RayTracer;

public record App<T, R>(T Terminal) where T : ITerminal<R> where R : IRenderer
{
    public void StartMainLoop(Action<T, float> action)
    {
        var stopwatch = Stopwatch.StartNew();
        var lastFrame = 0L;
        while (true)
        {
            var now = stopwatch.ElapsedMilliseconds;
            var dt = now - lastFrame;
            lastFrame = now;
            action(Terminal, dt);
            Terminal.Update();
        }
    }
}

public readonly record struct AppConfig<T, R>(int Width, int Height)
    where T : ITerminal<R>
    where R : IRenderer;
