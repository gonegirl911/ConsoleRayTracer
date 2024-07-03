namespace ConsoleRayTracer;

readonly struct Crosshair : IDrawable
{
    public void Draw(ICanvas canvas)
    {
        if (canvas.Width < 3 || canvas.Height < 3)
        {
            return;
        }

        var centerX = canvas.Width / 2;
        var centerY = canvas.Height / 2;
        canvas.Set(centerX - 1, centerY, 1F);
        canvas.Set(centerX, centerY - 1, 1F);
        canvas.Set(centerX, centerY, 1F);
        canvas.Set(centerX, centerY + 1, 1F);
        canvas.Set(centerX + 1, centerY, 1F);
    }
}
