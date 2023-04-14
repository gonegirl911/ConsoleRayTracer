namespace ConsoleRayTracer;

interface ICamera
{
    Ray CastRay(float s, float t);
}
