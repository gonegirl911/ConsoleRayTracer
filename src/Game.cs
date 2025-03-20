namespace ConsoleRayTracer;

readonly struct Game(
    Scene<Group, Lights, Camera, RayTracer<Group, Lights, Camera>> scene,
    Animator animator,
    Crosshair crosshair,
    Tutorial tutorial
) : IDrawable, IEventHandler
{
    public void Draw(ICanvas canvas)
    {
        scene.Draw(canvas);
        crosshair.Draw(canvas);
        tutorial.Draw(canvas);
    }

    public void Handle(Event? ev, TimeSpan dt)
    {
        scene.Camera.Handle(ev, dt);
        animator.Handle(ev, dt);
        tutorial.Handle(ev, dt);
        animator.MoveForward(scene.Entity);
        animator.MoveForward(scene.Light);
    }
}
