using System.Numerics;

namespace ConsoleRayTracer;

readonly record struct Group(IEntity[] Entities) : IAnimatedEntity
{
    readonly IAnimatedEntity[] _animatedEntities = Entities.OfType<IAnimatedEntity>().ToArray();

    public HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
        HitRecord? hitRecord = null;
        foreach (var entity in Entities.AsSpan())
        {
            if (entity.Hit(ray, tMin, tMax) is HitRecord record)
            {
                hitRecord = record;
                tMax = record.T;
            }
        }
        return hitRecord;
    }

    public void Update(float timeElapsed)
    {
        foreach (var animatedEntity in _animatedEntities.AsSpan())
        {
            animatedEntity.Update(timeElapsed);
        }
    }
}

readonly record struct Lights(IEntity[] Sources) : IAnimatedEntity
{
    readonly IAnimatedEntity[] _animatedSources = Sources.OfType<IAnimatedEntity>().ToArray()!;

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity
    {
        var accum = 0F;
        foreach (var source in Sources.AsSpan())
        {
            accum += source.Illuminate(entity, record);
        }
        return accum;
    }

    public void Update(float timeElapsed)
    {
        foreach (var animatedSource in _animatedSources.AsSpan())
        {
            animatedSource.Update(timeElapsed);
        }
    }
}

readonly record struct LightSource : IEntity
{
    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity
    {
        Ray lightRay = new(record.Point, Vector3.Normalize(-record.Point));
        return entity.Hit(lightRay, 0.001F, float.MaxValue) is null
            ? float.Max(record.Brightness * Vector3.Dot(record.Normal, lightRay.Direction), 0F)
            : 0F;
    }
}

readonly record struct And<L, R>(L Left, R Right) : IEntity
    where L : IEntity
    where R : IEntity
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        Left.Hit(ray, tMin, tMax) is HitRecord left
            ? Right.Hit(ray, tMin, left.T) ?? left
            : Right.Hit(ray, tMin, tMax);

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Left.Illuminate(entity, record) + Right.Illuminate(entity, record);
}

readonly record struct Plane<A> : IEntity where A : IAxis
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
        var t = -A.GetAxis(ray.Origin) / A.GetAxis(ray.Direction);
        return t < tMin || t > tMax ? null : new(t, ray.PointAt(t), A.Unit);
    }
}

readonly record struct Circle<A>(float Radius) : IEntity where A : IAxis
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        new Plane<A>().Hit(ray, tMin, tMax) is HitRecord record
            ? float.Sqrt(Vector3.Dot(record.Point, record.Point)) > Radius ? null : record
            : null;
}

readonly record struct Rect<A>(float Width, float Height) : IEntity where A : IAxis
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        new Plane<A>().Hit(ray, tMin, tMax) is HitRecord record
            ? A.GetMain(record.Point) < 0
                || A.GetMain(record.Point) > Width
                || A.GetSecondary(record.Point) < 0
                || A.GetSecondary(record.Point) > Height
                ? null
                : record
            : null;
}

readonly record struct Sphere(float Radius) : IEntity
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
        var a = Vector3.Dot(ray.Direction, ray.Direction);
        var b = Vector3.Dot(ray.Origin, ray.Direction);
        var c = Vector3.Dot(ray.Origin, ray.Origin) - Radius * Radius;
        var sqrtD = float.Sqrt(b * b - a * c);
        if (float.IsNaN(sqrtD))
        {
            return null;
        }

        var t = (-b - sqrtD) / a;
        if (t < tMin || t > tMax)
        {
            t = (-b + sqrtD) / a;
            if (t < tMin || t > tMax)
            {
                return null;
            }
        }

        var point = ray.PointAt(t);
        return new(t, point, point / Radius);
    }
}

readonly record struct Cylinder(float Height, float Radius) : IEntity
{
    readonly And<And<Apply<Circle<AxisY>>, Circle<AxisY>>, Lateral> _components = new(
        new(
            new(new(Radius), new(0F, Height, 0F)),
            new(Radius)
        ),
        new(Height, Radius)
    );

    public HitRecord? Hit(Ray ray, float tMin, float tMax) => _components.Hit(ray, tMin, tMax);

    readonly record struct Lateral(float Height, float Radius) : IEntity
    {
        public HitRecord? Hit(Ray ray, float tMin, float tMax)
        {
            Ray sideRay = new(ray.Origin with { Y = 0 }, ray.Direction with { Y = 0F });
            var a = Vector3.Dot(sideRay.Direction, sideRay.Direction);
            var b = Vector3.Dot(sideRay.Origin, sideRay.Direction);
            var c = Vector3.Dot(sideRay.Origin, sideRay.Origin) - Radius * Radius;
            var sqrtD = float.Sqrt(b * b - a * c);
            if (float.IsNaN(sqrtD))
            {
                return null;
            }

            var t = (-b - sqrtD) / a;
            if (t < tMin || t > tMax)
            {
                t = (-b + sqrtD) / a;
                if (t < tMin || t > tMax)
                {
                    return null;
                }
            }

            var point = ray.PointAt(t);
            return point.Y < 0F || point.Y > Height
                ? null
                : new(t, point, Vector3.Normalize(point with { Y = 0F }));
        }
    }
}

readonly record struct Cone(float Height, float Radius) : IEntity
{
    readonly And<Circle<AxisY>, Lateral> _components = new(new(Radius), new(Height, Radius));

    public HitRecord? Hit(Ray ray, float tMin, float tMax) => _components.Hit(ray, tMin, tMax);

    readonly record struct Lateral(float Height, float Radius) : IEntity
    {
        readonly float _ratio = Radius / Height;

        public HitRecord? Hit(Ray ray, float tMin, float tMax)
        {
            Ray sideRay = new(ray.Origin with { Y = 0F }, ray.Direction with { Y = 0F });
            var tan = _ratio * _ratio;
            var D = Height - ray.Origin.Y;
            var a = Vector3.Dot(sideRay.Direction, sideRay.Direction) - tan * ray.Direction.Y * ray.Direction.Y;
            var b = Vector3.Dot(sideRay.Origin, sideRay.Direction) + tan * D * ray.Direction.Y;
            var c = Vector3.Dot(sideRay.Origin, sideRay.Origin) - tan * D * D;
            var sqrtD = float.Sqrt(b * b - a * c);
            if (float.IsNaN(sqrtD))
            {
                return null;
            }

            var t = (-b - sqrtD) / a;
            if (t < tMin || t > tMax)
            {
                t = (-b + sqrtD) / a;
                if (t < tMin || t > tMax)
                {
                    return null;
                }
            }

            var point = ray.PointAt(t);
            return point.Y < 0F || point.Y > Height
                ? null
                : new(t, point, Vector3.Normalize(point with { Y = float.Sqrt(point.X * point.X + point.Z * point.Z) * _ratio }));
        }
    }
}

readonly record struct Cuboid(float Width, float Height, float Depth) : IEntity
{
    readonly And<And<And<Rect<AxisZ>, Rect<AxisX>>, Rect<AxisY>>, And<And<Apply<Rect<AxisZ>>, Apply<Rect<AxisX>>>, Apply<Rect<AxisY>>>> _components = new(
        new(new(new(Width, Height), new(Depth, Height)), new(Width, Depth)),
        new(
            new(
                new(new(Width, Height), new(0F, 0F, Depth)),
                new(new(Depth, Height), new(Width, 0F, 0F))
            ),
            new(new(Width, Depth), new(0F, Height, 0F))
        )
    );

    public HitRecord? Hit(Ray ray, float tMin, float tMax) => _components.Hit(ray, tMin, tMax);
}
