namespace RayTracer;

public interface ICamera
{
    Ray GetRay(float s, float t);
}
