namespace ConsoleRayTracer;

sealed class Animator : IEventHandler
{
    float _timeElapsed;
    Controller _controller;

    public Animator(float speed, float sensitivity, bool isRunning = true)
    {
        _timeElapsed = 0f;
        _controller = new(speed, sensitivity, isRunning);
    }

    public void Handle(Event? ev, TimeSpan dt)
    {
        _controller.Handle(ev);
        _controller.ApplyUpdates(this, dt);
    }

    public void MoveForward<E>(in E animatedEntity) where E : IAnimatedEntity
    {
        animatedEntity.Update(_timeElapsed);
    }

    struct Controller
    {
        Keys _relevantKeys;
        Keys _keyHistory;
        bool _isRunning;
        float _speed;
        readonly float _sensitivity;

        public Controller(float speed, float sensitivity, bool isRunning)
        {
            _relevantKeys = 0;
            _keyHistory = 0;
            _isRunning = isRunning;
            _speed = speed;
            _sensitivity = sensitivity;
        }

        public void Handle(Event? ev)
        {
            if (ev?.KeyEvent is KeyEvent keyEvent)
            {
                OnKeyEvent(keyEvent);
            }
        }

        public void ApplyUpdates(Animator animator, TimeSpan dt)
        {
            if ((_relevantKeys & Keys.P) != 0)
            {
                _isRunning = !_isRunning;
                _relevantKeys &= ~Keys.P;
            }

            if (_isRunning)
            {
                Continue(animator, dt);
            }
            else
            {
                TimeTravel(animator, dt);
            }
        }

        void OnKeyEvent(KeyEvent ev)
        {
            var (key, oppositeKey) = KeyPair(ev);
            if (ev.State == KeyState.Pressed)
            {
                _relevantKeys |= key;
                _relevantKeys &= ~oppositeKey;
                _keyHistory |= key;
            }
            else if (ev.State == KeyState.Released)
            {
                _relevantKeys &= ~key;
                if ((_keyHistory & oppositeKey) != 0)
                {
                    _relevantKeys |= oppositeKey;
                }
                _keyHistory &= ~key;
            }
        }

        void Continue(Animator animator, TimeSpan dt)
        {
            var ds = _sensitivity * (float)dt.TotalSeconds;
            if ((_relevantKeys & Keys.L) != 0)
            {
                _speed += ds;
            }
            else if ((_relevantKeys & Keys.K) != 0)
            {
                _speed -= ds;
            }
            animator._timeElapsed += (float)dt.TotalMilliseconds * _speed;
        }

        readonly void TimeTravel(Animator animator, TimeSpan dt)
        {
            var de = (float)dt.TotalMilliseconds * _speed;
            if ((_relevantKeys & Keys.L) != 0)
            {
                animator._timeElapsed += de;
            }
            else if ((_relevantKeys & Keys.K) != 0)
            {
                animator._timeElapsed -= de;
            }
        }

        readonly (Keys, Keys) KeyPair(KeyEvent ev) =>
            ev switch
            {
                { Key: ConsoleKey.P, State: KeyState.Pressed } when (_keyHistory & Keys.P) == 0 => (Keys.P, default),
                { Key: ConsoleKey.P, State: KeyState.Released } => (Keys.P, default),
                { Key: ConsoleKey.L, State: _ } => (Keys.L, Keys.K),
                { Key: ConsoleKey.K, State: _ } => (Keys.K, Keys.L),
                _ => default,
            };

        enum Keys : byte
        {
            P = 1 << 0,
            L = 1 << 2,
            K = 1 << 1,
        }
    }
}
