using System.Numerics;

namespace ConsoleRayTracer;

readonly record struct RayTracer<E, L, C>(int Depth) : IRenderer<Scene<E, L, C, RayTracer<E, L, C>>>
    where E : IEntity
    where L : IEntity
    where C : ICamera
{
    public float Render(in Scene<E, L, C, RayTracer<E, L, C>> scene, float s, float t) =>
        Trace(scene.Entity, scene.Light, scene.Camera.CastRay(s, t), Depth);

    float Trace(in E entity, in L light, Ray ray, int depth)
    {
        if (depth > 0f && entity.Hit(ray, 0.001f, float.PositiveInfinity) is HitRecord record)
        {
            record = record with { Normal = ray.Opposite(record.Normal) };
            var diffused = light.Illuminate(entity, record);
            ray = new(record.Point, Vector3.Reflect(ray.Direction, record.Normal));
            var reflected = Trace(entity, light, ray, depth - 1);
            return float.Clamp(Lerp(diffused, reflected, record.Reflectance), 0f, 1f);
        }
        return 0f;
    }

    float Lerp(float a, float b, float t) => a * (1f - t) + b * t;
}
