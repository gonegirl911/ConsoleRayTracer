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
        App<WindowsTerminal, RayTracer> app = new(
            terminal: new(WIDTH, HEIGHT),
            renderer: new()
        );

        Light light = new(new LightSource[] { new(new(0f, 1000f, 1000f), 1.5f) });

        World world = new(
            Enumerable.Range(0, 10)
                .Select(_ =>
                {
                    var x = RandomInRange(-7f, 7f);
                    var y = RandomInRange(0.5f, 1.5f);
                    var z = RandomInRange(-7f, 7f);
                    return new Translate<Sphere>(new(y, 1.5f, 0.3f), new(x, y, z)) as IHittable;
                })
                .Append(new ConsoleRayTracer.Plane(Vector3.UnitY, 2.1f, 0.7f))
                .ToArray()
        );

        Camera camera = new(
            lookFrom: new(-10f, 2.5f, 10f),
            lookAt: new(0f, 1f, 0f),
            vUp: Vector3.UnitY,
            vFov: 25f,
            aspectRatio: (float)WIDTH / HEIGHT,
            speed: 3f,
            sensitivity: 0.5f
        );

        app.StartMainLoop((window, dt) =>
        {
            camera.Move(window.KeyPressed(), dt / 1000);
            window.Draw(world, camera, light);
        });
    }

    static float RandomInRange(float start, float end) =>
        start + (float)random.NextDouble() * (end - start);
}
