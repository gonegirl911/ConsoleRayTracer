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

    public Camera(
        Vector3 lookFrom,
        Vector3 lookAt,
        float vFov,
        float aspect,
        float speed,
        float sensitivity
    )
    {
        _height = float.Tan(vFov * float.Pi / 360f);
        _width = _height * aspect;
        _speed = speed / 1000f;
        _sensitivity = sensitivity / 1000f;

        _origin = lookFrom;
        _forward = Vector3.Normalize(lookAt - lookFrom);
        _right = Vector3.Normalize(new(_forward.Z, 0f, -_forward.X));
        _up = Vector3.Cross(_forward, _right);

        _yaw = float.Atan2(_forward.Z, _forward.X);
        _pitch = float.Asin(_forward.Y);
    }

    public Ray CastRay(float s, float t)
    {
        var px = (2f * s - 1f) * _width;
        var py = (1f - 2f * t) * _height;
        return new(_origin, _right * px + _up * py + _forward);
    }

    public void Handle(in Event? ev, float dt)
    {
        if (ev is Event current)
        {
            if (current.KeyEvent is KeyEvent keyEvent)
            {
                var _ = Move(keyEvent.PressedKey, dt) || Rotate(keyEvent.PressedKey, dt);
            }
            else if (current.ResizeEvent is ResizeEvent resizeEvent)
            {
                Adjust(resizeEvent.AspectRatio);
            }
        }
    }

    private void Adjust(float aspect)
    {
        _width = _height * aspect;
    }

    private bool Move(ConsoleKey? key, float dt)
    {
        var dp = _speed * dt;
        var forward = Vector3.Normalize(_forward with { Y = 0f });
        var right = _right;
        var up = Vector3.UnitY;

        switch (key)
        {
            case ConsoleKey.W:
                _origin += dp * forward;
                break;
            case ConsoleKey.A:
                _origin -= dp * right;
                break;
            case ConsoleKey.S:
                _origin -= dp * forward;
                break;
            case ConsoleKey.D:
                _origin += dp * right;
                break;
            case ConsoleKey.Spacebar:
                _origin += dp * up;
                break;
            case ConsoleKey.Z:
                _origin -= dp * up;
                break;
            default:
                return false;
        }

        return true;      
    }

    private bool Rotate(ConsoleKey? key, float dt)
    {
        const float SAFE_FRAC_PI_2 = float.Pi / 2f - 0.0001f;

        var dr = _sensitivity * dt;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                _pitch += dr;
                break;
            case ConsoleKey.LeftArrow:
                _yaw += dr;
                break;
            case ConsoleKey.DownArrow:
                _pitch -= dr;
                break;
            case ConsoleKey.RightArrow:
                _yaw -= dr;
                break;
            default:
                return false;
        }

        _yaw %= float.Tau;
        _pitch = float.Clamp(_pitch, -SAFE_FRAC_PI_2, SAFE_FRAC_PI_2);

        var (sinYaw, cosYaw) = float.SinCos(_yaw);
        var (sinPitch, cosPitch) = float.SinCos(_pitch);

        _forward = new(cosYaw * cosPitch, sinPitch, sinYaw * cosPitch);
        _right = Vector3.Normalize(new(_forward.Z, 0f, -_forward.X));
        _up = Vector3.Cross(_forward, _right);

        return true;
    }
}
