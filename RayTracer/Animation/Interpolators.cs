namespace RayTracer;

public interface IInterpolator
{
    float GetInterpolation(float input);
}

public readonly record struct LinearInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => input;
}

public readonly record struct AccelerateInterpolator(float Factor) : IInterpolator
{
    public float GetInterpolation(float input) => (float)Math.Pow(input, 2f * Factor);
}

public readonly record struct DecelerateInterpolator(float Factor) : IInterpolator
{
    public float GetInterpolation(float input) => 1f - (float)Math.Pow(1f - input, 2f * Factor);
}

public readonly record struct AcelerateDecelerateInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => (float)Math.Cos((input + 1f) * Math.PI) * 0.5f + 0.5f;
}

public readonly record struct AnticipateInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) => ((Tension + 1f) * input - Tension) * input * input;
}

public readonly record struct OvershootInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) =>
        ((Tension + 1f) * (input - 1f) + Tension) * (input - 1f) * (input - 1f) + 1f;
}

public readonly record struct AnticipateOvershootInterpolator(float Tension) : IInterpolator
{
    public float GetInterpolation(float input) => input < 0.5f
        ? 0.5f * (((Tension + 1f) * 2f * input - Tension) * 4f * input * input)
        : 0.5f * (((Tension + 1f) * (2f * input - 2f) + Tension) * (4f * input * input - 8f * input + 4f)) + 1f;
}

public readonly record struct CycleInterpolator(float Cycles) : IInterpolator
{
    public float GetInterpolation(float input) => (float)Math.Sin(2d * Math.PI * Cycles * input);
}

public readonly record struct LightsInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => input < 0.9f ? input * 5f / 9f : input * 5f;
}
