using System.Numerics;

namespace ConsoleRayTracer;

interface IEntity
{
    HitRecord? Hit(Ray ray, float tMin, float tMax) => null;
    float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity => 0F;
}

readonly record struct HitRecord(
    float T,
    Vector3 Point,
    Vector3 Normal,
    float Brightness = 1F,
    float Reflectance = 0F
);

readonly record struct Apply<E>(
    E Entity,
    Vector3 Offset = default,
    float Brightness = 1F,
    float Reflectance = 0F
) : IEntity
    where E : IEntity
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        Entity.Hit(ray with { Origin = ray.Origin - Offset }, tMin, tMax) is HitRecord record
            ? record with
            {
                Point = record.Point + Offset,
                Brightness = Brightness,
                Reflectance = Reflectance,
            }
            : null;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Entity.Illuminate(new Apply<I>(entity, -Offset), record with { Point = record.Point - Offset })
            * Brightness;
}
