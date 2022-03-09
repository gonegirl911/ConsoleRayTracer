namespace ConsoleRayTracer;

readonly struct Camera : ICamera
{
    private readonly Vector3 _origin;
    private readonly Vector3 _lower_left_corner;
    private readonly Vector3 _horizontal;
    private readonly Vector3 _vertical;

    public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 vUp, float vFov, float aspectRatio)
    {
        var theta = vFov * (float)Math.PI / 180;
        var h = (float)Math.Tan(theta / 2);
        var viewportHeight = h * 2;
        var viewportWidth = viewportHeight * aspectRatio;

        var w = Vector3.Normalize(lookFrom - lookAt);
        var u = Vector3.Normalize(Vector3.Cross(vUp, w));
        var v = Vector3.Cross(w, u);

        _origin = lookFrom;
        _horizontal = viewportWidth * u;
        _vertical = viewportHeight * v;
        _lower_left_corner = _origin - _horizontal / 2 - _vertical / 2 - w;
    }

    public Ray GetRay(float s, float t) =>
        new(_origin, _lower_left_corner + s * _horizontal + t * _vertical - _origin);
}
