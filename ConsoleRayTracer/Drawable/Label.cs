namespace ConsoleRayTracer;

public readonly record struct Label(string Text, int BorderWidth, int Padding) : IDrawable
{
    public void Draw<T, R>(in T terminal) where T : ITerminal<R> where R : IRenderer
    {
        var width = Text.Length + (BorderWidth + Padding) * 2;
        var height = 1 + (BorderWidth + Padding) * 2;
        var topLeftX = (terminal.Width - width) / 2;
        var topLeftY = (terminal.Height - height) / 2;

        for (var dx = 0; dx < width; dx++)
        {
            for (var dy = 0; dy < BorderWidth; dy++)
            {
                terminal.Draw<T, R>(topLeftX + dx, topLeftY + dy, 1f);
                terminal.Draw<T, R>(topLeftX + dx, topLeftY + height - BorderWidth + dy, 1f);
            }
        }

        for (var dy = BorderWidth; dy < height - BorderWidth; dy++)
        {
            for (var dx = 0; dx < BorderWidth; dx++)
            {
                terminal.Draw<T, R>(topLeftX + dx, topLeftY + dy, 1f);
                terminal.Draw<T, R>(topLeftX + width - BorderWidth + dx, topLeftY + dy, 1f);
            }
            for (var dx = 0; dx < Padding; dx++)
            {
                terminal.Draw<T, R>(topLeftX + BorderWidth + dx, topLeftY + dy, 0f);
                terminal.Draw<T, R>(topLeftX + width - BorderWidth - Padding + dx, topLeftY + dy, 0f);
            }
            for (var dx = 0; dx < Text.Length; dx++)
            {
                if (dy == BorderWidth + Padding)
                {
                    terminal.Set(topLeftX + BorderWidth + Padding + dx, topLeftY + dy, Text[dx]);
                }
                else
                {
                    terminal.Draw<T, R>(topLeftX + BorderWidth + Padding + dx, topLeftY + dy, 0f);
                }
            }
        }
    }
}
