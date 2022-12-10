namespace ConsoleRayTracer;

public sealed class Animator : IEventHandler
{
    private float _speed;
    private readonly float _sensitivity;

    private bool _isRunning;
    private float _timeElapsed;
    
    private readonly Controller _controller;

    public Animator(float sensitivity, float speed = 1f, bool isRunning = true)
    {
        _speed = speed;
        _sensitivity = sensitivity / 1000f;
    
        _isRunning = isRunning;
        _timeElapsed = 0f;
        
        _controller = new();
    }

    public void Handle(in Event? ev, float dt)
    {
        _controller.Handle(ev);
        _controller.Update(this, dt);
    }

    public void MoveForward<E>(in E animatedEntity) where E : IAnimatedEntity
    {
        animatedEntity.Update(_timeElapsed);
    }

    private sealed class Controller
    {
        private Keys _relevantKeys;
        private Keys _keyHistory;

        public Controller()
        {
            _relevantKeys = 0;
            _keyHistory = 0;
        }

        public void Handle(in Event? ev)
        {
            var (key, keyState, opp) = ev?.KeyEvent switch
            {
                { Key: ConsoleKey.P, State: KeyState.Pressed } when (_keyHistory & Keys.P) == 0 => (Keys.P, KeyState.Pressed, (Keys)0),
                { Key: ConsoleKey.P, State: KeyState.Released } => (Keys.P, KeyState.Released, (Keys)0),
                { Key: ConsoleKey.K, State: var state } => (Keys.K, state, Keys.L),
                { Key: ConsoleKey.L, State: var state } => (Keys.L, state, Keys.K),
                _ => ((Keys)0, KeyState.Pressed, (Keys)0),
            };

            switch (keyState)
            {
                case KeyState.Pressed:
                    _relevantKeys |= key;
                    _relevantKeys &= ~opp;
                    _keyHistory |= key;
                    break;
                case KeyState.Released:
                    _relevantKeys &= ~key;
                    if ((_keyHistory & opp) != 0)
                    {
                        _relevantKeys |= opp;
                    }
                    _keyHistory &= ~key;
                    break;
            }
        }

        public void Update(Animator animator, float dt)
        {
            if ((_relevantKeys & Keys.P) != 0)
            {
                animator._isRunning = !animator._isRunning;
                _relevantKeys &= ~Keys.P;
            }

            if (animator._isRunning)
            {
                if ((_relevantKeys & Keys.K) != 0)
                {
                    animator._speed -= dt * animator._sensitivity;
                }
                else if ((_relevantKeys & Keys.L) != 0)
                {
                    animator._speed += dt * animator._sensitivity;
                }

                animator._timeElapsed += dt * animator._speed;
            }
            else
            {
                if ((_relevantKeys & Keys.K) != 0)
                {
                    animator._timeElapsed -= dt * animator._speed;
                }
                else if ((_relevantKeys & Keys.L) != 0)
                {
                    animator._timeElapsed += dt * animator._speed;
                }
            }
        }

        private enum Keys : byte
        {
            P = 1 << 0,
            K = 1 << 1,
            L = 1 << 2,
        }
    }
}
