using System.Diagnostics;

namespace ConsoleRayTracer;

public abstract class App<C, R>
    where C : ICanvas
    where R : IRenderer
{
    protected abstract C Canvas { get; }
    protected abstract R Renderer { get; }

    protected abstract void OnFrameUpdate(float dt);
    protected abstract void OnFrameUpdated();

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        var lastFrame = 0L;
        while (true)
        {
            var now = stopwatch.ElapsedMilliseconds;
            var dt = now - lastFrame;
            lastFrame = now;
            RunFrame(dt);
        }
    }

    public void RunFrame(float dt)
    {
        Canvas.Refresh();
        OnFrameUpdate(dt);
        OnFrameUpdated();
        Canvas.Commit();
    }
}
