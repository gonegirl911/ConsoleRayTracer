using System.Diagnostics;

namespace ConsoleRayTracer;

readonly struct App<D>(ICanvas canvas, D drawable) where D : IDrawable, IEventHandler
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
        drawable.Handle(canvas.Refresh(), dt);
        drawable.Draw(canvas);
        canvas.Commit();
    }
}
