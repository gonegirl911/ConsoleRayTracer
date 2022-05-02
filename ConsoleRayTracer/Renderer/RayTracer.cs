﻿namespace ConsoleRayTracer;

readonly record struct RayTracer : IRenderer
{
    public float PixelColor<E, L, C>(float s, float t, in E entity, in L light, in C camera)
        where E : IEntity
        where L : IEntity
        where C : ICamera
    {
        return RayTrace(entity, light, camera.GetRay(s, t));
    }

    private float RayTrace<E, L>(in E entity, in L light, in Ray ray, int depth = 50)
        where E : IEntity
        where L : IEntity
    {
        if (depth > 0f && entity.Hit(ray, 0.001f, float.PositiveInfinity) is HitRecord record)
        {
            record = record with { Normal = ray.Opposite(record.Normal) };
            Ray reflected = new(record.Point, Vector3.Reflect(ray.Direction, record.Normal));
            var diffused = light.Illuminate(entity, record);
            var k = record.Reflectance;
            return Math.Clamp(k * RayTrace(entity, light, reflected, depth - 1) + (1f - k) * diffused, 0f, 1f);
        }
        return 0f;
    }
}
