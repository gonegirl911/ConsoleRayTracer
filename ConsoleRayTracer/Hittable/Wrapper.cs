namespace ConsoleRayTracer;

readonly record struct Translate<H>(H Hittable, Vector3 Offset) : IHittable where H : struct, IHittable
{
    public HitRecord? Hit(in Ray ray, float tMin, float tMax) =>
        Hittable.Hit(ray with { Origin = ray.Origin - Offset }, tMin, tMax) is HitRecord record
            ? record with { Point = record.Point + Offset }
            : null;
}
