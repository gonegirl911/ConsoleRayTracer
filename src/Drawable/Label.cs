namespace ConsoleRayTracer;

readonly struct Label(string text) : IDrawable
{
    const int PADDING = 2;
    const int OUTLINE = 1;

    public void Draw(ICanvas canvas)
    {
        var width = text.Length + (PADDING + OUTLINE) * 2;
        var height = 1 + (PADDING + OUTLINE) * 2;
        if (width > canvas.Width || height > canvas.Height)
        {
            return;
        }

        var topLeftX = (canvas.Width - width) / 2;
        var topLeftY = (canvas.Height - height) / 2;
        for (var dx = 0; dx < width; ++dx)
        {
            for (var dy = 0; dy < OUTLINE; ++dy)
            {
                canvas.Set(topLeftX + dx, topLeftY + dy, 1F);
                canvas.Set(topLeftX + dx, topLeftY + height - OUTLINE + dy, 1F);
            }
        }
        for (var dy = OUTLINE; dy < height - OUTLINE; ++dy)
        {
            for (var dx = 0; dx < PADDING; ++dx)
            {
                canvas.Set(topLeftX + OUTLINE + dx, topLeftY + dy, 0F);
                canvas.Set(topLeftX + width - PADDING - OUTLINE + dx, topLeftY + dy, 0F);
            }
            for (var dx = 0; dx < OUTLINE; ++dx)
            {
                canvas.Set(topLeftX + dx, topLeftY + dy, 1F);
                canvas.Set(topLeftX + width - OUTLINE + dx, topLeftY + dy, 1F);
            }
            for (var dx = 0; dx < text.Length; ++dx)
            {
                if (dy == PADDING + OUTLINE)
                {
                    canvas.Set(topLeftX + PADDING + OUTLINE + dx, topLeftY + dy, text[dx]);
                }
                else
                {
                    canvas.Set(topLeftX + PADDING + OUTLINE + dx, topLeftY + dy, 0F);
                }
            }
        }
    }
}
