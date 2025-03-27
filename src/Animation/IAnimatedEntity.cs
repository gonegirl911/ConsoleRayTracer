using System.Numerics;

namespace ConsoleRayTracer;

interface IAnimatedEntity : IEntity
{
    void Update(float timeElapsed);
}

sealed class Animated<E, O, B, R>(
    E entity,
    O offset = default!,
    B brightness = default!,
    R reflectance = default!
) : IAnimatedEntity
    where E : IEntity
    where O : IAnimation<Vector3>
    where B : IAnimation<float>
    where R : IAnimation<float>
{
    Vector3 _offset = offset.GetValue(0F);
    float _brightness = 1F + brightness.GetValue(0F);
    float _reflectance = reflectance.GetValue(0F);

    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        entity.Hit(ray with { Origin = ray.Origin - _offset }, tMin, tMax) is HitRecord record
            ? record with
            {
                Point = record.Point + _offset,
                Brightness = record.Brightness + _brightness,
                Reflectance = record.Reflectance + _reflectance,
            }
            : null;

    public float Illuminate<T>(in T target, in HitRecord record) where T : IEntity =>
        entity.Illuminate(new Apply<T>(target, -_offset), record with { Point = record.Point - _offset })
            * _brightness;

    public void Update(float timeElapsed)
    {
        _offset = offset.GetValue(timeElapsed);
        _brightness = 1F + brightness.GetValue(timeElapsed);
        _reflectance = reflectance.GetValue(timeElapsed);
    }
}
