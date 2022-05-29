namespace ConsoleRayTracer;

public sealed record Scene<E, L, C>(E Entity, L Light, C Camera) : IDrawable
    where E : IEntity
    where L : IEntity
    where C : ICamera
{
    public void Draw<C, R>(in C canvas, in R renderer)
        where C : ICanvas
        where R : IRenderer
    {
        var scaleX = 1f / canvas.Width;
        var scaleY = 1f / canvas.Height;
        var rendererCopy = renderer;
        canvas.Draw((x, y) => rendererCopy.Color(this, x * scaleX, y * scaleY));
    }
}
