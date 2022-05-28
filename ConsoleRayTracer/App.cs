using System.Diagnostics;

namespace ConsoleRayTracer;

public abstract class App<C, R>
    where C : ICanvas
    where R : IRenderer
{
    protected abstract C Canvas { get; }
    protected abstract R Renderer { get; }

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        var lastFrame = 0L;
        while (true)
        {
            var now = stopwatch.ElapsedMilliseconds;
            var dt = now - lastFrame;
            lastFrame = now;
            OnFrameUpdate(dt);
            OnFrameUpdated();
        }
    }

    public void RunOnce()
    {
        OnFrameUpdate(0f);
        OnFrameUpdated();
    }

    protected abstract void OnFrameUpdate(float dt);
    protected abstract void OnFrameUpdated();
}
