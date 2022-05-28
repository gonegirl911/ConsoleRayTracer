namespace ConsoleRayTracer;

public interface IControllableCamera : ICamera
{
    void Adjust(float aspectRatio);
    void Move(ConsoleKey? key, float dt);
    void Rotate(ConsoleKey? key, float dr);
}
