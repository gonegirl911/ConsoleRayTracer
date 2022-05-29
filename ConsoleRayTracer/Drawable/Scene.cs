namespace ConsoleRayTracer;

public abstract record Scene<E, L, C>(E Entity, L Light, C Camera) : IDrawable
    where E : IEntity
    where L : IEntity
    where C : ICamera
{
    public virtual void Draw<C, R>(C canvas, R renderer)
        where C : class, ICanvas
        where R : class, IRenderer
    {
        var scaleX = 1f / canvas.Width;
        var scaleY = 1f / canvas.Height;
        canvas.Draw((x, y) => renderer.Color(this, x * scaleX, y * scaleY));
    }
}
