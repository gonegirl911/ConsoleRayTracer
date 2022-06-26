namespace ConsoleRayTracer;

public interface IDrawable
{
    void Draw<C>(C canvas) where C : class, ICanvas;
}
