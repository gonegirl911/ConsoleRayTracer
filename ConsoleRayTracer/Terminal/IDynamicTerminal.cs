namespace ConsoleRayTracer;

public interface IDynamicTerminal<R> : ITerminal<R> where R : IRenderer
{
    void Update();
}
