namespace ConsoleRayTracer;

public interface ITerminal<R> where R : IRenderer
{
    int Width { get; }
    int Height { get; }
    R Renderer { get; }

    void Set(int x, int y, char ch);
}

public static class TerminalExtensions
{
    public delegate char CharacterCalculator<R>(in R renderer, int x, int y);
    public delegate float ColorCalculator<R>(in R renderer, int x, int y);

    public static void Draw<T, R>(this T terminal, int x, int y, float color)
        where T : ITerminal<R>
        where R : IRenderer
    {
        const string ASCII = " .:+%#@";
        terminal.Set(x, y, ASCII[(int)(Math.Clamp(color, 0f, 1f) * ASCII.Length - 1e-12)]);
    }

    public static void Draw<T, R>(this T terminal, CharacterCalculator<R> ch)
        where T : ITerminal<R>
        where R : IRenderer
    {
        Parallel.For(0, terminal.Height, y =>
        {
            Parallel.For(0, terminal.Width, x =>
            {
                terminal.Draw<T, R>(x, y, ch(terminal.Renderer, x, y));
            });
        });
    }

    public static void Draw<T, R>(this T terminal, ColorCalculator<R> color)
        where T : ITerminal<R>
        where R : IRenderer
    {
        Parallel.For(0, terminal.Height, y =>
        {
            Parallel.For(0, terminal.Width, x =>
            {
                terminal.Draw<T, R>(x, y, color(terminal.Renderer, x, y));
            });
        });
    }

    public static void Draw<T, R, D>(this T terminal, in D drawable)
        where T : ITerminal<R>
        where R : IRenderer
        where D : IDrawable
    {
        drawable.Draw<T, R>(terminal);
    }
}
