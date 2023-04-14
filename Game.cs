namespace ConsoleRayTracer;

public sealed record Game(
    Scene<Group, Lights, Camera, RayTracer<Group, Lights, Camera>> Scene,
    Animator Animator,
    Crosshair Crosshair,
    Tutorial Tutorial
) : IDrawable, IEventHandler
{
    public void Draw<C>(C canvas) where C : class, ICanvas<C>
    {
        Scene.Draw(canvas);
        Crosshair.Draw(canvas);
        Tutorial.Draw(canvas);
    }

    public void Handle(Event? ev, TimeSpan dt)
    {
        Scene.Camera.Handle(ev, dt);
        Animator.Handle(ev, dt);
        Tutorial.Handle(ev, dt);
        Animator.MoveForward(Scene.Entity);
        Animator.MoveForward(Scene.Light);
    }
}
