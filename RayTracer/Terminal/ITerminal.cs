namespace RayTracer;

interface ITerminal
{
    int Width { get; }
    int Height { get; }

    void SetPixel(int x, int y, float color);
    void Draw();
    ConsoleKey? KeyPressed();
}
