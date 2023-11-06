using System.Numerics;

namespace ConsoleRayTracer;

readonly record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public Vector3 PointAt(float t) => Origin + Direction * t;
    public Vector3 Opposite(Vector3 vector) => -float.Sign(Vector3.Dot(vector, Direction)) * vector;
}

interface IAxis
{
    int Axis { get; }
    int Main { get; }
    int Secondary { get; }
    Vector3 Unit { get; }

    float GetAxis(Vector3 vector);
    float GetMain(Vector3 vector);
    float GetSecondary(Vector3 vector);
    Vector3 Apply(Vector3 vector);
}

readonly struct AxisX : IAxis
{
    public int Axis => 0;
    public int Main => 2;
    public int Secondary => 1;
    public Vector3 Unit => Vector3.UnitX;

    public float GetAxis(Vector3 vector) => vector.X;
    public float GetMain(Vector3 vector) => vector.Z;
    public float GetSecondary(Vector3 vector) => vector.Y;
    public Vector3 Apply(Vector3 vector) => new(vector.X, vector.Z, vector.Y);
}

readonly struct AxisY : IAxis
{
    public int Axis => 1;
    public int Main => 0;
    public int Secondary => 2;
    public Vector3 Unit => Vector3.UnitY;

    public float GetAxis(Vector3 vector) => vector.Y;
    public float GetMain(Vector3 vector) => vector.X;
    public float GetSecondary(Vector3 vector) => vector.Z;
    public Vector3 Apply(Vector3 vector) => new(vector.Y, vector.X, vector.Z);
}

readonly struct AxisZ : IAxis
{
    public int Axis => 2;
    public int Main => 0;
    public int Secondary => 1;
    public Vector3 Unit => Vector3.UnitZ;

    public float GetAxis(Vector3 vector) => vector.Z;
    public float GetMain(Vector3 vector) => vector.X;
    public float GetSecondary(Vector3 vector) => vector.Y;
    public Vector3 Apply(Vector3 vector) => new(vector.Y, vector.Z, vector.X);
}
