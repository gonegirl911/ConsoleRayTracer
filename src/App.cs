using System.Diagnostics;

namespace ConsoleRayTracer;

sealed record App<D>(ICanvas Canvas, D Drawable)
    where D : IDrawable, IEventHandler
{
    public void Run()
    {
        var lastFrame = Stopwatch.GetTimestamp();
        while (true)
        {
            var now = Stopwatch.GetTimestamp();
            RunFrame(Stopwatch.GetElapsedTime(lastFrame, now));
            lastFrame = now;
        }
    }

    public void RunFrame(TimeSpan dt)
    {
        Drawable.Handle(Canvas.Refresh(), dt);
        Drawable.Draw(Canvas);
        Canvas.Commit();
    }
}
