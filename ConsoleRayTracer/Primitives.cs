namespace ConsoleRayTracer;

public readonly record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public Vector3 PointAt(float t) => Origin + Direction * t;

    public Vector3 Opposite(in Vector3 vector3) =>
        -Math.Sign(Vector3.Dot(Direction, vector3)) * vector3;
}

public interface IAxis
{
    int Axis { get; }
    int Main { get; }
    int Secondary { get; }
    Vector3 Unit { get; }

    float GetAxis(Vector3 vector3);
    float GetMain(Vector3 vector3);
    float GetSecondary(Vector3 vector3);
    Vector3 Apply(Vector3 vector3);
}

public readonly record struct AxisX : IAxis
{
    public int Axis => 0;
    public int Main => 2;
    public int Secondary => 1;
    public Vector3 Unit => Vector3.UnitX;

    public float GetAxis(Vector3 vector3) => vector3.X;
    public float GetMain(Vector3 vector3) => vector3.Z;
    public float GetSecondary(Vector3 vector3) => vector3.Y;
    public Vector3 Apply(Vector3 vector3) => new(vector3.X, vector3.Z, vector3.Y);
}

public readonly record struct AxisY : IAxis
{
    public int Axis => 1;
    public int Main => 0;
    public int Secondary => 2;
    public Vector3 Unit => Vector3.UnitY;

    public float GetAxis(Vector3 vector3) => vector3.Y;
    public float GetMain(Vector3 vector3) => vector3.X;
    public float GetSecondary(Vector3 vector3) => vector3.Z;
    public Vector3 Apply(Vector3 vector3) => new(vector3.Y, vector3.X, vector3.Z);
}

public readonly record struct AxisZ : IAxis
{
    public int Axis => 2;
    public int Main => 0;
    public int Secondary => 1;
    public Vector3 Unit => Vector3.UnitZ;

    public float GetAxis(Vector3 vector3) => vector3.Z;
    public float GetMain(Vector3 vector3) => vector3.X;
    public float GetSecondary(Vector3 vector3) => vector3.Y;
    public Vector3 Apply(Vector3 vector3) => new(vector3.Y, vector3.Z, vector3.X);
}
