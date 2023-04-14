namespace ConsoleRayTracer;

public interface ICamera
{
    Ray CastRay(float s, float t);
}
