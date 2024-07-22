using System.Diagnostics;
using System.Numerics;

namespace ConsoleRayTracer;

interface IAnimation<out T>
{
    float Duration { get; }

    T GetValue(float timeElapsed) => GetValueUnchecked(
        (timeElapsed, timeElapsed % Duration) switch
        {
            ( > 0F, 0F) => Duration,
            ( <= 0F, 0F) => 0F,
            ( >= 0F, var elapsed) => elapsed,
            ( < 0F, var elapsed) => elapsed + Duration,
            _ => throw new ArgumentException("invalid timeElapsed"),
        }
    );

    T GetValueUnchecked(float timeElapsed);
}

readonly record struct Animation<T, M, I>(M Motion, I Interpolator, float Duration) : IAnimation<T>
    where M : IMotion<T>
    where I : IInterpolator
{
    public T GetValueUnchecked(float timeElapsed) =>
        Motion.GetValue(Interpolator.GetInterpolation(timeElapsed / Duration));
}

readonly record struct Constant<T>(T Value, float Duration) : IAnimation<T>
{
    public T GetValue(float timeElapsed) => Value;
    public T GetValueUnchecked(float timeElapsed) => Value;
}

readonly record struct MotionChain(IAnimation<float>[] Animations) : IAnimation<float>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public float GetValueUnchecked(float timeElapsed)
    {
        var acc = 0F;
        var elapsed = 0F;
        foreach (var animation in Animations.AsSpan())
        {
            var dt = timeElapsed - elapsed;
            if (dt > animation.Duration)
            {
                acc += animation.GetValueUnchecked(animation.Duration);
                elapsed += animation.Duration;
            }
            else
            {
                return acc + animation.GetValueUnchecked(dt);
            }
        }
        throw new UnreachableException();
    }
}

readonly record struct PathChain(IAnimation<Vector3>[] Animations) : IAnimation<Vector3>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public Vector3 GetValueUnchecked(float timeElapsed)
    {
        var acc = Vector3.Zero;
        var elapsed = 0F;
        foreach (var animation in Animations.AsSpan())
        {
            var dt = timeElapsed - elapsed;
            if (dt > animation.Duration)
            {
                acc += animation.GetValueUnchecked(animation.Duration);
                elapsed += animation.Duration;
            }
            else
            {
                return acc + animation.GetValueUnchecked(dt);
            }
        }
        throw new UnreachableException();
    }
}
