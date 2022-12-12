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

        public Controller()
        {
            _relevantKeys = 0;
            _keyHistory = 0;
        }

        public void Handle(in Event? ev)
        {
            if (ev.HasValue)
            {
                if (ev.Value.Variant is EventVariant.Key)
                {
                    OnKeyEvent(ev.Value.Data.KeyEvent);
                }
            }
        }

        public void ApplyUpdates(Animator animator, float dt)
        {
            if ((_relevantKeys & Keys.P) != 0)
            {
                ToggleIsRunning(animator);
                _relevantKeys &= ~Keys.P;
            }

            if (animator._isRunning)
            {
                ChangeTimeSpeed(animator, dt);
            }
            else
            {
                TimeTravel(animator, dt);
            }
        }

        private void OnKeyEvent(in KeyEvent keyEvent)
        {
            var (key, opp) = keyEvent switch
            {
                { Key: ConsoleKey.P, State: KeyState.Pressed } when (_keyHistory & Keys.P) == 0 => (Keys.P, (Keys)0),
                { Key: ConsoleKey.P, State: KeyState.Released } => (Keys.P, (Keys)0),
                { Key: ConsoleKey.K, State: _ } => (Keys.K, Keys.L),
                { Key: ConsoleKey.L, State: _ } => (Keys.L, Keys.K),
                _ => ((Keys)0, (Keys)0),
            };

            if (keyEvent.State is KeyState.Pressed)
            {
                _relevantKeys |= key;
                _relevantKeys &= ~opp;
                _keyHistory |= key;
            }
            else if (keyEvent.State is KeyState.Released)
            {
                _relevantKeys &= ~key;
                if ((_keyHistory & opp) != 0)
                {
                    _relevantKeys |= opp;
                }
                _keyHistory &= ~key;
            }
        }

        private void ToggleIsRunning(Animator animator)
        {
            animator._isRunning = !animator._isRunning;
        }

        private void ChangeTimeSpeed(Animator animator, float dt)
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

        private void TimeTravel(Animator animator, float dt)
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

        private enum Keys : byte
        {
            P = 1 << 0,
            K = 1 << 1,
            L = 1 << 2,
        }
    }
}
