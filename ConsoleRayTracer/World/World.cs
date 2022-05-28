using System.Runtime.InteropServices;

namespace ConsoleRayTracer;

public record World<E, C>(string Name, Scene<E, C> Scene, Animator Animator)
    where E : IAnimatedEntity
    where C : IControllableCamera
{
    public void Start<T, R>(AppConfig<T, R> config) where T : IDynamicTerminal<R> where R : IRenderer
    {
        Tutorial tutorial = new();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && config is AppConfig<WindowsTerminal<RayTracer>, RayTracer>)
        {
            App<WindowsTerminal<RayTracer>, RayTracer> app = new(new(config.Width, config.Height, new(), Name));

            app.StartMainLoop((terminal, dt) =>
            {
                var key = terminal.KeyPressed();
                Animator.Update(Scene, key, dt, (float)terminal.Width / terminal.Height);
                tutorial.Update(key);
                terminal.Draw<WindowsTerminal<RayTracer>, RayTracer, Scene<E, C>>(Scene);
                terminal.Draw<WindowsTerminal<RayTracer>, RayTracer, Tutorial>(tutorial);
            });
        }

        throw new ArgumentException("invalid AppConfig");
    }
}
