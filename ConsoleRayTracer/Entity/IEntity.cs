namespace ConsoleRayTracer;

interface IEntity
{
    HitRecord? Hit(in Ray ray, float tMin, float tMax) => null;
    float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity => 0f;
}

readonly record struct HitRecord(
    float T,
    Vector3 Point,
    Vector3 Normal,
    float Brightness = 1f,
    float Reflectance = 0f
);

readonly record struct Apply<E>(
    E Entity,
    Vector3? Offset = null,
    float? Brightness = null,
    float? Reflectance = null
) : IEntity
    where E : IEntity
{
    private readonly Vector3 _offset = Offset ?? new(0f);
    private readonly float _brightness = Brightness ?? 1f;
    private readonly float _reflectance = Reflectance ?? 0f;

    public HitRecord? Hit(in Ray ray, float tMin, float tMax) =>
        Entity.Hit(ray with { Origin = ray.Origin - _offset }, tMin, tMax) is HitRecord record
            ? record with { Point = record.Point + _offset, Brightness = _brightness, Reflectance = _reflectance }
            : null;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Entity.Illuminate(new Apply<I>(entity, -_offset), record with { Point = record.Point - _offset }) * _brightness;
}
