using System.Diagnostics;

namespace ConsoleRayTracer;

public sealed record App<C, D>(C Canvas, D Drawable)
    where C : class, ICanvas<C>
    where D : IDrawable, IEventHandler
{
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
        var ev = Canvas.Refresh();

        Drawable.Handle(ev, dt);
        if (ev?.Variant is not EventVariant.Resize)
        {
            Canvas.Draw(Drawable);
        }
        Canvas.Commit();
    }
}
