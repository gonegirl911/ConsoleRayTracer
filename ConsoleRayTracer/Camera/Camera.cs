namespace ConsoleRayTracer;

struct Camera : ICamera
{
    private Vector3 _origin;
    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _up;

    private readonly float _w;
    private readonly float _h;

    private float _yaw;
    private float _pitch;

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

        _yaw = (float)Math.Atan2(_forward.X, _forward.Z);
        _pitch = (float)Math.Asin(-_forward.Y);

        _origin = lookFrom;
        _speed = speed;
        _sensitivity = sensitivity;
    }

    public Ray GetRay(float s, float t) =>
        new(_origin, _forward + (s * 2 - 1) * _w * _right + (t * 2 - 1) * _h * _up);

    public void Move(ConsoleKey key, float dt)
    {
        _origin += key switch
        {
            ConsoleKey.W => _speed * dt * _forward,
            ConsoleKey.A => _speed * dt * -_right,
            ConsoleKey.S => _speed * dt * -_forward,
            ConsoleKey.D => _speed * dt * _right,
            ConsoleKey.Spacebar => _speed * dt * _up,
            ConsoleKey.Z => _speed * dt * -_up,
            _ => new(0f),
        };

        _yaw += key switch
        {
            ConsoleKey.LeftArrow => _sensitivity * dt,
            ConsoleKey.RightArrow => -_sensitivity * dt,
            _ => 0f,
        };

        _pitch += key switch
        {
            ConsoleKey.UpArrow => _sensitivity * dt,
            ConsoleKey.DownArrow => -_sensitivity * dt,
            _ => 0f,
        };

        _pitch = Math.Clamp(-(float)Math.PI / 2, _pitch, (float)Math.PI / 2);

        var (sinYaw, cosYaw) = Math.SinCos(_yaw);
        var (sinPitch, cosPitch) = Math.SinCos(_pitch);

        //_forward = new((float)(cosYaw * cosPitch), (float)sinPitch, (float)(sinYaw * cosPitch));
        //_right = new((float)-sinYaw, 0f, (float)cosYaw);
        //_up = Vector3.Cross(_forward, _right);
    }
}
