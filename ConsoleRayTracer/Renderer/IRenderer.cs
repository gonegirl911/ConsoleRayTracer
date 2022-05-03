namespace ConsoleRayTracer;

interface IRenderer
{
    float PixelColor<E, C>(float s, float t, in E entity, in C camera)
        where E : IEntity
        where C : ICamera;
}
