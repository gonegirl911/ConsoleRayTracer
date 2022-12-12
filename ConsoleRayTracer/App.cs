using System.Diagnostics;

namespace ConsoleRayTracer;

public sealed record App<C, D>(C Canvas, D Drawable)
    where C : class, ICanvas<C>
    where D : IDrawable, IEventHandler
{
    public void Run()
    {
        var lastFrame = Stopwatch.GetTimestamp();
        while (true)
        {
            var now = Stopwatch.GetTimestamp();
            var dt = Stopwatch.GetElapsedTime(lastFrame, now);
            lastFrame = now;
            RunFrame((float)dt.TotalMilliseconds);
        }
    }

    public void RunFrame(float dt)
    {
        var ev = Canvas.Refresh();
        Drawable.Handle(ev, dt);
        if (ev?.Variant is not EventVariant.Resize)
        {
            Canvas.Draw(Drawable);
        }
        Canvas.Commit();
    }
}
