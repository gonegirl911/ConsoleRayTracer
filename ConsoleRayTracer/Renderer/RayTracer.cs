namespace ConsoleRayTracer;

readonly struct RayTracer : IRenderer
{
    public float PixelColor<H, C, L>(float s, float t, in H hittable, in C camera, in L light)
        where H : IHittable
        where C : ICamera
        where L : ILight
    {
        return RayTrace(hittable, light, camera.GetRay(s, t));
    }

    private float RayTrace<H, L>(in H hittable, in L light, in Ray ray, int depth = 50)
        where H : IHittable
        where L : ILight
    {
        if (depth == 0f)
        {
            return 0f;
        }
        if (hittable.Hit(ray, 0.001f, float.MaxValue) is HitRecord record)
        {
            var diffused = light.Illuminate(hittable, record);
            var reflected = ray.Direction - 2 * Vector3.Dot(ray.Direction, record.Normal) * record.Normal;
            var k = record.Reflection;
            return Math.Clamp(k * RayTrace(hittable, light, new(record.Point, reflected), depth - 1) + (1 - k) * diffused, 0f, 1f);
        }
        return 0f;
    }
}
