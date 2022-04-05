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
        Motion.GetValue(Interpolator.GetInterpolation((timeElapsed / Duration) % 1));
}

readonly record struct Chain<T>(IEnumerable<IAnimation<T>> Animations) : IAnimation<T>
{
    public float Duration { get => _duration; }
    private readonly float _duration = Animations.Sum(a => a.Duration);

    public T GetValue(float timeElapsed)
    {
        timeElapsed %= Duration;
        var start = 0f;
        dynamic? accum = null;
        foreach (var animation in Animations)
        {
            var dt = Math.Abs(timeElapsed) - start;
            if (dt > animation.Duration)
            {
                start += animation.Duration;
                accum = accum is null
                    ? animation.GetValue(animation.Duration - 1e-4f)
                    : accum + animation.GetValue(animation.Duration - 1e-4f);
            }
            else
            {
                return accum is null
                    ? animation.GetValue((float)Math.CopySign(dt, timeElapsed))!
                    : accum + animation.GetValue(dt)!;
            }
        }
        throw new InvalidOperationException();
    }
}
