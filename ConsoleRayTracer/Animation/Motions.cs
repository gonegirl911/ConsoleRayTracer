namespace ConsoleRayTracer;

interface IMotion<T>
{
    T GetValue(float interpolation);
}

readonly record struct LinearMotion(float Path) : IMotion<float>
{
    public float GetValue(float interpolation) => Path * interpolation;
}

readonly record struct LinearPath(Vector3 Path) : IMotion<Vector3>
{
    public Vector3 GetValue(float interpolation) => Path * interpolation;
}

readonly record struct CircularPath<A>(float Radius, A Axis) : IMotion<Vector3> where A : IAxis
{
    public Vector3 GetValue(float interpolation)
    {
        var angle = Math.PI * 2f * interpolation;
        Vector3 point = new(0f);
        point.Set(Axis.Main, (float)Math.Cos(angle) * Radius);
        point.Set(Axis.Secondary, (float)Math.Sin(angle) * Radius);
        return point;
    }
}
