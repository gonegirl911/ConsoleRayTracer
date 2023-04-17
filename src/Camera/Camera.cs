using System.Numerics;

namespace ConsoleRayTracer;

sealed class Camera : ICamera, IEventHandler
{
    private Vector3 _origin;
    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _up;
    private float _yaw;
    private float _pitch;
    private float _width;
    private readonly float _height;
    private readonly Controller _controller;

    public Camera(
        Vector3 lookFrom,
        Vector3 lookAt,
        float verticalFov,
        float aspectRatio,
        float speed,
        float sensitivity
    )
    {
        _origin = lookFrom;
        _forward = Vector3.Normalize(lookAt - lookFrom);
        _right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, _forward));
        _up = Vector3.Cross(_forward, _right);
        _yaw = float.Atan2(_forward.Z, _forward.X);
        _pitch = float.Asin(_forward.Y);
        _height = float.Tan(verticalFov * float.Pi / 360f * 0.5f);
        _width = _height * aspectRatio;
        _controller = new(speed, sensitivity);
    }

    public Ray CastRay(float s, float t)
    {
        var sx = (-1f + s * 2f) * _width;
        var sy = (1f - t * 2f) * _height;
        return new(_origin, _forward + _right * sx + _up * sy);
    }

    public void Handle(Event? ev, TimeSpan dt)
    {
        _controller.Handle(ev);
        _controller.ApplyUpdates(this, dt);
    }

    private sealed class Controller
    {
        private Keys _relevantKeys;
        private Keys _keyHistory;
        private float _aspectRatio;
        private readonly float _speed;
        private readonly float _sensitivity;

        public Controller(float speed, float sensitivity)
        {
            _relevantKeys = 0;
            _keyHistory = 0;
            _aspectRatio = 0f;
            _speed = speed;
            _sensitivity = sensitivity;
        }

        public void Handle(Event? ev)
        {
            if (ev?.KeyEvent is KeyEvent keyEvent)
            {
                OnKeyEvent(keyEvent);
            }
            else if (ev?.ResizeEvent is ResizeEvent resizeEvent)
            {
                OnResizeEvent(resizeEvent);
            }
        }

        public void ApplyUpdates(Camera camera, TimeSpan dt)
        {
            if (_relevantKeys != 0)
            {
                ApplyRotation(camera, dt);
                ApplyMovement(camera, dt);
            }

            if (_aspectRatio != 0.0f)
            {
                ApplyResize(camera);
                _aspectRatio = 0.0f;
            }
        }

        private void OnKeyEvent(KeyEvent ev)
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

        private void OnResizeEvent(ResizeEvent ev)
        {
            _aspectRatio = ev.AspectRatio;
        }

        private void ApplyRotation(Camera camera, TimeSpan dt)
        {
            const float VERTICAL_BOUND = float.Pi * 0.5f - 0.0001f;

            var dr = _sensitivity * (float)dt.TotalSeconds;

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
            camera._pitch = float.Clamp(camera._pitch, -VERTICAL_BOUND, VERTICAL_BOUND);

            camera._forward = Forward(camera._yaw, camera._pitch);
            camera._right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, camera._forward));
            camera._up = Vector3.Cross(camera._forward, camera._right);
        }

        private void ApplyMovement(Camera camera, TimeSpan dt)
        {
            var direction = Vector3.Zero;
            var right = camera._right;
            var up = Vector3.UnitY;
            var forward = Vector3.Cross(right, up);

            if ((_relevantKeys & Keys.W) != 0)
            {
                direction += forward;
            }
            else if ((_relevantKeys & Keys.S) != 0)
            {
                direction -= forward;
            }

            if ((_relevantKeys & Keys.A) != 0)
            {
                direction -= right;
            }
            else if ((_relevantKeys & Keys.D) != 0)
            {
                direction += right;
            }

            if ((_relevantKeys & Keys.Spacebar) != 0)
            {
                direction += up;
            }
            else if ((_relevantKeys & Keys.Z) != 0)
            {
                direction -= up;
            }

            if (direction != Vector3.Zero)
            {
                camera._origin += Vector3.Normalize(direction) * _speed * (float)dt.TotalSeconds;
            }
        }

        private void ApplyResize(Camera camera)
        {
            camera._width = camera._height * _aspectRatio;
        }

        private static (Keys, Keys) KeyPair(KeyEvent ev) =>
            ev.Key switch
            {
                ConsoleKey.W => (Keys.W, Keys.S),
                ConsoleKey.A => (Keys.A, Keys.D),
                ConsoleKey.S => (Keys.S, Keys.W),
                ConsoleKey.D => (Keys.D, Keys.A),
                ConsoleKey.Spacebar => (Keys.Spacebar, Keys.Z),
                ConsoleKey.Z => (Keys.Z, Keys.Spacebar),
                ConsoleKey.UpArrow => (Keys.UpArrow, Keys.DownArrow),
                ConsoleKey.LeftArrow => (Keys.LeftArrow, Keys.RightArrow),
                ConsoleKey.DownArrow => (Keys.DownArrow, Keys.UpArrow),
                ConsoleKey.RightArrow => (Keys.RightArrow, Keys.LeftArrow),
                _ => (default, default),
            };

        private static Vector3 Forward(float yaw, float pitch) =>
            new(float.Cos(yaw) * float.Cos(pitch), float.Sin(pitch), float.Sin(yaw) * float.Cos(pitch));

        private enum Keys : ushort
        {
            W = 1 << 0,
            A = 1 << 1,
            S = 1 << 2,
            D = 1 << 3,
            Spacebar = 1 << 4,
            Z = 1 << 5,
            UpArrow = 1 << 6,
            LeftArrow = 1 << 7,
            DownArrow = 1 << 8,
            RightArrow = 1 << 9,
        }
    }
}
