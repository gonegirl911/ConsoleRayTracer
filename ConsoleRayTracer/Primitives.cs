namespace ConsoleRayTracer;

readonly record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public Vector3 PointAt(float t) => Origin + Direction * t;

    public (bool, Vector3) OppositeNormal(Vector3 normal) =>
        Vector3.Dot(Direction, normal) < 0f
            ? (true, normal)
            : (false, -normal);
}

static class Vector3Extensions
{
    public static void Set(ref this Vector3 vector3, int axis, float value)
    {
        if (axis == 0)
            vector3.X = value;
        else if (axis == 1)
            vector3.Y = value;
        else if (axis == 2)
            vector3.Z = value;
        else
            throw new IndexOutOfRangeException("axis must be between 0 and 2");
    }
}

interface IAxis
{
    int Axis { get; }
    int Main { get; }
    int Secondary { get; }
}

readonly struct AxisX : IAxis
{
    public int Axis => 0;
    public int Main => 2;
    public int Secondary => 1;
}

readonly struct AxisY : IAxis
{
    public int Axis => 1;
    public int Main => 0;
    public int Secondary => 2;
}

readonly struct AxisZ : IAxis
{
    public int Axis => 2;
    public int Main => 0;
    public int Secondary => 1;
}
