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

sealed class NativeTerminal : ICanvas
{
    readonly ICanvas _terminal;

    public NativeTerminal(int width, int height, string title)
    {
        _terminal = OperatingSystem.IsWindows()
            ? new WindowsTerminal(width, height, title)
            : throw new InvalidOperationException("unsupported platform, expected one of \"Windows\"");
    }

    public int Width => _terminal.Width;

    public int Height => _terminal.Height;

    public Event? Refresh() => _terminal.Refresh();

    public void Set(int x, int y, char ch)
    {
        _terminal.Set(x, y, ch);
    }

    public void Set(int x, int y, float color)
    {
        _terminal.Set(x, y, color);
    }

    public void Set(Func<int, int, float> color)
    {
        _terminal.Set(color);
    }

    public void Commit()
    {
        _terminal.Commit();
    }
}
