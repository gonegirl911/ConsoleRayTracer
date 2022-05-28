namespace ConsoleRayTracer;

public interface ICamera : IEntity
{
    Ray GetRay(float s, float t);
}
