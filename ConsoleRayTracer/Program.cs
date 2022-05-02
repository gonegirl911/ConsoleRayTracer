using ConsoleRayTracer;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
class Program
{
    const short WIDTH = 95;
    const short HEIGHT = 70;

    static void Main()
    {
        App<WindowsTerminal, RayTracer> app = new(
            terminal: new(WIDTH, HEIGHT),
            renderer: new()
        );

        World world = new(new IEntity[]
        {
            new Apply<Cylinder>(
                Entity: new(1f, 4f),
                Offset: new(-5f, 0f, 0f),
                Brightness: 1.5f,
                Reflectance: 0.3f
            ),
            new Apply<Cone>(
                Entity: new(1f, 4f),
                Brightness: 1.5f,
                Reflectance: 0.3f
            ),
            new Apply<Cylinder>(
                Entity: new(1f, 4f),
                Offset: new(5f, 0f, 0f),
                Brightness: 1.5f,
                Reflectance: 0.3f
            ),
            new Apply<Sphere>(
                Entity: new(1f),
                Offset: new(-5f, 6f, 0f),
                Brightness: 1.5f,
                Reflectance: 0.3f
            ),
            new Apply<RectPrism>(
                Entity: new(2f, 2f, 2f),
                Offset: new(0f, 5f, 0f),
                Brightness: 1.5f,
                Reflectance: 0.3f
            ),
            new Apply<Sphere>(
                Entity: new(1f),
                Offset: new(5f, 6f, 0f),
                Brightness: 1.5f,
                Reflectance: 0.3f
            ),
            new Animated<Apply<Sphere>, PathChain, Constant<float>, MotionChain>(
                Entity: new(
                    Entity: new(1f),
                    Offset: new(0f, 1f, 0f),
                    Brightness: 1.5f
                ),
                Offset: new(new IAnimation<Vector3>[]
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
                Reflectance: new(new IAnimation<float>[]
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
            ),
            new Apply<Plane<AxisY>>(
                Entity: new(new()),
                Brightness: 2.1f,
                Reflectance: 0.7f
            ),
        });

        Light light = new(new IEntity[]
        {
            new Animated<Apply<LightSource>, Animation<Vector3, CircularPath<AxisZ>, FunctionalInterpolator>, Constant<float>, Constant<float>>(
                Entity: new(
                    Entity: new(),
                    Brightness: 1.5f
                ),
                Offset: new(
                    Motion: new(1000f, new()),
                    Interpolator: new(input => input < 0.9f ? input * 5f / 9f : input * 5f),
                    Duration: 20_000f
                )
            ),
        });

        Camera camera = new(
            lookFrom: new(-12f, 9f, -21f),
            lookAt: new(0f, 3f, 0f),
            vFov: 45f,
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
}
