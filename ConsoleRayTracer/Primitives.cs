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
    public static ref float Get(ref this Vector3 vector3, int axis)
    {
        switch (axis)
        {
            case 0:
                return ref vector3.X;
            case 1:
                return ref vector3.Y;
            case 2:
                return ref vector3.Z;
            default:
                throw new IndexOutOfRangeException("axis must be between 0 and 2");
        }            
    }
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
