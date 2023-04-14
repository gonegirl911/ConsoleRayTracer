namespace ConsoleRayTracer;

sealed class Animator : IEventHandler
{
    private float _timeElapsed;
    private readonly Controller _controller;

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

    private sealed class Controller
    {
        private Keys _relevantKeys;
        private Keys _keyHistory;
        private bool _isRunning;
        private float _speed;
        private readonly float _sensitivity;

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

        private void OnKeyEvent(KeyEvent keyEvent)
        {
            var (key, opposite) = keyEvent switch
            {
                { Key: ConsoleKey.P, State: KeyState.Pressed } when (_keyHistory & Keys.P) == 0 => (Keys.P, default),
                { Key: ConsoleKey.P, State: KeyState.Released } => (Keys.P, default),
                { Key: ConsoleKey.L, State: _ } => (Keys.L, Keys.K),
                { Key: ConsoleKey.K, State: _ } => (Keys.K, Keys.L),
                _ => (default, default),
            };

            if (keyEvent.State == KeyState.Pressed)
            {
                _relevantKeys |= key;
                _relevantKeys &= ~opposite;
                _keyHistory |= key;
            }
            else if (keyEvent.State == KeyState.Released)
            {
                _relevantKeys &= ~key;

                if ((_keyHistory & opposite) != 0)
                {
                    _relevantKeys |= opposite;
                }

                _keyHistory &= ~key;
            }
        }

        private void Continue(Animator animator, TimeSpan dt)
        {
            var ds = _sensitivity * (float)dt.TotalSeconds;
            var de = (float)dt.TotalMilliseconds * _speed;

            if ((_relevantKeys & Keys.L) != 0)
            {
                _speed += ds;
            }
            else if ((_relevantKeys & Keys.K) != 0)
            {
                _speed -= ds;
            }

            animator._timeElapsed += de;
        }

        private void TimeTravel(Animator animator, TimeSpan dt)
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

        private enum Keys : byte
        {
            P = 1 << 0,
            L = 1 << 2,
            K = 1 << 1,
        }
    }
}
