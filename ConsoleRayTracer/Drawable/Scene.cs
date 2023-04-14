namespace ConsoleRayTracer;

public sealed record Scene<E, L, C, R>(E Entity, L Light, C Camera, R Renderer) : IDrawable
    where E : IEntity
    where L : IEntity
    where C : ICamera
    where R : IRenderer<Scene<E, L, C, R>>
{
    public void Draw<V>(V canvas) where V : class, ICanvas<V>
    {
        var scaleX = 1f / canvas.Width;
        var scaleY = 1f / canvas.Height;
        canvas.Set((x, y) => Renderer.Render(this, x * scaleX, y * scaleY));
    }
}
