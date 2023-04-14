namespace ConsoleRayTracer;

public interface IEventHandler
{
    void Handle(Event? ev, TimeSpan dt);
}
