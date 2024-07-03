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
    public float GetInterpolation(float input) => float.Pow(input, 2F * Factor);
}

readonly record struct DecelerateInterpolator(float Factor) : IInterpolator
{
    public float GetInterpolation(float input) => 1F - float.Pow(1F - input, 2F * Factor);
}

readonly record struct SunInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => input <= 0.9F ? input * 5F / 9F : (input - 0.8F) * 5F;
}
