namespace ConsoleRayTracer;

interface IHittable
{
    HitRecord? Hit(in Ray ray, float tMin, float tMax);
}

readonly record struct HitRecord(
    float T,
    Vector3 Point,
    Vector3 Normal,
    bool FrontFace,
    float Brightness,
    float Reflection
);
