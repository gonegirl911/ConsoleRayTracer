namespace ConsoleRayTracer;

interface IInterpolator
{
    float GetInterpolation(float inptut);
}

readonly record struct LinearInterpolator() : IInterpolator
{
    public float GetInterpolation(float input) => input;
}

readonly record struct AccelerateInterpolator(float Factor) : IInterpolator
{
    public float GetInterpolation(float input) => (float)Math.Pow(input, 2 * Factor);
}

readonly record struct DecelerateInterpolator(float Factor) : IInterpolator
{
    public float GetInterpolation(float input) => 1 - (float)Math.Pow(1 - input, 2 * Factor);
}

readonly record struct AcelerateDecelerateInterpolator() : IInterpolator
{
    public float GetInterpolation(float input) => (float)Math.Cos((input + 1) * Math.PI) / 2 + 0.5f;
}

readonly record struct AnticipateInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) => ((Tension + 1) * input - Tension) * input * input;
}

readonly record struct OvershootInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) =>
        ((Tension + 1) * (input - 1) + Tension) * (input - 1) * (input - 1) + 1;
}

readonly record struct AnticipateOvershootInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) => input < 0.5f
        ? 0.5f * (((Tension + 1) * 2 * input - Tension) * 4 * input * input)
        : 0.5f * (((Tension + 1) * (2 * input - 2) + Tension) * (4 * input * input - 8 * input + 4)) + 1;
}

readonly record struct CycleInterpolator(float Cycles)
{
    public float GetIterpolation(float input) => (float)Math.Sin(2 * Math.PI * Cycles * input);
}

readonly record struct FunctionalInterpolator(Func<float, float> Func) : IInterpolator
{
    public float GetInterpolation(float input) => Func(input);
}
