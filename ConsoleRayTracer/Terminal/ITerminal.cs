namespace ConsoleRayTracer;

interface ITerminal
{
    short Width { get; }
    short Height { get; }

    void SetPixel(int x, int y, float color);
    void Draw();
}
