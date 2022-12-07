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
        Drawable.Handle(Canvas.Refresh(), dt);
        Canvas.Draw(Drawable);
        Canvas.Commit();
    }
}
