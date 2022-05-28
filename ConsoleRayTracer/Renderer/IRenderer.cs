namespace ConsoleRayTracer;

public interface IRenderer
{
    float Color<E, L, C>(Scene<E, L, C> scene, float s, float t)
        where E : IEntity
        where L : IEntity
        where C : ICamera;
}
