namespace ConsoleRayTracer;

public sealed class Tutorial : IDrawable, IEventHandler
{
    private static readonly Label?[] LABELS =
    {
        new("Created by Damyan Slavov, Petar Dobrev, Simeon Obretenov, 11d", 1, 2),
        new("Do you want to go through the tutorial?    Y/n", 1, 2),
        new("Move around    W,A,S,D", 1, 2),
        new("Look around    ArrowUp,Left,Down,Right", 1, 2),
        new("Go up/down     Space,Z", 1, 2),
        new("Slow down/speed up time    K,L", 1, 2),
        new("Stop time      P", 1, 2),
        new("Go back/forward in time    K,L", 1, 2),
        new("Start time     P", 1, 2),
        new("Tutorial completed", 1, 2),
        new("Feel free to resize the window", 1, 2),
        null,
    };

    private int _step;
    private ConsoleKey? _lastKey;

    public Tutorial()
    {
        _step = 0;
        _lastKey = null;
    }

    public void Draw<C>(C canvas) where C : class, ICanvas<C>
    {
        if (LABELS[_step] is Label label)
        {
            canvas.Draw(label);
        }
    }

    public void Handle(in Event? ev, float dt)
    {
        if (ev?.KeyEvent is KeyEvent keyEvent)
        {
            if (keyEvent.State is KeyState.Pressed && keyEvent.Key != _lastKey)
            {
                _step = (_step, keyEvent.Key) switch
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
                _lastKey = keyEvent.Key;
            }
            else if (keyEvent.State is KeyState.Released && keyEvent.Key == _lastKey)
            {
                _lastKey = null;
            }
        }
    }
}
