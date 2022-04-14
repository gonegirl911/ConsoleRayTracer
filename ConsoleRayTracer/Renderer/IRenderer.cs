namespace ConsoleRayTracer;

interface IRenderer
{
    float PixelColor<E, L, C>(float s, float t, in E entity, in L light, in C camera)
        where E : IEntity
        where L : IEntity
        where C : ICamera;
}
