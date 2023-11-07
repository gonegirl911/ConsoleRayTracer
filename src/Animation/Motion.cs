using System.Numerics;

namespace ConsoleRayTracer;

interface IMotion<out T>
{
    T GetValue(float progress);
}

readonly record struct LinearMotion(float Motion) : IMotion<float>
{
    public float GetValue(float progress) => Motion * progress;
}

readonly record struct LinearPath(Vector3 Path) : IMotion<Vector3>
{
    public Vector3 GetValue(float progress) => Path * progress;
}

readonly record struct CircularPath<A>(float Radius) : IMotion<Vector3>
    where A : IAxis
{
    public Vector3 GetValue(float progress)
    {
        var theta = float.Tau * progress;
        return A.Apply(new(0f, float.Cos(theta), float.Sin(theta))) * Radius;
    }
}
