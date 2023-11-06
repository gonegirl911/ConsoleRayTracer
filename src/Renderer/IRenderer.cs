namespace ConsoleRayTracer;

interface IRenderer<T>
{
    float Render(in T obj, float s, float t);
}
