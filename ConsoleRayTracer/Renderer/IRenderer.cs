namespace ConsoleRayTracer;

public interface IRenderer<T>
{
    float Trace(in T obj, float s, float t);
}
