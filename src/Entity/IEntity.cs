using System.Numerics;

namespace ConsoleRayTracer;

interface IEntity
{
    HitRecord? Hit(Ray ray, float tMin, float tMax) => null;
    float Illuminate<I>(in I target, in HitRecord record) where I : IEntity => 0F;
}

readonly record struct HitRecord(
    float T,
    Vector3 Point,
    Vector3 Normal,
    float Brightness = 1F,
    float Reflectance = 0F
);

readonly struct Apply<E>(
    E entity,
    Vector3 offset = default,
    float brightness = 1F,
    float reflectance = 0F
) : IEntity
    where E : IEntity
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        entity.Hit(ray with { Origin = ray.Origin - offset }, tMin, tMax) is HitRecord record
            ? record with
            {
                Point = record.Point + offset,
                Brightness = brightness,
                Reflectance = reflectance,
            }
            : null;

    public float Illuminate<T>(in T target, in HitRecord record) where T : IEntity =>
        entity.Illuminate(new Apply<T>(target, -offset), record with { Point = record.Point - offset })
            * brightness;
}
