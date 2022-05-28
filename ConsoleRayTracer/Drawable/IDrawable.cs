namespace ConsoleRayTracer;

public interface IDrawable
{
    void Draw<C, R>(in C canvas, in R renderer)
        where C : ICanvas
        where R : IRenderer;
}
