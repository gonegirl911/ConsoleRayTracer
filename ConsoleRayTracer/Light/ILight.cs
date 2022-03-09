namespace ConsoleRayTracer;

interface ILight
{
    float Illuminate<H>(in H hittable, in HitRecord record) where H : IHittable;
}
