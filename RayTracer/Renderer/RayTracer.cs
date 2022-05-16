namespace RayTracer;

readonly record struct RayTracer : IRenderer
{
    public float PixelColor<E, C>(in E entity, in C camera, float s, float t, float aspectRatio)
        where E : IEntity
        where C : ICamera
    {
        return RayTrace(entity, camera.GetRay(s, t, aspectRatio));
    }

    private float RayTrace<E>(in E entity, in Ray ray, int depth = 50) where E : IEntity
    {
        if (depth > 0f && entity.Hit(ray, 0.001f, float.PositiveInfinity) is HitRecord record)
        {
            record = record with { Normal = ray.Opposite(record.Normal) };
            Ray reflected = new(record.Point, Vector3.Reflect(ray.Direction, record.Normal));
            var diffused = entity.Illuminate(entity, record);
            var k = record.Reflectance;
            return Math.Clamp(k * RayTrace(entity, reflected, depth - 1) + (1f - k) * diffused, 0f, 1f);
        }
        return 0f;
    }
}
