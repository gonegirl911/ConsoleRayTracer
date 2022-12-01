﻿namespace ConsoleRayTracer;

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

public readonly record struct CircularPath<A>(float Radius, A Axis) : IMotion<Vector3>
    where A : IAxis
{
    public Vector3 GetValue(float interpolation)
    {
        var (sin, cos) = float.SinCos(float.Tau * interpolation);
        return Axis.Apply(new(0f, cos, sin)) * Radius;
    }
}
