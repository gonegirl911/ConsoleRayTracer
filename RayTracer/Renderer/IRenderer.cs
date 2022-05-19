namespace RayTracer;

interface IRenderer
{
    float PixelColor<E, C>(in E entity, in C camera, float s, float t)
        where E : IEntity
        where C : ICamera;
}
