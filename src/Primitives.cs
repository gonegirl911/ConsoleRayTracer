using System.Numerics;

namespace ConsoleRayTracer;

readonly record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public Vector3 PointAt(float t) => Origin + Direction * t;
    public Vector3 Opposite(Vector3 vector) => -float.Sign(Vector3.Dot(vector, Direction)) * vector;
}

interface IAxis
{
    static abstract Vector3 Unit { get; }
    static abstract float GetAxis(Vector3 vector);
    static abstract float GetMain(Vector3 vector);
    static abstract float GetSecondary(Vector3 vector);
    static abstract Vector3 Apply(Vector3 vector);
}

readonly struct AxisX : IAxis
{
    public static Vector3 Unit => Vector3.UnitX;
    public static float GetAxis(Vector3 vector) => vector.X;
    public static float GetMain(Vector3 vector) => vector.Z;
    public static float GetSecondary(Vector3 vector) => vector.Y;
    public static Vector3 Apply(Vector3 vector) => new(vector.X, vector.Z, vector.Y);
}

readonly struct AxisY : IAxis
{
    public static Vector3 Unit => Vector3.UnitY;
    public static float GetAxis(Vector3 vector) => vector.Y;
    public static float GetMain(Vector3 vector) => vector.X;
    public static float GetSecondary(Vector3 vector) => vector.Z;
    public static Vector3 Apply(Vector3 vector) => new(vector.Y, vector.X, vector.Z);
}

readonly struct AxisZ : IAxis
{
    public static Vector3 Unit => Vector3.UnitZ;
    public static float GetAxis(Vector3 vector) => vector.Z;
    public static float GetMain(Vector3 vector) => vector.X;
    public static float GetSecondary(Vector3 vector) => vector.Y;
    public static Vector3 Apply(Vector3 vector) => new(vector.Y, vector.Z, vector.X);
}
