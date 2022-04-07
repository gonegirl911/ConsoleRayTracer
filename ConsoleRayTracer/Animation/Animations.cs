namespace ConsoleRayTracer;

interface IAnimation<T>
{
    float Duration { get; }

    T GetValue(float timeElapsed);
}

readonly record struct Animation<T, M, I>(M Motion, I Interpolator, float Duration) : IAnimation<T>
    where M : IMotion<T>
    where I : IInterpolator
{
    public T GetValue(float timeElapsed) =>
        Motion.GetValue(Interpolator.GetInterpolation(timeElapsed / Duration % 1));
}

readonly record struct MotionChain(IEnumerable<IAnimation<float>> Animations) : IAnimation<float>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public float GetValue(float timeElapsed)
    {
        timeElapsed = (Duration + timeElapsed % Duration) % Duration;
        var start = 0f;
        var accum = 0f;
        foreach (var animation in Animations)
        {
            var dt = timeElapsed - start;
            if (dt > animation.Duration)
            {
                start += animation.Duration;
                accum += animation.GetValue(animation.Duration - 1e-4f);
            }
            else
            {
                return accum + animation.GetValue(dt);
            }
        }
        throw new InvalidOperationException();
    }
}

readonly record struct PathChain(IEnumerable<IAnimation<Vector3>> Animations) : IAnimation<Vector3>
{
    public float Duration { get; } = Animations.Sum(a => a.Duration);

    public Vector3 GetValue(float timeElapsed)
    {
        timeElapsed = (Duration + timeElapsed % Duration) % Duration;
        var start = 0f;
        Vector3 accum = new(0f);
        foreach (var animation in Animations)
        {
            var dt = timeElapsed - start;
            if (dt > animation.Duration)
            {
                start += animation.Duration;
                accum += animation.GetValue(animation.Duration - 1e-4f);
            }
            else
            {
                return accum + animation.GetValue(dt);
            }
        }
        throw new InvalidOperationException();
    }
}
