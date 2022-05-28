namespace ConsoleRayTracer;

public interface IDrawable
{
    void Draw<T, R>(in T Terminal) where T : ITerminal<R> where R : IRenderer;
}
