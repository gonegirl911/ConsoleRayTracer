using System.Numerics;

namespace ConsoleRayTracer;

public interface IMotion<out T>
{
    T GetValue(float interpolation);
}

public readonly record struct LinearMotion(float Motion) : IMotion<float>
{
    public float GetValue(float interpolation) => Motion * interpolation;
}

public readonly record struct LinearPath(Vector3 Path) : IMotion<Vector3>
{
    public Vector3 GetValue(float interpolation) => Path * interpolation;
}

public readonly record struct CircularPath<A>(A Axis, float Radius) : IMotion<Vector3>
    where A : IAxis
{
    public Vector3 GetValue(float interpolation)
    {
        var theta = float.Tau * interpolation;
        return Axis.Apply(new(0f, float.Cos(theta), float.Sin(theta))) * Radius;
    }
}
