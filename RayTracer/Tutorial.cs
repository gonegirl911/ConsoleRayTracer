namespace RayTracer;

public class Tutorial
{
    private static readonly string?[] _labels =
    {
        "Do you want to go through the tutorial?    Y/n",
        "Move around    W,A,S,D",
        "Look around    ArrowUp,Left,Down,Right",
        "Go up/down     Space,Z",
        "Slow down/speed up time    K,L",
        "Stop time      P",
        "Go back/forward in time    K,L",
        "Start time     P",
        "Tutorial completed",
        "Feel free to resize the window",
        null,
    };

    private int _step = 0;
    private ConsoleKey? _lastKey = null;

    public string? Label => _labels[_step];

    public void Update(ConsoleKey? key)
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
