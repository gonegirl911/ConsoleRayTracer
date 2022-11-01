namespace ConsoleRayTracer;

public sealed class Camera : ICamera, IEventHandler
{
    private float _width;
    private readonly float _height;
    private readonly float _speed;
    private readonly float _sensitivity;

    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _up;
    private Vector3 _origin;

    private float _yaw;
    private float _pitch;

    private readonly Controller _controller;

    public Camera(
        Vector3 lookFrom,
        Vector3 lookAt,
        float vFov,
        float aspect,
        float speed,
        float sensitivity
    )
    {
        _height = float.Tan(0.5f * vFov * float.Pi / 360f);
        _width = _height * aspect;
        _speed = speed / 1000f;
        _sensitivity = sensitivity / 1000f;

        _origin = lookFrom;
        _forward = Vector3.Normalize(lookAt - lookFrom);
        _right = Vector3.Normalize(new(_forward.Z, 0f, -_forward.X));
        _up = Vector3.Cross(_forward, _right);

        _yaw = float.Atan2(_forward.Z, _forward.X);
        _pitch = float.Asin(_forward.Y);

        _controller = new();
    }

    public Ray CastRay(float s, float t)
    {
        var px = (2f * s - 1f) * _width;
        var py = (1f - 2f * t) * _height;
        return new(_origin, _right * px + _up * py + _forward);
    }

    public void Handle(in Event? ev, float dt)
    {
        _controller.Handle(ev);
        _controller.ApplyUpdates(this, dt);
    }

    private sealed class Controller
    {
        private Keys _relevantKeys;
        private Keys _keyHistory;
        private float _aspectRatio;

        public Controller()
        {
            _relevantKeys = 0;
            _keyHistory = 0;
            _aspectRatio = 0f;
        }

        public void Handle(in Event? ev)
        {
            if (ev.HasValue)
            {
                if (ev.Value.Variant is EventVariant.Key)
                {
                    OnKeyEvent(ev.Value.Data.KeyEvent);
                }
                else if (ev.Value.Variant is EventVariant.Resize)
                {
                    OnResizeEvent(ev.Value.Data.ResizeEvent);
                }
            }
        }

        public void ApplyUpdates(Camera camera, float dt)
        {
            if (_aspectRatio != 0.0f)
            {
                ApplyResize(camera);
                _aspectRatio = 0.0f;
            }

            if (_relevantKeys != 0)
            {
                ApplyRotation(camera, dt);
                ApplyMovement(camera, dt);
            }
        }

        private void OnKeyEvent(in KeyEvent keyEvent)
        {
            var (key, opp) = keyEvent.Key switch
            {
                ConsoleKey.W => (Keys.W, Keys.S),
                ConsoleKey.A => (Keys.A, Keys.D),
                ConsoleKey.S => (Keys.S, Keys.W),
                ConsoleKey.D => (Keys.D, Keys.A),
                ConsoleKey.Z => (Keys.Z, Keys.Spacebar),
                ConsoleKey.Spacebar => (Keys.Spacebar, Keys.Z),
                ConsoleKey.UpArrow => (Keys.UpArrow, Keys.DownArrow),
                ConsoleKey.LeftArrow => (Keys.LeftArrow, Keys.RightArrow),
                ConsoleKey.DownArrow => (Keys.DownArrow, Keys.UpArrow),
                ConsoleKey.RightArrow => (Keys.RightArrow, Keys.LeftArrow),
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

        private void OnResizeEvent(in ResizeEvent resizeEvent)
        {
            _aspectRatio = resizeEvent.AspectRatio;
        }

        private void ApplyResize(Camera camera)
        {
            camera._width = camera._height * _aspectRatio;
        }

        private void ApplyRotation(Camera camera, float dt)
        {
            const float SAFE_FRAC_PI_2 = float.Pi / 2f - 0.0001f;

            var dr = camera._sensitivity * dt;

            if ((_relevantKeys & Keys.UpArrow) != 0)
            {
                camera._pitch += dr;
            }
            else if ((_relevantKeys & Keys.DownArrow) != 0)
            {
                camera._pitch -= dr;
            }

            if ((_relevantKeys & Keys.LeftArrow) != 0)
            {
                camera._yaw += dr;
            }
            else if ((_relevantKeys & Keys.RightArrow) != 0)
            {
                camera._yaw -= dr;
            }

            camera._yaw %= float.Tau;
            camera._pitch = float.Clamp(camera._pitch, -SAFE_FRAC_PI_2, SAFE_FRAC_PI_2);

            var (sinYaw, cosYaw) = float.SinCos(camera._yaw);
            var (sinPitch, cosPitch) = float.SinCos(camera._pitch);

            camera._forward = new(cosYaw * cosPitch, sinPitch, sinYaw * cosPitch);
            camera._right = Vector3.Normalize(new(camera._forward.Z, 0f, -camera._forward.X));
            camera._up = Vector3.Cross(camera._forward, camera._right);
        }

        private void ApplyMovement(Camera camera, float dt)
        {
            var dp = camera._speed * dt;
            var forward = Vector3.Normalize(camera._forward with { Y = 0f });
            var right = camera._right;
            var up = Vector3.UnitY;

            if ((_relevantKeys & Keys.W) != 0)
            {
                camera._origin += forward * dp;
            }
            else if ((_relevantKeys & Keys.S) != 0)
            {
                camera._origin -= forward * dp;
            }

            if ((_relevantKeys & Keys.A) != 0)
            {
                camera._origin -= right * dp;
            }
            else if ((_relevantKeys & Keys.D) != 0)
            {
                camera._origin += right * dp;
            }

            if ((_relevantKeys & Keys.Z) != 0)
            {
                camera._origin -= up * dp;
            }
            else if ((_relevantKeys & Keys.Spacebar) != 0)
            {
                camera._origin += up * dp;
            }
        }

        private enum Keys : ushort
        {
            W = 1 << 0,
            A = 1 << 1,
            S = 1 << 2,
            D = 1 << 3,
            Z = 1 << 4,
            Spacebar = 1 << 5,
            UpArrow = 1 << 6,
            LeftArrow = 1 << 7,
            DownArrow = 1 << 8,
            RightArrow = 1 << 9,
        }
    }
}
