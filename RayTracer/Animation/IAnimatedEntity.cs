namespace RayTracer;

public interface IAnimatedEntity : IEntity
{
    void Update(float timeElapsed);
}

public record Animated<E, O, B, R>(
    E Entity,
    O Offset = default!,
    B Brightness = default!,
    R Reflectance = default!
) : IAnimatedEntity
    where E : IEntity
    where O : IAnimation<Vector3>
    where B : IAnimation<float>
    where R : IAnimation<float>
{
    private Vector3 _offset = Offset.GetValue(0f);
    private float _brightness = 1f + Brightness.GetValue(0f);
    private float _reflectance = Reflectance.GetValue(0f);

    public HitRecord? Hit(in Ray ray, float tMin, float tMax) =>
        Entity.Hit(ray with { Origin = ray.Origin - _offset }, tMin, tMax) is HitRecord record
            ? record with
            {
                Point = record.Point + _offset,
                Brightness = record.Brightness + _brightness,
                Reflectance = record.Reflectance + _reflectance,
            }
            : null;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Entity.Illuminate(new Apply<I>(entity, -_offset), record with { Point = record.Point - _offset })
            * _brightness;

    public void Update(float timeElapsed)
    {
        _offset = Offset.GetValue(timeElapsed);
        _brightness = 1f + Brightness.GetValue(timeElapsed);
        _reflectance = Reflectance.GetValue(timeElapsed);
    }   
}
