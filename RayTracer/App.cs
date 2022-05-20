using System.Diagnostics;

namespace RayTracer;

public record App<T, R>(T Terminal) where T : ITerminal<R> where R : IRenderer
{
    public static bool TryFrom(AppConfig config, Func<T> terminal, out App<T, R>? app)
    {
        if (config.Terminal == typeof(T) && config.Renderer == typeof(R))
        {
            app = new(terminal());
            return true;
        }
        app = null;
        return false;
    }

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

public readonly record struct AppConfig(
    Type Terminal,
    Type Renderer,
    int Width,
    int Height
);
