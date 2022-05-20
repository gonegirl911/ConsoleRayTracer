namespace RayTracer;

public interface ITerminal<R> where R : IRenderer
{
    int Width { get; }
    int Height { get; }
    R Renderer { get; }

    void Set(int x, int y, char ch);
    void Update();
}

public static class TerminalExtensions
{
    public static void Draw<T, R>(this T terminal, int x, int y, float color)
        where T : ITerminal<R>
        where R : IRenderer
    {
        const string ASCII = " .:+%#@";
        terminal.Set(x, y, ASCII[(int)(Math.Clamp(color, 0f, 1f) * ASCII.Length - 1e-12)]);
    }

    public static void Draw<T, R>(this T terminal, Func<int, int, char> pixel)
        where T : ITerminal<R>
        where R : IRenderer
    {
        Parallel.For(0, terminal.Height, y => Parallel.For(0, terminal.Width, x => terminal.Draw<T, R>(x, y, pixel(x, y))));
    }

    public static void Draw<T, R>(this T terminal, Func<int, int, float> pixelColor)
        where T : ITerminal<R>
        where R : IRenderer
    {
        Parallel.For(0, terminal.Height, y => Parallel.For(0, terminal.Width, x => terminal.Draw<T, R>(x, y, pixelColor(x, y))));
    }

    public static void Draw<T, R>(this T terminal, string? label)
        where T : ITerminal<R>
        where R : IRenderer
    {
        if (string.IsNullOrEmpty(label))
        {
            return;
        }

        const int borderWidth = 1;
        const int padding = 2;

        var width = label.Length + (borderWidth + padding) * 2;
        var height = 1 + (borderWidth + padding) * 2;
        var topLeftX = (terminal.Width - width) / 2;
        var topLeftY = (terminal.Height - height) / 2;

        for (var dx = 0; dx < width; dx++)
        {
            for (var dy = 0; dy < borderWidth; dy++)
            {
                terminal.Draw<T, R>(topLeftX + dx, topLeftY + dy, 1f);
                terminal.Draw<T, R>(topLeftX + dx, topLeftY + height - borderWidth + dy, 1f);
            }
        }

        for (var dy = borderWidth; dy < height - borderWidth; dy++)
        {
            for (var dx = 0; dx < borderWidth; dx++)
            {
                terminal.Draw<T, R>(topLeftX + dx, topLeftY + dy, 1f);
                terminal.Draw<T, R>(topLeftX + width - borderWidth + dx, topLeftY + dy, 1f);
            }
            for (var dx = 0; dx < padding; dx++)
            {
                terminal.Draw<T, R>(topLeftX + borderWidth + dx, topLeftY + dy, 0f);
                terminal.Draw<T, R>(topLeftX + width - borderWidth - padding + dx, topLeftY + dy, 0f);
            }
            for (var dx = 0; dx < label.Length; dx++)
            {
                if (dy == borderWidth + padding)
                {
                    terminal.Set(topLeftX + borderWidth + padding + dx, topLeftY + dy, label[dx]);
                }
                else
                {
                    terminal.Draw<T, R>(topLeftX + borderWidth + padding + dx, topLeftY + dy, 0f);
                }
            }
        }
    }

    public static void Draw<T, R, E, C>(this T terminal, E entity, C camera, Tutorial tutorial)
        where T : ITerminal<R>
        where R : IRenderer
        where E : IEntity
        where C : ICamera
    {
        var scaleX = 1f / terminal.Width;
        var scaleY = 1f / terminal.Height;
        terminal.Draw<T, R>((x, y) => terminal.Renderer.Color(entity, camera, x * scaleX, y * scaleY));
        terminal.Draw<T, R>(tutorial.Label);
    }
}
