namespace App;

sealed record World(
    Scene<Group, Lights, Camera, RayTracer<Group, Lights, Camera>> Scene,
    Animator Animator,
    Crosshair Crosshair,
    Tutorial Tutorial
) : IDrawable, IEventHandler
{
    public void Draw<C>(C canvas) where C : class, ICanvas<C>
    {
        canvas.Draw(Scene);
        canvas.Draw(Crosshair);
        canvas.Draw(Tutorial);
    }

    public void Handle(in Event? ev, float dt)
    {
        Scene.Camera.Handle(ev, dt);
        Animator.Handle(ev, dt);
        Tutorial.Handle(ev, dt);

        Animator.MoveForward(Scene.Entity);
        Animator.MoveForward(Scene.Light);
    }
}
