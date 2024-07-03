using System.Numerics;
using ConsoleRayTracer;

new App<Game>(
    Canvas: NativeTerminal.Get(95, 70, "ConsoleRayTracer"),
    Drawable: new(
        Scene: new(
            Entity: new(
            [
                new Apply<Cylinder>(
                    Entity: new(4F, 1F),
                    Offset: new(-5F, 0F, 0F),
                    Brightness: 1.5F,
                    Reflectance: 0.3F
                ),
                new Apply<Cone>(
                    Entity: new(4F, 1F),
                    Brightness: 1.5F,
                    Reflectance: 0.3F
                ),
                new Apply<Cylinder>(
                    Entity: new(4F, 1F),
                    Offset: new(5F, 0F, 0F),
                    Brightness: 1.5F,
                    Reflectance: 0.3F
                ),
                new Apply<Sphere>(
                    Entity: new(1F),
                    Offset: new(-5F, 6F, 0F),
                    Brightness: 1.5F,
                    Reflectance: 0.3F
                ),
                new Apply<Cuboid>(
                    Entity: new(2F, 2F, 2F),
                    Offset: new(-1F, 5F, -1F),
                    Brightness: 1.5F,
                    Reflectance: 0.3F
                ),
                new Apply<Sphere>(
                    Entity: new(1F),
                    Offset: new(5F, 6F, 0F),
                    Brightness: 1.5F,
                    Reflectance: 0.3F
                ),
                new Animated<Apply<Sphere>, PathChain, Constant<float>, MotionChain>(
                    Entity: new(
                        Entity: new(1F),
                        Offset: new(0F, 1F, 0F),
                        Brightness: 1.5F
                    ),
                    Offset: new(
                    [
                        new Animation<Vector3, CircularPath<AxisY>, LinearInterpolator>(
                            Motion: new(10F),
                            Interpolator: new(),
                            Duration: 2000F
                        ),
                        new Animation<Vector3, LinearPath, DecelerateInterpolator>(
                            Motion: new(new(0F, 10F, 0F)),
                            Interpolator: new(2F),
                            Duration: 750F
                        ),
                        new Animation<Vector3, LinearPath, AccelerateInterpolator>(
                            Motion: new(new(0F, -10F, 0F)),
                            Interpolator: new(2F),
                            Duration: 750F
                        ),
                        new Animation<Vector3, LinearPath, DecelerateInterpolator>(
                            Motion: new(new(0F, 2F, 0F)),
                            Interpolator: new(2F),
                            Duration: 150F
                        ),
                        new Animation<Vector3, LinearPath, AccelerateInterpolator>(
                            Motion: new(new(0F, -2F, 0F)),
                            Interpolator: new(2F),
                            Duration: 150F
                        ),
                    ]),
                    Reflectance: new(
                    [
                        new Animation<float, LinearMotion, LinearInterpolator>(
                            Motion: new(1F),
                            Interpolator: new(),
                            Duration: 1900F
                        ),
                        new Animation<float, LinearMotion, LinearInterpolator>(
                            Motion: new(-1F),
                            Interpolator: new(),
                            Duration: 1900F
                        ),
                    ])
                ),
                new Apply<Plane<AxisY>>(
                    Entity: new(),
                    Brightness: 2.1F,
                    Reflectance: 0.7F
                ),
            ]),
            Light: new(
            [
                new Animated<LightSource, Animation<Vector3, CircularPath<AxisZ>, SunInterpolator>, Constant<float>, Constant<float>>(
                    Entity: new(),
                    Offset: new(
                        Motion: new(1000F),
                        Interpolator: new(),
                        Duration: 20000F
                    )
                ),
            ]),
            Camera: new(
                lookFrom: new(-12F, 9F, -21F),
                lookAt: new(0F, 3F, 0F),
                verticalFov: 90F,
                aspectRatio: 0F,
                speed: 3F,
                sensitivity: 0.5F
            ),
            Renderer: new(Depth: 50)
        ),
        Animator: new(speed: 1F, sensitivity: 3F),
        Crosshair: new(),
        Tutorial: new()
    )
)
.Run();
