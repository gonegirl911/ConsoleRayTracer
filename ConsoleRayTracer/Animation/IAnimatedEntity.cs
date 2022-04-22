namespace ConsoleRayTracer;

interface IAnimatedEntity : IEntity
{
    void Update(float timeElapsed);
}

record Animated<E>(
    E Entity,
    IAnimation<Vector3>? Offset = null,
    IAnimation<float>? Brightness = null,
    IAnimation<float>? Reflectance = null
) : IAnimatedEntity
    where E : IEntity
{
    private Vector3 _offset = new(0f);
    private float _brightness = 1f;
    private float _reflectance = 0f;

    public HitRecord? Hit(in Ray ray) =>
        Entity.Hit(ray with { Origin = ray.Origin - _offset }) is HitRecord record
            ? record with { Point = record.Point + _offset, Brightness = _brightness, Reflectance = _reflectance }
            : null;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Entity.Illuminate(new Apply<I>(entity, -_offset), record with { Point = record.Point - _offset }) * _brightness;

    public void Update(float timeElapsed)
    {
        _offset = Offset?.GetValue(timeElapsed) ?? new(0f);
        _brightness = Brightness?.GetValue(timeElapsed) ?? 1f;
        _reflectance = Reflectance?.GetValue(timeElapsed) ?? 0f;
    }
}
