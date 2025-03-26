using System.Collections.Immutable;
using System.Numerics;

namespace ConsoleRayTracer;

readonly struct Group(ImmutableArray<IEntity> entities) : IAnimatedEntity
{
    readonly ImmutableArray<IAnimatedEntity> _animatedEntities = [.. entities.OfType<IAnimatedEntity>()];

    public HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
        HitRecord? hitRecord = null;
        foreach (var entity in entities)
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
        foreach (var animatedEntity in _animatedEntities)
        {
            animatedEntity.Update(timeElapsed);
        }
    }
}

readonly struct Lights(ImmutableArray<IEntity> sources) : IAnimatedEntity
{
    readonly ImmutableArray<IAnimatedEntity> _animatedSources = [.. sources.OfType<IAnimatedEntity>()];

    public float Illuminate<I>(in I target, in HitRecord record) where I : IEntity
    {
        var acc = 0F;
        foreach (var source in sources)
        {
            acc += source.Illuminate(target, record);
        }
        return acc;
    }

    public void Update(float timeElapsed)
    {
        foreach (var animatedSource in _animatedSources)
        {
            animatedSource.Update(timeElapsed);
        }
    }
}

readonly struct LightSource : IEntity
{
    public float Illuminate<I>(in I target, in HitRecord record) where I : IEntity
    {
        Ray lightRay = new(record.Point, Vector3.Normalize(-record.Point));
        return target.Hit(lightRay, 0.001F, float.MaxValue) is null
            ? float.Max(record.Brightness * Vector3.Dot(record.Normal, lightRay.Direction), 0F)
            : 0F;
    }
}

readonly struct And<L, R>(L left, R right) : IEntity
    where L : IEntity
    where R : IEntity
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        left.Hit(ray, tMin, tMax) is HitRecord leftRecord
            ? right.Hit(ray, tMin, leftRecord.T) ?? leftRecord
            : right.Hit(ray, tMin, tMax);

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        left.Illuminate(entity, record) + right.Illuminate(entity, record);
}

readonly struct Plane<A> : IEntity where A : IAxis
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
        var t = -A.GetAxis(ray.Origin) / A.GetAxis(ray.Direction);
        return t < tMin || t > tMax ? null : new(t, ray.PointAt(t), A.Unit);
    }
}

readonly struct Circle<A>(float radius) : IEntity where A : IAxis
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        new Plane<A>().Hit(ray, tMin, tMax) is HitRecord record
            ? record.Point.Length() > radius ? null : record
            : null;
}

readonly struct Rect<A>(float width, float height) : IEntity where A : IAxis
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax) =>
        new Plane<A>().Hit(ray, tMin, tMax) is HitRecord record
            ? A.GetMain(record.Point) < 0F
                || A.GetMain(record.Point) > width
                || A.GetSecondary(record.Point) < 0F
                || A.GetSecondary(record.Point) > height
                ? null
                : record
            : null;
}

readonly struct Sphere(float radius) : IEntity
{
    public HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
        var a = Vector3.Dot(ray.Direction, ray.Direction);
        var b = Vector3.Dot(ray.Origin, ray.Direction);
        var c = Vector3.Dot(ray.Origin, ray.Origin) - radius * radius;
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
        return new(t, point, point / radius);
    }
}

readonly struct Cylinder(float height, float radius) : IEntity
{
    readonly And<And<Apply<Circle<AxisY>>, Circle<AxisY>>, Lateral> _components = new(
        new(
            new(new(radius), new(0F, height, 0F)),
            new(radius)
        ),
        new(height, radius)
    );

    public HitRecord? Hit(Ray ray, float tMin, float tMax) => _components.Hit(ray, tMin, tMax);

    readonly struct Lateral(float height, float radius) : IEntity
    {
        public HitRecord? Hit(Ray ray, float tMin, float tMax)
        {
            Ray sideRay = new(ray.Origin with { Y = 0F }, ray.Direction with { Y = 0F });
            var a = Vector3.Dot(sideRay.Direction, sideRay.Direction);
            var b = Vector3.Dot(sideRay.Origin, sideRay.Direction);
            var c = Vector3.Dot(sideRay.Origin, sideRay.Origin) - radius * radius;
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
            return point.Y < 0F || point.Y > height
                ? null
                : new(t, point, Vector3.Normalize(point with { Y = 0F }));
        }
    }
}

readonly struct Cone(float height, float radius) : IEntity
{
    readonly And<Circle<AxisY>, Lateral> _components = new(new(radius), new(height, radius));

    public HitRecord? Hit(Ray ray, float tMin, float tMax) => _components.Hit(ray, tMin, tMax);

    readonly struct Lateral(float height, float radius) : IEntity
    {
        readonly float _ratio = radius / height;

        public HitRecord? Hit(Ray ray, float tMin, float tMax)
        {
            Ray sideRay = new(ray.Origin with { Y = 0F }, ray.Direction with { Y = 0F });
            var tan = _ratio * _ratio;
            var D = height - ray.Origin.Y;
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
            return point.Y < 0F || point.Y > height
                ? null
                : new(t, point, Vector3.Normalize(point with { Y = float.Sqrt(point.X * point.X + point.Z * point.Z) * _ratio }));
        }
    }
}

readonly struct Cuboid(float width, float height, float depth) : IEntity
{
    readonly And<And<And<Rect<AxisZ>, Rect<AxisX>>, Rect<AxisY>>, And<And<Apply<Rect<AxisZ>>, Apply<Rect<AxisX>>>, Apply<Rect<AxisY>>>> _components = new(
        new(new(new(width, height), new(depth, height)), new(width, depth)),
        new(
            new(
                new(new(width, height), new(0F, 0F, depth)),
                new(new(depth, height), new(width, 0F, 0F))
            ),
            new(new(width, depth), new(0F, height, 0F))
        )
    );

    public HitRecord? Hit(Ray ray, float tMin, float tMax) => _components.Hit(ray, tMin, tMax);
}
