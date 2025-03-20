namespace ConsoleRayTracer;

interface IInterpolator
{
    float GetInterpolation(float input);
}

readonly struct LinearInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => input;
}

readonly struct AccelerateInterpolator(float factor) : IInterpolator
{
    public float GetInterpolation(float input) => float.Pow(input, 2F * factor);
}

readonly struct DecelerateInterpolator(float factor) : IInterpolator
{
    public float GetInterpolation(float input) => 1F - float.Pow(1F - input, 2F * factor);
}

readonly struct SunInterpolator : IInterpolator
{
    public float GetInterpolation(float input) => input <= 0.9F ? input * 5F / 9F : (input - 0.8F) * 5F;
}
