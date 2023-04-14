namespace ConsoleRayTracer;

interface IInterpolator
{
    float GetInterpolation(float input);
}

readonly record struct LinearInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => input;
}

readonly record struct AccelerateInterpolator(float Factor) : IInterpolator
{
    private readonly float _factor = 2f * Factor;

    public float GetInterpolation(float input) => float.Pow(input, _factor);
}

readonly record struct DecelerateInterpolator(float Factor) : IInterpolator
{
    private readonly float _factor = 2f * Factor;

    public float GetInterpolation(float input) => 1f - float.Pow(1f - input, _factor);
}

readonly record struct LightsInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => input < 0.9f ? input * 5f / 9f : input * 5f;
}
