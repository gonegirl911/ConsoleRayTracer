namespace ConsoleRayTracer;

public record Scene<E, C>(E Entity, C Camera) : IDrawable
    where E : IEntity
    where C : ICamera
{
    public void Draw<T, R>(in T terminal) where T : ITerminal<R> where R : IRenderer
    {
        var scaleX = 1f / terminal.Width;
        var scaleY = 1f / terminal.Height;
        terminal.Draw((in R renderer, int x, int y) => renderer.Color(Entity, Camera, x * scaleX, y * scaleY));
    }
}
