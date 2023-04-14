namespace ConsoleRayTracer;

interface IRenderer<T>
{
    float Render(in T value, float s, float t);
}
