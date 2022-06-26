using System.Diagnostics;

namespace ConsoleRayTracer;

public abstract class App<C, R, D>
    where C : class, ICanvas
    where D : IDrawable
{
    protected abstract C Canvas { get; }
    protected abstract D Drawable { get; }

    protected virtual void OnFrameUpdate(float dt) { }
    protected virtual void OnFrameUpdated() => Canvas.Draw(Drawable);

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
