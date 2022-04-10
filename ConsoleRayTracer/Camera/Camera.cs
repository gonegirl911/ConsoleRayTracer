namespace ConsoleRayTracer;

class Camera : ICamera
{
    private const float SAFE_FRAC_PI_2 = (float)Math.PI / 2f - 0.0001f;

    private Vector3 _origin;
    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _up;

    private float _yaw;
    private float _pitch;

    private readonly float _w;
    private readonly float _h;
    private Vector3 _u;
    private Vector3 _v;

    private readonly Vector3 _vUp;
    private readonly float _speed;
    private readonly float _sensitivity;

    public Camera(
        Vector3 lookFrom,
        Vector3 lookAt,
        Vector3 vUp,
        float vFov,
        float aspectRatio,
        float speed,
        float sensitivity
    )
    {
        _origin = lookFrom;
        _forward = Vector3.Normalize(lookAt - lookFrom);
        _right = Vector3.Normalize(Vector3.Cross(vUp, _forward));
        _up = Vector3.Cross(_forward, _right);

        _yaw = (float)Math.Atan2(_forward.Z, _forward.X);
        _pitch = (float)Math.Asin(_forward.Y);

        _h = (float)Math.Tan(vFov * Math.PI / 360d) * 2f;
        _w = _h * aspectRatio;
        _u = _w * _right;
        _v = _h * _up;

        _vUp = vUp;
        _speed = speed / 1000f;
        _sensitivity = sensitivity / 1000f;
    }

    public Ray GetRay(float s, float t) =>
        new(_origin, _forward + (s * 2f - 1f) * _u + (t * 2f - 1f) * _v);

    public void Move(ConsoleKey? key, float dt)
    {
        var dp = _speed * dt;
        var forward = Vector3.Normalize(_forward with { Y = 0f });
        var right = _right;
        var up = _vUp;

        _origin += key switch
        {
            ConsoleKey.W => dp * forward,
            ConsoleKey.A => -dp * right,
            ConsoleKey.S => -dp * forward,
            ConsoleKey.D => dp * right,
            ConsoleKey.Spacebar => dp * up,
            ConsoleKey.Z => -dp * up,
            _ => Rotate(key, dt),
        };        
    }

    private Vector3 Rotate(ConsoleKey? key, float dt)
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
        _yaw %= (float)Math.PI * 2f;
        _pitch = Math.Clamp(_pitch, -SAFE_FRAC_PI_2, SAFE_FRAC_PI_2);

        var (sinYaw, cosYaw) = Math.SinCos(_yaw);
        var (sinPitch, cosPitch) = Math.SinCos(_pitch);

        _forward = new((float)(cosYaw * cosPitch), (float)sinPitch, (float)(sinYaw * cosPitch));
        _right = Vector3.Normalize(Vector3.Cross(_vUp, _forward));
        _up = Vector3.Cross(_forward, _right);

        _u = _w * _right;
        _v = _h * _up;

        return new(0);
    }
}
