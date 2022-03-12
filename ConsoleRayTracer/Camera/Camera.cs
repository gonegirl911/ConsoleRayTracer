namespace ConsoleRayTracer;

class Camera : ICamera
{
    private const float SAFE_FRAC_PI_2 = (float)Math.PI / 2 - 0.0001f;

    private Vector3 _origin;
    private float _yaw;
    private float _pitch;

    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _up;

    private readonly float _w;
    private readonly float _h;

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
        var theta = vFov * (float)Math.PI / 180;
        _h = (float)Math.Tan(theta / 2) * 2;
        _w = _h * aspectRatio;

        _forward = Vector3.Normalize(lookAt - lookFrom);
        _right = Vector3.Normalize(Vector3.Cross(vUp, _forward));
        _up = Vector3.Cross(_forward, _right);

        _yaw = (float)Math.Atan2(_forward.Z, _forward.X);
        _pitch = (float)Math.Asin(_forward.Y);

        _origin = lookFrom;
        _vUp = vUp;
        _speed = speed;
        _sensitivity = sensitivity;
    }

    public Ray GetRay(float s, float t) =>
        new(_origin, _forward + (s * 2 - 1) * _w * _right + (t * 2 - 1) * _h * _up);

    public void Move(ConsoleKey? key, float dt)
    {
        var dp = _speed * dt;

        var forward = Vector3.Normalize(_forward with { Y = 0 });
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

        _yaw += key switch
        {
            ConsoleKey.LeftArrow => dr,
            ConsoleKey.RightArrow => -dr,
            _ => 0f,
        };

        _pitch += key switch
        {
            ConsoleKey.UpArrow => dr,
            ConsoleKey.DownArrow => -dr,
            _ => 0f,
        };

        _yaw %= (float)Math.PI * 2;
        _pitch = Math.Clamp(_pitch, -SAFE_FRAC_PI_2, SAFE_FRAC_PI_2);

        var (sinYaw, cosYaw) = Math.SinCos(_yaw);
        var (sinPitch, cosPitch) = Math.SinCos(_pitch);

        _forward = new((float)(cosYaw * cosPitch), (float)sinPitch, (float)(sinYaw * cosPitch));
        _right = Vector3.Normalize(Vector3.Cross(_vUp, _forward));
        _up = Vector3.Cross(_forward, _right);

        return new(0);
    }
}
