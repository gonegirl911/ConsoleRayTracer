using ConsoleRayTracer;

class Program
{
    static readonly Random random = new();

    static void Main()
    {
        const float INTENSITY = 1.5f;
        const float BRIGHTNESS = 1.5f;
        const float REFLECTION = 0.3f;

        Light light = new(new LightSource[] { new(new(0f, 1002.5f, 1000f), INTENSITY) });

        World world = new(
            Enumerable.Range(0, 10)
                .Select(_ =>
                {
                    var x = RandomInRange(-5f, 5f);
                    var y = RandomInRange(0.4f, 1.5f);
                    var z = RandomInRange(-5f, 5f);
                    return new Translate<Sphere>(new(y, BRIGHTNESS, REFLECTION), new(x, y, z)) as IHittable;
                })
                .Append(new ConsoleRayTracer.Plane(Vector3.UnitY, 2f, 0.7f))
                .ToArray()
        );

        for (var i = 0; ; i++)
        {
            Camera camera = new(
                lookFrom: new(-10f, 2.5f, i * 0.125f),
                lookAt: new(0f, 1f, 0f),
                vUp: Vector3.UnitY,
                vFov: 25f,
                aspectRatio: (float)Screen.WIDTH / Screen.HEIGHT
            );

            Parallel.For(0, Screen.HEIGHT, y =>
            {
                for (var x = 0; x < Screen.WIDTH; x++)
                {
                    Screen.Set(x, y, PixelColor(x, y, world, camera, light));
                }
            });

            Screen.Draw();
        }
    }

    static float PixelColor(int x, int y, in World world, in Camera camera, in Light light) =>
        RayTrace(world, light, camera.GetRay((float)x / Screen.WIDTH, (float)y / Screen.HEIGHT));

    static float RayTrace(in World world, in Light light, Ray ray, int depth = 50)
    {
        if (depth == 0f)
        {
            return 0f;
        }
        if (world.Hit(ray, 0.001f, float.MaxValue) is HitRecord record)
        {
            var diffused = light.Enlighten(world, record);
            var reflected = ray.Direction - 2 * Vector3.Dot(ray.Direction, record.Normal) * record.Normal;
            var k = record.Reflection;
            return Math.Clamp(k * RayTrace(world, light, new(record.Point, reflected), depth - 1) + (1 - k) * diffused, 0f, 1f);
        }
        return 0f;
    }

    static float RandomInRange(float start, float end) =>
        start + (float)random.NextDouble() * (end - start);
}
