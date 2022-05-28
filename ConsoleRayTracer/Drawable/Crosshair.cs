namespace ConsoleRayTracer;

public readonly record struct Crosshair : IDrawable
{
    public void Draw<C, R>(in C canvas, in R renderer)
        where C : ICanvas
        where R : IRenderer
    {
        if (canvas.Width < 3 || canvas.Height < 3)
        {
            return;
        }

        var centerX = canvas.Width / 2;
        var centerY = canvas.Height / 2;
        canvas.Set(centerX - 1, centerY, 1f);
        canvas.Set(centerX, centerY - 1, 1f);
        canvas.Set(centerX + 1, centerY, 1f);
        canvas.Set(centerX, centerY + 1, 1f);
        canvas.Set(centerX, centerY, 1f);
    }
}
