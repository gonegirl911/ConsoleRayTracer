namespace ConsoleRayTracer;

public sealed class Tutorial : IDrawable
{
    private const int BORDER_WIDTH = 1;
    private const int PADDING = 2;

    private static readonly Label?[] _labels =
    {
        new("Do you want to go through the tutorial?    Y/n", BORDER_WIDTH, PADDING),
        new("Move around    W,A,S,D", BORDER_WIDTH, PADDING),
        new("Look around    ArrowUp,Left,Down,Right", BORDER_WIDTH, PADDING),
        new("Go up/down     Space,Z", BORDER_WIDTH, PADDING),
        new("Slow down/speed up time    K,L", BORDER_WIDTH, PADDING),
        new("Stop time      P", BORDER_WIDTH, PADDING),
        new("Go back/forward in time    K,L", BORDER_WIDTH, PADDING),
        new("Start time     P", BORDER_WIDTH, PADDING),
        new("Tutorial completed", BORDER_WIDTH, PADDING),
        new("Feel free to resize the window", BORDER_WIDTH, PADDING),
        null,
    };

    private int _step = 0;
    private ConsoleKey? _lastKey = null;

    public void Draw<C, R>(C canvas, R renderer)
        where C : class, ICanvas
        where R : class, IRenderer
    {
        if (_labels[_step] is Label label)
        {
            canvas.Draw(label, renderer);
        }
    }

    public void Progress(ConsoleKey? key)
    {
        if (key is not null && key != _lastKey)
        {
            _step = (_step, key) switch
            {
                (0, ConsoleKey.Y) => 1,
                (0, ConsoleKey.N) => 10,
                (1, ConsoleKey.W or ConsoleKey.A or ConsoleKey.S or ConsoleKey.D) => 2,
                (2, ConsoleKey.UpArrow or ConsoleKey.LeftArrow or ConsoleKey.DownArrow or ConsoleKey.RightArrow) => 3,
                (3, ConsoleKey.Spacebar or ConsoleKey.Z) => 4,
                (4, ConsoleKey.K or ConsoleKey.L) => 5,
                (5, ConsoleKey.P) => 6,
                (6, ConsoleKey.K or ConsoleKey.L) => 7,
                (7, ConsoleKey.P) => 8,
                (8, _) => 9,
                (9, _) => 10,
                _ => _step,
            };
        }
        _lastKey = key;
    }
}
