namespace ConsoleRayTracer;

public interface IDrawable
{
    void Draw<C, R>(C canvas, R renderer)
        where C : class, ICanvas
        where R : class, IRenderer;
}
