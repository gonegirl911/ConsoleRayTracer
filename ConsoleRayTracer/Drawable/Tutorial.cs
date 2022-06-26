namespace ConsoleRayTracer;

public sealed class Tutorial : IDrawable
{
    private const int BORDER_WIDTH = 1;
    private const int PADDING = 2;

    private static readonly Label?[] _labels =
    {
        new("Created by Damyan Slavov, Petar Dobrev, Simeon Obretenov, 11d", BORDER_WIDTH, PADDING),
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

    public void Draw<C>(C canvas) where C : class, ICanvas
    {
        if (_labels[_step] is Label label)
        {
            canvas.Draw(label);
        }
    }

    public void Update(ConsoleKey? key)
    {
        if (key is not null && key != _lastKey)
        {
            _step = (_step, key) switch
            {
                (0, _) => 1,
                (1, ConsoleKey.Y) => 2,
                (1, ConsoleKey.N) => 11,
                (2, ConsoleKey.W or ConsoleKey.A or ConsoleKey.S or ConsoleKey.D) => 3,
                (3, ConsoleKey.UpArrow or ConsoleKey.LeftArrow or ConsoleKey.DownArrow or ConsoleKey.RightArrow) => 4,
                (4, ConsoleKey.Spacebar or ConsoleKey.Z) => 5,
                (5, ConsoleKey.K or ConsoleKey.L) => 6,
                (6, ConsoleKey.P) => 7,
                (7, ConsoleKey.K or ConsoleKey.L) => 8,
                (8, ConsoleKey.P) => 9,
                (9, _) => 10,
                (10, _) => 11,
                _ => _step,
            };
        }
        _lastKey = key;
    }
}
