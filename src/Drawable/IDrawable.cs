namespace ConsoleRayTracer;

interface IDrawable
{
    void Draw<C>(C canvas) where C : class, ICanvas<C>;
}
