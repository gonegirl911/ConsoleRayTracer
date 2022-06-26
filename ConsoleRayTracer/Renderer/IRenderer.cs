namespace ConsoleRayTracer;

public interface IRenderer<Self> where Self : class, IRenderer<Self>
{
    float Color<E, L, C>(Scene<E, L, C, Self> scene, float s, float t)
        where E : IEntity
        where L : IEntity
        where C : ICamera;
}
