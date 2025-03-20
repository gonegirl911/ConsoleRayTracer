using System.Collections.Immutable;
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

readonly struct Animation<T, M, I>(M motion, I interpolator, float duration) : IAnimation<T>
    where M : IMotion<T>
    where I : IInterpolator
{
    public float Duration { get; } = duration;

    public T GetValueUnchecked(float timeElapsed) =>
        motion.GetValue(interpolator.GetInterpolation(timeElapsed / Duration));
}

readonly struct Constant<T>(T value, float duration) : IAnimation<T>
{
    public float Duration { get; } = duration;

    public T GetValue(float timeElapsed) => value;
    public T GetValueUnchecked(float timeElapsed) => value;
}

readonly struct MotionChain(ImmutableArray<IAnimation<float>> animations) : IAnimation<float>
{
    public float Duration { get; } = animations.Sum(a => a.Duration);

    public float GetValueUnchecked(float timeElapsed)
    {
        var acc = 0F;
        var elapsed = 0F;
        foreach (var animation in animations)
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

readonly struct PathChain(ImmutableArray<IAnimation<Vector3>> animations) : IAnimation<Vector3>
{
    public float Duration { get; } = animations.Sum(a => a.Duration);

    public Vector3 GetValueUnchecked(float timeElapsed)
    {
        var acc = Vector3.Zero;
        var elapsed = 0F;
        foreach (var animation in animations)
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
