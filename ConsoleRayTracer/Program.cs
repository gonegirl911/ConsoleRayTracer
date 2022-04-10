using ConsoleRayTracer;
using System.Runtime.Versioning;
using Plane = ConsoleRayTracer.Plane;

[SupportedOSPlatform("windows")]
class Program
{
    const short WIDTH = 120;
    const short HEIGHT = 100;

    static void Main()
    {
        App<WindowsTerminal, RayTracer> app = new(
            terminal: new(WIDTH, HEIGHT),
            renderer: new()
        );

        World world = new(
            Enumerable.Range(0, 10)
                .Select(_ =>
                {
                    var x = RandomInRange(-7f, 7f);
                    var y = RandomInRange(0.5f, 1.5f);
                    var z = RandomInRange(-7f, 7f);
                    return new Apply<Sphere>(
                        Entity: new(y),
                        Offset: new(x, y, z),
                        Brightness: 1.5f,
                        Reflectance: 0.3f
                    ) as IEntity;
                })
                .Append(new Animated<Apply<Sphere>>(
                    Entity: new(
                        Entity: new(1f),
                        Offset: new(0f, 1f, 0f),
                        Brightness: 1.5f
                    ),
                    Offset: new PathChain(new IAnimation<Vector3>[]
                    {   
                        new Animation<Vector3, CircularPath<AxisY>, LinearInterpolator>(
                            Motion: new(10f, new()),
                            Interpolator: new(),
                            Duration: 2000f
                        ),
                        new Animation<Vector3, LinearPath, DecelerateInterpolator>(
                            Motion: new(new(0f, 10f, 0f)),
                            Interpolator: new(2f),
                            Duration: 750f
                        ),
                        new Animation<Vector3, LinearPath, AccelerateInterpolator>(
                            Motion: new(new(0f, -10f, 0f)),
                            Interpolator: new(2f),
                            Duration: 750f
                        ),
                        new Animation<Vector3, LinearPath, DecelerateInterpolator>(
                            Motion: new(new(0f, 2f, 0f)),
                            Interpolator: new(2f),
                            Duration: 150f
                        ),
                        new Animation<Vector3, LinearPath, AccelerateInterpolator>(
                            Motion: new(new(0f, -2f, 0f)),
                            Interpolator: new(2f),
                            Duration: 150f
                        ),
                    }),
                    Reflectance: new MotionChain(new IAnimation<float>[]
                    {
                        new Animation<float, LinearMotion, LinearInterpolator>(
                            Motion: new(1f),
                            Interpolator: new(),
                            Duration: 1900f
                        ),
                        new Animation<float, LinearMotion, LinearInterpolator>(
                            Motion: new(-1f),
                            Interpolator: new(),
                            Duration: 1900f
                        ),
                    })
                 ))
                .Append(new Apply<Plane>(
                    Entity: new(Vector3.UnitY),
                    Brightness: 2.1f,
                    Reflectance: 0.7f
                 ))
                .ToArray()
        );

        Light light = new(new IEntity[]
        {
            new Animated<Apply<LightSource>>(
                Entity: new(
                    Entity: new(),
                    Brightness: 1.5f
                ),
                Offset: new Animation<Vector3, CircularPath<AxisZ>, FunctionalInterpolator>(
                    Motion: new(1000f, new()),
                    Interpolator: new(input => input < 0.9f ? input * 5f / 9f : input * 5f),
                    Duration: 20_000f
                )
            ),
        });

        Camera camera = new(
            lookFrom: new(-16f, 8f, -16f),
            lookAt: new(0f, 3f, 0f),
            vUp: Vector3.UnitY,
            vFov: 25f,
            aspectRatio: (float)WIDTH / HEIGHT,
            speed: 3f,
            sensitivity: 0.5f
        );

        Animator animator = new(sensitivity: 0.2f);

        app.StartMainLoop((window, dt) =>
        {
            var key = window.KeyPressed();
            animator.Update(key, world, light, dt);
            camera.Move(key, dt);
            window.Draw(world, light, camera);
        });
    }

    static readonly Random _random = new();

    static float RandomInRange(float start, float end) =>
        start + (float)_random.NextDouble() * (end - start);
}
