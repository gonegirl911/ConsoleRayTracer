using System.Numerics;

namespace ConsoleRayTracer;

sealed class Camera : ICamera, IEventHandler
{
    Vector3 _origin;
    Vector3 _forward;
    Vector3 _right;
    Vector3 _up;
    float _yaw;
    float _pitch;
    float _width;
    readonly float _height;
    Controller _controller;

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
        _right = Right(_forward);
        _up = Up(_forward, _right);
        _yaw = float.Atan2(_forward.Z, _forward.X);
        _pitch = float.Asin(_forward.Y);
        _height = float.Tan(verticalFov * float.Pi / 360F * 0.5F);
        _width = _height * aspectRatio;
        _controller = new(speed, sensitivity);
    }

    public Ray CastRay(float s, float t)
    {
        var screenX = (-1F + s * 2F) * _width;
        var screenY = (1F - t * 2F) * _height;
        return new(_origin, _forward + _right * screenX + _up * screenY);
    }

    public void Handle(Event? ev, TimeSpan dt)
    {
        _controller.Handle(ev);
        _controller.ApplyUpdates(this, dt);
    }

    static Vector3 Forward(float yaw, float pitch) =>
        new(float.Cos(yaw) * float.Cos(pitch), float.Sin(pitch), float.Sin(yaw) * float.Cos(pitch));

    static Vector3 Right(Vector3 forward) => Vector3.Normalize(Vector3.Cross(Vector3.UnitY, forward));

    static Vector3 Up(Vector3 forward, Vector3 right) => Vector3.Cross(forward, right);

    struct Controller(float speed, float sensitivity)
    {
        Keys _relevantKeys = 0;
        Keys _keyHistory = 0;
        float _aspectRatio = 0F;

        public void Handle(Event? ev)
        {
            if (ev?.KeyEvent is KeyEvent keyEvent)
            {
                OnKeyEvent(keyEvent);
            }
            else if (ev?.ResizeEvent is ResizeEvent resizeEvent)
            {
                _aspectRatio = resizeEvent.AspectRatio;
            }
        }

        public void ApplyUpdates(Camera camera, TimeSpan dt)
        {
            if (_relevantKeys != 0)
            {
                ApplyRotation(camera, dt);
                ApplyMovement(camera, dt);
            }

            if (_aspectRatio != 0F)
            {
                camera._width = camera._height * _aspectRatio;
                _aspectRatio = 0F;
            }
        }

        void OnKeyEvent(KeyEvent keyEvent)
        {
            var (key, oppositeKey) = KeyPair(keyEvent.Key);
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

        readonly void ApplyRotation(Camera camera, TimeSpan dt)
        {
            const float VERTICAL_BOUND = float.Pi * 0.5F - 1e-7F;

            var dr = sensitivity * (float)dt.TotalSeconds;

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
            camera._right = Right(camera._forward);
            camera._up = Up(camera._forward, camera._right);
        }

        readonly void ApplyMovement(Camera camera, TimeSpan dt)
        {
            var direction = Vector3.Zero;
            var forward = Vector3.Cross(camera._right, Vector3.UnitY);

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
                direction -= camera._right;
            }
            else if ((_relevantKeys & Keys.D) != 0)
            {
                direction += camera._right;
            }

            if ((_relevantKeys & Keys.Spacebar) != 0)
            {
                ++direction.Y;
            }
            else if ((_relevantKeys & Keys.Z) != 0)
            {
                --direction.Y;
            }

            if (direction != Vector3.Zero)
            {
                camera._origin += Vector3.Normalize(direction) * speed * (float)dt.TotalSeconds;
            }
        }

        static (Keys, Keys) KeyPair(ConsoleKey key) =>
            key switch
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
                _ => default,
            };

        enum Keys : ushort
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
