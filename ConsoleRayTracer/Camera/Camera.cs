namespace ConsoleRayTracer;

public sealed class Camera : ICamera
{
    private const float SAFE_FRAC_PI_2 = float.Pi / 2f - 0.0001f;

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

    public void Update(ConsoleKey? key, float dt, float aspect)
    {
        Adjust(aspect);
        Move(key, dt);
        Rotate(key, dt);
    }

    public void Adjust(float aspect)
    {
        _width = _height * aspect;
    }

    public void Move(ConsoleKey? key, float dt)
    {
        var dp = _speed * dt;
        var forward = Vector3.Normalize(_forward with { Y = 0f });
        var right = _right;
        var up = Vector3.UnitY;

        _origin += key switch
        {
            ConsoleKey.W => dp * forward,
            ConsoleKey.A => -dp * right,
            ConsoleKey.S => -dp * forward,
            ConsoleKey.D => dp * right,
            ConsoleKey.Spacebar => dp * up,
            ConsoleKey.Z => -dp * up,
            _ => new(0f),
        };        
    }

    public void Rotate(ConsoleKey? key, float dt)
    {
        var dr = _sensitivity * dt;

        (_yaw, _pitch) = key switch
        {
            ConsoleKey.LeftArrow => (_yaw + dr, _pitch),
            ConsoleKey.RightArrow => (_yaw - dr, _pitch),
            ConsoleKey.UpArrow => (_yaw, _pitch + dr),
            ConsoleKey.DownArrow => (_yaw, _pitch - dr),
            _ => (_yaw, _pitch),
        };
        _yaw %= float.Tau;
        _pitch = float.Clamp(_pitch, -SAFE_FRAC_PI_2, SAFE_FRAC_PI_2);

        var (sinYaw, cosYaw) = float.SinCos(_yaw);
        var (sinPitch, cosPitch) = float.SinCos(_pitch);

        _forward = new(cosYaw * cosPitch, sinPitch, sinYaw * cosPitch);
        _right = Vector3.Normalize(new(_forward.Z, 0f, -_forward.X));
        _up = Vector3.Cross(_forward, _right);
    }
}
