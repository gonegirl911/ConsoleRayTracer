namespace ConsoleRayTracer;

interface IAnimatedEntity : IEntity
{
    void Update(float dt);
}

record Animated<E>(
    E Entity,
    IAnimation<Vector3>? Offset = null,
    IAnimation<float>? Brightness = null,
    IAnimation<float>? Reflectance = null
) : IAnimatedEntity
    where E : IEntity
{
    private float _timeElapsed = 0f;
    private Vector3 _offset = new(0f);
    private float _brightness = 1f;
    private float _reflectance = 0f;

    public HitRecord? Hit(in Ray ray, float tMin, float tMax) =>
        Entity.Hit(ray with { Origin = ray.Origin - _offset }, tMin, tMax) is HitRecord record
            ? record with { Point = record.Point + _offset, Brightness = _brightness, Reflectance = _reflectance }
            : null;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Entity.Illuminate(new Apply<I>(entity, -_offset), record with { Point = record.Point - _offset }) * _brightness;

    public void Update(float dt)
    {
        _timeElapsed += dt;
        _offset = Offset?.GetValue(_timeElapsed) ?? new(0f);
        _brightness = Brightness?.GetValue(_timeElapsed) ?? 1f;
        _reflectance = Reflectance?.GetValue(_timeElapsed) ?? 0f;
    }
}
