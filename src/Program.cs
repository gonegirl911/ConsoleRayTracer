using System.Numerics;
using ConsoleRayTracer;

if (OperatingSystem.IsWindows())
{
    App<WindowsTerminal, Game> app = new(
        Canvas: new(
            width: 95,
            height: 70,
            title: "ConsoleRayTracer"
        ),
        Drawable: new(
            Scene: new(
                Entity: new(new IEntity[]
                {
                    new Apply<Cylinder>(
                        Entity: new(4f, 1f),
                        Offset: new(-5f, 0f, 0f),
                        Brightness: 1.5f,
                        Reflectance: 0.3f
                    ),
                    new Apply<Cone>(
                        Entity: new(4f, 1f),
                        Brightness: 1.5f,
                        Reflectance: 0.3f
                    ),
                    new Apply<Cylinder>(
                        Entity: new(4f, 1f),
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
                    new Apply<Cuboid>(
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
                                Motion: new(new(), 10f),
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
                }),
                Light: new(new IEntity[]
                {
                    new Animated<LightSource, Animation<Vector3, CircularPath<AxisZ>, LightsInterpolator>, Constant<float>, Constant<float>>(
                        Entity: new(),
                        Offset: new(
                            Motion: new(new(), 1000f),
                            Interpolator: new(),
                            Duration: 20_000f
                        )
                    ),
                }),
                Camera: new(
                    lookFrom: new(-12f, 9f, -21f),
                    lookAt: new(0f, 3f, 0f),
                    verticalFov: 90f,
                    aspectRatio: 95f / 70f,
                    speed: 3f,
                    sensitivity: 0.5f
                ),
                Renderer: new(Depth: 50)
            ),
            Animator: new(speed: 1f, sensitivity: 3f),
            Crosshair: new(),
            Tutorial: new()
        )
    );

    app.Run();
}

throw new InvalidOperationException("WindowsTerminal is only supported on Windows");
