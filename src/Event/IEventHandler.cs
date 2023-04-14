namespace ConsoleRayTracer;

interface IEventHandler
{
    void Handle(Event? ev, TimeSpan dt);
}
