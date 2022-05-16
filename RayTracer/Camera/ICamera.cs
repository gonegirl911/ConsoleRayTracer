namespace RayTracer;

public interface ICamera : IEntity
{
    Ray GetRay(float s, float t, float aspectRatio);
}
