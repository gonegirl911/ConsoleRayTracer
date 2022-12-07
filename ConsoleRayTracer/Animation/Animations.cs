using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ConsoleRayTracer;

public interface IAnimation<out T>
{
    float Duration { get; }

    T GetValue(float timeElapsed) => GetValueUnchecked(
        (timeElapsed, timeElapsed % Duration) switch
        {
            ( > 0f, 0f) => Duration,
            ( <= 0f, 0f) => 0f,
            ( >= 0f, var elapsed) => elapsed,
            ( < 0f, var elapsed) => Duration + elapsed,
            _ => throw new ArgumentException("invalid timeElapsed"),
        }
    );

    T GetValueUnchecked(float timeElapsed);
}

public readonly record struct Animation<T, M, I>(M Motion, I Interpolator, float Duration) : IAnimation<T>
    where M : IMotion<T>
    where I : IInterpolator
{
    public T GetValueUnchecked(float timeElapsed) =>
        Motion.GetValue(Interpolator.GetInterpolation(timeElapsed / Duration));
}

public readonly record struct Constant<T>(T Value, float Duration) : IAnimation<T>
{
    public T GetValue(float timeElapsed) => Value;
    public T GetValueUnchecked(float timeElapsed) => Value;
}

public readonly record struct MotionChain(IAnimation<float>[] Animations) : IAnimation<float>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public float GetValueUnchecked(float timeElapsed)
    {
        var start = 0f;
        var accum = 0f;
        ref var first = ref MemoryMarshal.GetReference(Animations.AsSpan());
        for (var i = 0; i < Animations.Length; i++)
        {
            var animation = Unsafe.Add(ref first, i);
            var dt = timeElapsed - start;
            if (dt > animation.Duration)
            {
                start += animation.Duration;
                accum += animation.GetValueUnchecked(animation.Duration);
            }
            else
            {
                return accum + animation.GetValueUnchecked(dt);
            }
        }
        throw new UnreachableException();
    }
}

public readonly record struct PathChain(IAnimation<Vector3>[] Animations) : IAnimation<Vector3>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public Vector3 GetValueUnchecked(float timeElapsed)
    {
        var start = 0f;
        Vector3 accum = new(0f);
        ref var first = ref MemoryMarshal.GetReference(Animations.AsSpan());
        for (var i = 0; i < Animations.Length; i++)
        {
            var animation = Unsafe.Add(ref first, i);
            var dt = timeElapsed - start;
            if (dt > animation.Duration)
            {
                start += animation.Duration;
                accum += animation.GetValueUnchecked(animation.Duration);
            }
            else
            {
                return accum + animation.GetValueUnchecked(dt);
            }
        }
        throw new UnreachableException();
    }
}
