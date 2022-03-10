namespace ConsoleRayTracer;

struct Camera : ICamera
{
    private Vector3 _origin;
    private Vector3 _lower_left_corner;

    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _up;

    private readonly float _w;
    private readonly float _h;

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

        _forward = Vector3.Normalize(lookFrom - lookAt);
        _right = Vector3.Normalize(Vector3.Cross(vUp, _forward));
        _up = Vector3.Cross(_forward, _right);

        _origin = lookFrom;
        _lower_left_corner = _origin - _right * _w / 2 - _up * _h / 2 - _forward;
        _speed = speed;
        _sensitivity = sensitivity;
    }

    public Ray GetRay(float s, float t) =>
        new(_origin, _lower_left_corner + _right * s * _w + _up * t * _h - _origin);

    public void Move(ConsoleKey key, float dt)
    {
        switch (key)
        {
            case ConsoleKey.W:
                var mf = -_forward * _speed * dt;
                _origin += mf;
                _lower_left_corner += mf;
                break;
            case ConsoleKey.A:
                var ml = -_right * _speed * dt;
                _origin += ml;
                _lower_left_corner += ml;
                break;
            case ConsoleKey.S:
                var mb = _forward * _speed * dt;
                _origin += mb;
                _lower_left_corner += mb;
                break;
            case ConsoleKey.D:
                var mr = _right * _speed * dt;
                _origin += mr;
                _lower_left_corner += mr;
                break;
            case ConsoleKey.UpArrow:
                break;
            case ConsoleKey.LeftArrow:
                break;
            case ConsoleKey.DownArrow:
                break;
            case ConsoleKey.RightArrow:
                break;
            case ConsoleKey.Spacebar:
                var mu = _up * _speed * dt;
                _origin += mu;
                _lower_left_corner += mu;
                break;
            case ConsoleKey.Z:
                var md = -_up * _speed * dt;
                _origin += md;
                _lower_left_corner += md;
                break;
            default:
                break;
        }
    }
}
