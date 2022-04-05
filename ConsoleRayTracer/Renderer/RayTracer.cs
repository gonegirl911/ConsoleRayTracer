﻿namespace ConsoleRayTracer;

readonly struct RayTracer : IRenderer
{
    public float PixelColor<E, C, L>(float s, float t, in E entity, in L light, in C camera)
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
        if (depth == 0f)
        {
            return 0f;
        }
        if (entity.Hit(ray, 0.001f, float.MaxValue) is HitRecord record)
        {
            var diffused = light.Illuminate(entity, record);
            var reflected = ray.Direction - 2 * Vector3.Dot(ray.Direction, record.Normal) * record.Normal;
            var k = record.Reflectance;
            return Math.Clamp(k * RayTrace(entity, light, new(record.Point, reflected), depth - 1) + (1 - k) * diffused, 0f, 1f);
        }
        return 0f;
    }
}
