namespace ConsoleRayTracer;

interface IAnimation<out T>
{
    float Duration { get; }

    T GetValue(float timeElapsed) => GetValueUnchecked(
        (timeElapsed, timeElapsed % Duration) switch
        {
            ( > 0f, 0f) => Duration,
            ( <= 0f, 0f) => 0f,
            ( >= 0f, var elapsed) => elapsed,
            ( < 0f, var elapsed) => Duration + elapsed,
            _ => throw new ArgumentException("Invalid timeElapsed"),
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

readonly record struct MotionChain(IEnumerable<IAnimation<float>> Animations) : IAnimation<float>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public float GetValueUnchecked(float timeElapsed)
    {
        var start = 0f;
        var accum = 0f;
        foreach (var animation in Animations)
        {
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
        throw new InvalidOperationException();
    }
}

readonly record struct PathChain(IEnumerable<IAnimation<Vector3>> Animations) : IAnimation<Vector3>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public Vector3 GetValueUnchecked(float timeElapsed)
    {
        var start = 0f;
        Vector3 accum = new(0f);
        foreach (var animation in Animations)
        {
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
        throw new InvalidOperationException();
    }
}
