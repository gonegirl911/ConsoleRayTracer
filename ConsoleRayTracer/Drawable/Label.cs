namespace ConsoleRayTracer;

public readonly record struct Label(string Text, int BorderWidth, int Padding) : IDrawable
{
    public void Draw<C, R>(C canvas, R renderer)
        where C : class, ICanvas
        where R : class, IRenderer
    {
        var width = Text.Length + (BorderWidth + Padding) * 2;
        var height = 1 + (BorderWidth + Padding) * 2;
        var topLeftX = (canvas.Width - width) / 2;
        var topLeftY = (canvas.Height - height) / 2;

        if (canvas.Width < width || canvas.Height < height)
        {
            return;
        }

        for (var dx = 0; dx < width; dx++)
        {
            for (var dy = 0; dy < BorderWidth; dy++)
            {
                canvas.Set(topLeftX + dx, topLeftY + dy, 1f);
                canvas.Set(topLeftX + dx, topLeftY + height - BorderWidth + dy, 1f);
            }
        }
        for (var dy = BorderWidth; dy < height - BorderWidth; dy++)
        {
            for (var dx = 0; dx < BorderWidth; dx++)
            {
                canvas.Set(topLeftX + dx, topLeftY + dy, 1f);
                canvas.Set(topLeftX + width - BorderWidth + dx, topLeftY + dy, 1f);
            }
            for (var dx = 0; dx < Padding; dx++)
            {
                canvas.Set(topLeftX + BorderWidth + dx, topLeftY + dy, 0f);
                canvas.Set(topLeftX + width - BorderWidth - Padding + dx, topLeftY + dy, 0f);
            }
            for (var dx = 0; dx < Text.Length; dx++)
            {
                if (dy == BorderWidth + Padding)
                {
                    canvas.Set(topLeftX + BorderWidth + Padding + dx, topLeftY + dy, Text[dx]);
                }
                else
                {
                    canvas.Set(topLeftX + BorderWidth + Padding + dx, topLeftY + dy, 0f);
                }
            }
        }
    }
}
