using System.Diagnostics;

namespace RayTracer;

record App<W, R>(W Window) where W : IWindow<R> where R : IRenderer
{
    public void StartMainLoop(Action<W, float> action)
    {
        var stopwatch = Stopwatch.StartNew();
        var lastFrame = 0L;
        while (true)
        {
            var now = stopwatch.ElapsedMilliseconds;
            var dt = now - lastFrame;
            lastFrame = now;
            action(Window, dt);
            Window.Update();
        }
    }
}
