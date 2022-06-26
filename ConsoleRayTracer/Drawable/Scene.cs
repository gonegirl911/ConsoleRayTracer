namespace ConsoleRayTracer;

public abstract record Scene<E, L, C, R>(E Entity, L Light, C Camera, R Renderer) : IDrawable
    where E : IEntity
    where L : IEntity
    where C : ICamera
    where R : class, IRenderer<R>
{
    public virtual void Draw<C>(C canvas) where C : class, ICanvas
    {
        var scaleX = 1f / canvas.Width;
        var scaleY = 1f / canvas.Height;
        canvas.Draw((x, y) => Renderer.Color(this, x * scaleX, y * scaleY));
    }
}
