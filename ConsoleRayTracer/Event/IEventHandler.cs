namespace ConsoleRayTracer;

public interface IEventHandler
{
    void Handle(in Event? ev, float dt);
}
