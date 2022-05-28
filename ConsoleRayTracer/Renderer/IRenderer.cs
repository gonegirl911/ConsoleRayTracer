namespace ConsoleRayTracer;

public interface IRenderer
{
    float Color<E, C>(in E entity, in C camera, float s, float t)
        where E : IEntity
        where C : ICamera;
}
