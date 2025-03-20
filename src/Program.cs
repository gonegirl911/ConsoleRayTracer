using System.Numerics;
using ConsoleRayTracer;

new App<Game>(
    canvas: NativeTerminal.Get(95, 70, "ConsoleRayTracer"),
    drawable: new(
        scene: new(
            Entity: new(
            [
                new Apply<Cylinder>(
                    entity: new(4F, 1F),
                    offset: new(-5F, 0F, 0F),
                    brightness: 1.5F,
                    reflectance: 0.3F
                ),
                new Apply<Cone>(
                    entity: new(4F, 1F),
                    brightness: 1.5F,
                    reflectance: 0.3F
                ),
                new Apply<Cylinder>(
                    entity: new(4F, 1F),
                    offset: new(5F, 0F, 0F),
                    brightness: 1.5F,
                    reflectance: 0.3F
                ),
                new Apply<Sphere>(
                    entity: new(1F),
                    offset: new(-5F, 6F, 0F),
                    brightness: 1.5F,
                    reflectance: 0.3F
                ),
                new Apply<Cuboid>(
                    entity: new(2F, 2F, 2F),
                    offset: new(-1F, 5F, -1F),
                    brightness: 1.5F,
                    reflectance: 0.3F
                ),
                new Apply<Sphere>(
                    entity: new(1F),
                    offset: new(5F, 6F, 0F),
                    brightness: 1.5F,
                    reflectance: 0.3F
                ),
                new Animated<Apply<Sphere>, PathChain, Constant<float>, MotionChain>(
                    entity: new(
                        entity: new(1F),
                        offset: new(0F, 1F, 0F),
                        brightness: 1.5F
                    ),
                    offset: new(
                    [
                        new Animation<Vector3, CircularPath<AxisY>, LinearInterpolator>(
                            motion: new(10F),
                            interpolator: new(),
                            duration: 2000F
                        ),
                        new Animation<Vector3, LinearPath, DecelerateInterpolator>(
                            motion: new(new(0F, 10F, 0F)),
                            interpolator: new(2F),
                            duration: 750F
                        ),
                        new Animation<Vector3, LinearPath, AccelerateInterpolator>(
                            motion: new(new(0F, -10F, 0F)),
                            interpolator: new(2F),
                            duration: 750F
                        ),
                        new Animation<Vector3, LinearPath, DecelerateInterpolator>(
                            motion: new(new(0F, 2F, 0F)),
                            interpolator: new(2F),
                            duration: 150F
                        ),
                        new Animation<Vector3, LinearPath, AccelerateInterpolator>(
                            motion: new(new(0F, -2F, 0F)),
                            interpolator: new(2F),
                            duration: 150F
                        ),
                    ]),
                    reflectance: new(
                    [
                        new Animation<float, LinearMotion, LinearInterpolator>(
                            motion: new(1F),
                            interpolator: new(),
                            duration: 1900F
                        ),
                        new Animation<float, LinearMotion, LinearInterpolator>(
                            motion: new(-1F),
                            interpolator: new(),
                            duration: 1900F
                        ),
                    ])
                ),
                new Apply<Plane<AxisY>>(
                    entity: new(),
                    brightness: 2.1F,
                    reflectance: 0.7F
                ),
            ]),
            Light: new(
            [
                new Animated<LightSource, Animation<Vector3, CircularPath<AxisZ>, SunInterpolator>, Constant<float>, Constant<float>>(
                    entity: new(),
                    offset: new(
                        motion: new(1000F),
                        interpolator: new(),
                        duration: 20000F
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
            Renderer: new(depth: 50)
        ),
        animator: new(speed: 1F, sensitivity: 3F),
        crosshair: new(),
        tutorial: new()
    )
)
.Run();
