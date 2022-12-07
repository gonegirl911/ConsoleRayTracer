namespace ConsoleRayTracer;

public readonly record struct RayTracer<E, L, C>(int Depth = 50)
    : IRenderer<Scene<E, L, C, RayTracer<E, L, C>>>
    where E : IEntity
    where L : IEntity
    where C : ICamera
{
    public float Trace(in Scene<E, L, C, RayTracer<E, L, C>> scene, float s, float t)
    {
        return Color(scene.Entity, scene.Light, scene.Camera.CastRay(s, t), Depth);
    }

    private float Color(in E entity, in L light, in Ray ray, int depth)
    {
        if (depth > 0f && entity.Hit(ray, 0.001f, float.PositiveInfinity) is HitRecord record)
        {
            record = record with { Normal = ray.Opposite(record.Normal) };
            Ray reflected = new(record.Point, Vector3.Reflect(ray.Direction, record.Normal));
            var diffused = light.Illuminate(entity, record);
            var k = record.Reflectance;
            return float.Clamp(k * Color(entity, light, reflected, depth - 1) + (1f - k) * diffused, 0f, 1f);
        }
        return 0f;
    }
}
