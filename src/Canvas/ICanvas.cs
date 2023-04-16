namespace ConsoleRayTracer;

interface ICanvas<TSelf> where TSelf : class, ICanvas<TSelf>
{
    int Width { get; }
    int Height { get; }

    Event? Refresh();
    void Set(int x, int y, float color);
    void Set(int x, int y, char ch);
    void Commit();
}

static class CanvasExtensions
{
    public static void Set<C>(this C canvas, Func<int, int, float> color)
        where C : class, ICanvas<C>
    {
        Parallel.For(0, canvas.Height, y => Parallel.For(0, canvas.Width, x => canvas.Set(x, y, color(x, y))));
    }
}
