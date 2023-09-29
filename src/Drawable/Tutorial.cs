namespace ConsoleRayTracer;

sealed class Tutorial : IDrawable, IEventHandler
{
    private static readonly Label?[] LABELS =
    {
        new("Created by Damyan Slavov, Petar Dobrev, Simeon Obretenov, 11d"),
        new("Do you want to go through the tutorial?    Y/n"),
        new("Move around    W,A,S,D"),
        new("Look around    ArrowUp,Left,Down,Right"),
        new("Go up/down     Space,Z"),
        new("Slow down/speed up time    K,L"),
        new("Stop time      P"),
        new("Go back/forward in time    K,L"),
        new("Start time     P"),
        new("Tutorial completed"),
        new("Feel free to resize the window"),
        null,
    };

    int _stage;
    ConsoleKey? _lastKey;

    public Tutorial()
    {
        _stage = 0;
        _lastKey = null;
    }

    public void Draw<C>(C canvas) where C : class, ICanvas<C>
    {
        if (LABELS[_stage] is Label label)
        {
            label.Draw(canvas);
        }
    }

    public void Handle(Event? ev, TimeSpan dt)
    {
        if (ev?.KeyEvent is KeyEvent keyEvent)
        {
            if (keyEvent.PressedKey is ConsoleKey key && key != _lastKey)
            {
                _stage = (_stage, key) switch
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
                    _ => _stage,
                };
                _lastKey = key;
            }
            else
            {
                _lastKey = null;
            }
        }
    }
}
