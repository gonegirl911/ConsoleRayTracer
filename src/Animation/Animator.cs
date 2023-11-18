namespace ConsoleRayTracer;

sealed class Animator(float speed, float sensitivity, bool isRunning = true) : IEventHandler
{
    float _timeElapsed = 0f;
    Controller _controller = new(speed, sensitivity, isRunning);

    public void Handle(Event? ev, TimeSpan dt)
    {
        _controller.Handle(ev);
        _controller.ApplyUpdates(this, dt);
    }

    public void MoveForward<E>(in E animatedEntity) where E : IAnimatedEntity
    {
        animatedEntity.Update(_timeElapsed);
    }

    struct Controller(float speed, float sensitivity, bool isRunning)
    {
        Keys _relevantKeys = 0;
        Keys _keyHistory = 0;

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
                isRunning = !isRunning;
                _relevantKeys &= ~Keys.P;
            }

            if (isRunning)
            {
                Continue(animator, dt);
            }
            else
            {
                TimeTravel(animator, dt);
            }
        }

        void OnKeyEvent(KeyEvent keyEvent)
        {
            var (key, oppositeKey) = KeyPair(keyEvent);
            if (keyEvent.State == KeyState.Pressed)
            {
                _relevantKeys |= key;
                _relevantKeys &= ~oppositeKey;
                _keyHistory |= key;
            }
            else
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
            var ds = sensitivity * (float)dt.TotalSeconds;
            if ((_relevantKeys & Keys.L) != 0)
            {
                speed += ds;
            }
            else if ((_relevantKeys & Keys.K) != 0)
            {
                speed -= ds;
            }
            animator._timeElapsed += (float)dt.TotalMilliseconds * speed;
        }

        readonly void TimeTravel(Animator animator, TimeSpan dt)
        {
            var de = (float)dt.TotalMilliseconds * speed;
            if ((_relevantKeys & Keys.L) != 0)
            {
                animator._timeElapsed += de;
            }
            else if ((_relevantKeys & Keys.K) != 0)
            {
                animator._timeElapsed -= de;
            }
        }

        readonly (Keys, Keys) KeyPair(KeyEvent keyEvent) =>
            keyEvent switch
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
