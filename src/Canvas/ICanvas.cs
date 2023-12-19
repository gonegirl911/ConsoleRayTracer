namespace ConsoleRayTracer;

interface ICanvas
{
    int Width { get; }
    int Height { get; }

    Event? Refresh();

    void Set(int x, int y, char ch);

    void Set(int x, int y, float color)
    {
        const string ASCII = " .:+%#@";

        Set(x, y, ASCII[(int)float.Round(float.Clamp(color, 0f, 1f) * (ASCII.Length - 1))]);
    }

    void Set(Func<int, int, float> color)
    {
        Parallel.For(0, Height, y => Parallel.For(0, Width, x => Set(x, y, color(x, y))));
    }

    void Commit();
}

static class NativeTerminal
{
    public static ICanvas Get(int width, int height, string title)
    {
        return OperatingSystem.IsWindows()
            ? new WindowsTerminal(width, height, title)
            : throw new InvalidOperationException("unsupported platform, expected one of \"Windows\"");
    }
}
