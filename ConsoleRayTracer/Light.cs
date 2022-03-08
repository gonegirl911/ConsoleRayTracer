namespace ConsoleRayTracer;

readonly record struct LightSource(Vector3 Source, float Intensity);

readonly struct Light
{
    public readonly IEnumerable<LightSource> Sources { get; }

    public Light(IEnumerable<LightSource> sources) => Sources = sources;

    public float Enlighten(in World world, in HitRecord record)
    {
        var light = 0f;
        foreach (var (source, intensity) in Sources)
        {
            var toLight = new Ray(record.Point, Vector3.Normalize(source - record.Point));
            var diffused = 0f;
            if (world.Hit(toLight, 0.001f, float.MaxValue) is null)
            {
                diffused += intensity * record.Brightness * Vector3.Dot(toLight.Direction, record.Normal);
            }
            if (diffused >= 0f)
            {
                light += diffused;
            }
        }
        return light;
    }
}
