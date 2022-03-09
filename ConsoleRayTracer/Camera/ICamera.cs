namespace ConsoleRayTracer;

interface ICamera
{
    Ray GetRay(float s, float t);
}
