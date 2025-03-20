using System.Numerics;

namespace ConsoleRayTracer;

interface IMotion<out T>
{
    T GetValue(float progress);
}

readonly struct LinearMotion(float motion) : IMotion<float>
{
    public float GetValue(float progress) => motion * progress;
}

readonly struct LinearPath(Vector3 path) : IMotion<Vector3>
{
    public Vector3 GetValue(float progress) => path * progress;
}

readonly struct CircularPath<A>(float radius) : IMotion<Vector3>
    where A : IAxis
{
    public Vector3 GetValue(float progress)
    {
        var theta = float.Tau * progress;
        return A.Apply(new(0F, float.Cos(theta), float.Sin(theta))) * radius;
    }
}
