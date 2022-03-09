using ConsoleRayTracer;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
class Program
{
    const int WIDTH = 120;
    const int HEIGHT = 100;

    static readonly Random random = new();

    static void Main()
    {
        const float INTENSITY = 1.5f;
        const float BRIGHTNESS = 1.5f;
        const float REFLECTION = 0.3f;

        App<WindowsTerminal, RayTracer> app = new(
            terminal: new(WIDTH, HEIGHT),
            renderer: new()
        );

        Light light = new(new LightSource[] { new(new(0f, 1000f, 1000f), INTENSITY) });

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

        app.StartMainLoop((app, frame) =>
        {
            Camera camera = new(
                lookFrom: new(-10f, 2.5f, frame * 0.125f),
                lookAt: new(0f, 1f, 0f),
                vUp: Vector3.UnitY,
                vFov: 25f,
                aspectRatio: (float)WIDTH / HEIGHT
            );

            app.Render(world, camera, light);
        });
    }

    static float RandomInRange(float start, float end) =>
        start + (float)random.NextDouble() * (end - start);
}
