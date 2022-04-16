namespace ConsoleRayTracer;

readonly record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public Vector3 PointAt(float t) => Origin + Direction * t;

    public Vector3 OppositeNormal(in Vector3 normal) =>
        Vector3.Dot(Direction, normal) < 0f ? normal : -normal;
}

interface IAxis
{
    int Axis { get; }
    int Main { get; }
    int Secondary { get; }
    Vector3 Unit { get; }
}

readonly struct AxisX : IAxis
{
    public int Axis => 0;
    public int Main => 2;
    public int Secondary => 1;
    public Vector3 Unit => Vector3.UnitX;
}

readonly struct AxisY : IAxis
{
    public int Axis => 1;
    public int Main => 0;
    public int Secondary => 2;
    public Vector3 Unit => Vector3.UnitY;
}

readonly struct AxisZ : IAxis
{
    public int Axis => 2;
    public int Main => 0;
    public int Secondary => 1;
    public Vector3 Unit => Vector3.UnitZ;
}

static class Vector3Extensions
{
    public static float Get(in this Vector3 vector3, int axis) =>
        axis switch
        {
            0 => vector3.X,
            1 => vector3.Y,
            2 => vector3.Z,
            _ => throw new IndexOutOfRangeException("axis must be between 0 and 2"),
        };

    public static void Set(ref this Vector3 vector3, int axis, float value)
    {
        switch (axis)
        {
            case 0:
                vector3.X = value;
                break;
            case 1:
                vector3.Y = value;
                break;
            case 2:
                vector3.Z = value;
                break;
            default:
                throw new IndexOutOfRangeException("axis must be between 0 and 2");
        }
    }
}
