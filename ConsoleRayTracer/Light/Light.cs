namespace ConsoleRayTracer;

readonly record struct Light(IEnumerable<LightSource> Sources) : ILight
{
    public float Illuminate<H>(in H hittable, in HitRecord record) where H : IHittable
    {
        var light = 0f;
        foreach (var (source, intensity) in Sources)
        {
            var toLight = new Ray(record.Point, Vector3.Normalize(source - record.Point));
            var diffused = 0f;
            if (hittable.Hit(toLight, 0.001f, float.MaxValue) is null)
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

readonly record struct LightSource(Vector3 Source, float Intensity);
