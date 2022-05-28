namespace ConsoleRayTracer;

public interface ICamera
{
    Ray GetRay(float s, float t);
}
