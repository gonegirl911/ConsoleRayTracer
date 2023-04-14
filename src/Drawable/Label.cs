namespace ConsoleRayTracer;

readonly record struct Label(string Text) : IDrawable
{
    const int PADDING = 2;
    const int OUTLINE = 1;

    public void Draw<C>(C canvas) where C : class, ICanvas<C>
    {
        var width = Text.Length + (PADDING + OUTLINE) * 2;
        var height = 1 + (PADDING + OUTLINE) * 2;

        if (canvas.Width < width || canvas.Height < height)
        {
            return;
        }

        var topLeftX = (canvas.Width - width) / 2;
        var topLeftY = (canvas.Height - height) / 2;

        for (var dx = 0; dx < width; dx++)
        {
            for (var dy = 0; dy < OUTLINE; dy++)
            {
                canvas.Set(topLeftX + dx, topLeftY + dy, 1f);
                canvas.Set(topLeftX + dx, topLeftY + height - OUTLINE + dy, 1f);
            }
        }

        for (var dy = OUTLINE; dy < height - OUTLINE; dy++)
        {
            for (var dx = 0; dx < PADDING; dx++)
            {
                canvas.Set(topLeftX + OUTLINE + dx, topLeftY + dy, 0f);
                canvas.Set(topLeftX + width - PADDING - OUTLINE + dx, topLeftY + dy, 0f);
            }

            for (var dx = 0; dx < OUTLINE; dx++)
            {
                canvas.Set(topLeftX + dx, topLeftY + dy, 1f);
                canvas.Set(topLeftX + width - OUTLINE + dx, topLeftY + dy, 1f);
            }

            for (var dx = 0; dx < Text.Length; dx++)
            {
                if (dy == PADDING + OUTLINE)
                {
                    canvas.Set(topLeftX + PADDING + OUTLINE + dx, topLeftY + dy, Text[dx]);
                }
                else
                {
                    canvas.Set(topLeftX + PADDING + OUTLINE + dx, topLeftY + dy, 0f);
                }
            }
        }
    }
}
