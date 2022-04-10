namespace ConsoleRayTracer;

interface IInterpolator
{
    float GetInterpolation(float input);
}

readonly record struct LinearInterpolator() : IInterpolator
{
    public float GetInterpolation(float input) => input;
}

readonly record struct AccelerateInterpolator(float Factor) : IInterpolator
{
    public float GetInterpolation(float input) => (float)Math.Pow(input, 2f * Factor);
}

readonly record struct DecelerateInterpolator(float Factor) : IInterpolator
{
    public float GetInterpolation(float input) => 1f - (float)Math.Pow(1f - input, 2f * Factor);
}

readonly record struct AcelerateDecelerateInterpolator() : IInterpolator
{
    public float GetInterpolation(float input) => (float)Math.Cos((input + 1f) * Math.PI) * 0.5f + 0.5f;
}

readonly record struct AnticipateInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) => ((Tension + 1f) * input - Tension) * input * input;
}

readonly record struct OvershootInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) =>
        ((Tension + 1f) * (input - 1f) + Tension) * (input - 1f) * (input - 1f) + 1f;
}

readonly record struct AnticipateOvershootInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) => input < 0.5f
        ? 0.5f * (((Tension + 1f) * 2f * input - Tension) * 4f * input * input)
        : 0.5f * (((Tension + 1f) * (2f * input - 2f) + Tension) * (4f * input * input - 8f * input + 4f)) + 1f;
}

readonly record struct CycleInterpolator(float Cycles)
{
    public float GetIterpolation(float input) => (float)Math.Sin(2d * Math.PI * Cycles * input);
}

readonly record struct FunctionalInterpolator(Func<float, float> Func) : IInterpolator
{
    public float GetInterpolation(float input) => Func(input);
}
