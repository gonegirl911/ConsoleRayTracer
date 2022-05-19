using System.Runtime.InteropServices;

namespace RayTracer;

public record World(
    string Name,
    Scene Scene,
    Camera Camera,
    Animator Animator
)
{
    public void Start(AppConfig config)
    {
        Tutorial tutorial = new();

        if (config is { Window: Window.WindowsTerminal, Renderer: Renderer.RayTracer } && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            App<WindowsTerminal<RayTracer>, RayTracer> app = new(new(
                width: config.Width,
                height: config.Height,
                renderer: new(),
                title: Name
            ));

            app.StartMainLoop((window, dt) =>
            {
                var key = window.KeyPressed();
                tutorial.Update(key);
                Camera.AspectRatio = (float)window.Width / window.Height;
                Camera.Update(key, dt);
                Animator.Update(Scene, key, dt);
                ((IWindow<RayTracer>)window).Draw(Scene, Camera);
                ((IWindow<RayTracer>)window).Draw(tutorial.Label);
            });
        }

        throw new ArgumentException("invalid AppConfig");
    }

    public static World Default =>
        new(
            Name: "Default",
            Scene: new(
                Entities: new IEntity[]
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
                },
                Lights: new IEntity[]
                {
                    new Animated<LightSource, Animation<Vector3, CircularPath<AxisZ>, LightsInterpolator>, Constant<float>, Constant<float>>(
                        Entity: new(),
                        Offset: new(
                            Motion: new(1000f, new()),
                            Interpolator: new(),
                            Duration: 20_000f
                        )
                    ),
                }
            ),
            Camera: new(
                lookFrom: new(-12f, 9f, -21f),
                lookAt: new(0f, 3f, 0f),
                vFov: 45f,
                speed: 3f,
                sensitivity: 0.5f
            ),
            Animator: new(sensitivity: 0.2f)
        );
}

public readonly record struct AppConfig(
    Window Window,
    Renderer Renderer,
    int Width,
    int Height
);

public enum Window { WindowsTerminal }
public enum Renderer { RayTracer }
