namespace ConsoleRayTracer;

readonly record struct Light(IEnumerable<IEntity> Sources) : IAnimatedEntity
{
    private readonly IEnumerable<IAnimatedEntity> _animatedSources =
        Sources.Select(e => e as IAnimatedEntity).Where(e => e is not null).ToArray()!;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity
    {
        var accum = 0f;
        foreach (var source in Sources)
        {
            accum += source.Illuminate(entity, record);
        }
        return accum;
    }

    public void Update(float timeElapsed)
    {
        foreach (var source in _animatedSources)
        {
            source.Update(timeElapsed);
        }
    }
}

readonly record struct LightSource : IEntity
{
    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity
    {
        Ray toLight = new(record.Point, Vector3.Normalize(-record.Point));
        return entity.Hit(toLight, 0.001f, float.PositiveInfinity) is null
            ? Math.Max(record.Brightness * Vector3.Dot(toLight.Direction, record.Normal), 0f)
            : 0f;
    }
}
