namespace ConsoleRayTracer;

readonly record struct World(IEnumerable<IEntity> Entities) : IAnimatedEntity
{
    public HitRecord? Hit(in Ray ray, float tMin, float tMax)
    {
        var closest = tMax;
        HitRecord? hitRecord = null;
        foreach (var entity in Entities)
        {
            if (entity.Hit(ray, tMin, closest) is HitRecord record)
            {
                closest = record.T;
                hitRecord = record;
            }
        }
        return hitRecord;
    }

    public void Update(float timeElapsed)
    {
        foreach (var entity in Entities)
        {
            if (entity is IAnimatedEntity e)
            {
                e.Update(timeElapsed);
            }
        }
    }
}

readonly record struct Plane(Vector3 Normal) : IEntity
{
    public HitRecord? Hit(in Ray ray, float tMin, float tMax)
    {
        var denom = Vector3.Dot(ray.Direction, Normal);
        if (denom == 0f)
        {
            return null;
        }

        var t = Vector3.Dot(-ray.Origin, Normal) / denom;
        if (t < tMin || t > tMax)
        {
            return null;
        }

        var point = ray.PointAt(t);
        var (frontFace, normal) = ray.OppositeNormal(Normal);
        return new(t, point, normal, frontFace);
    }
}

readonly record struct Circle<A>(float Radius, A Axis) : IEntity where A : IAxis
{
    public HitRecord? Hit(in Ray ray, float tMin, float tMax) =>
        new Plane(Axis.Vector3).Hit(ray, tMin, tMax) is HitRecord record
            ? Math.Sqrt(Vector3.Dot(record.Point, record.Point)) <= Radius ? record : null
            : null;
}

readonly record struct Rect<A>(float Width, float Height, A Axis) : IEntity where A : IAxis
{
    private readonly float _width = Width / 2f;
    private readonly float _height = Height / 2f;

    public HitRecord? Hit(in Ray ray, float tMin, float tMax)
    {
        if (new Plane(Axis.Vector3).Hit(ray, tMin, tMax) is HitRecord record)
        {
            var point = record.Point;
            return point.Get(Axis.Main) < -_width
                || point.Get(Axis.Main) > _width
                || point.Get(Axis.Secondary) < -_height
                || point.Get(Axis.Secondary) > _height
                ? null
                : record;
        }
        return null;
    }
}

readonly record struct Sphere(float Radius) : IEntity
{
    public HitRecord? Hit(in Ray ray, float tMin, float tMax)
    {
        var a = Vector3.Dot(ray.Direction, ray.Direction);
        var b = Vector3.Dot(ray.Origin, ray.Direction);
        var c = Vector3.Dot(ray.Origin, ray.Origin) - Radius * Radius;

        var discriminant = b * b - a * c;
        if (discriminant < 0f)
        {
            return null;
        }

        var sqrtD = (float)Math.Sqrt(discriminant);
        var root = (-b - sqrtD) / a;
        if (root < tMin || root > tMax)
        {
            root = (-b + sqrtD) / a;
            if (root < tMin || root > tMax)
            {
                return null;
            }
        }

        var t = root;
        var point = ray.PointAt(t);
        var outwardNormal = point / Radius;
        var (frontFace, normal) = ray.OppositeNormal(outwardNormal);
        return new(t, point, normal, frontFace);
    }
}

readonly record struct Cylinder(float Radius, float Height) : IEntity
{
    public HitRecord? Hit(in Ray ray, float tMin, float tMax)
    {
        var hit1 = HitSide(ray, tMin, tMax);
        var hit2 = new Apply<Circle<AxisY>>(new(Radius, new()), new(0f, Height, 0f)).Hit(ray, tMin, tMax);
        var hit3 = new Circle<AxisY>(Radius, new()).Hit(ray, tMin, tMax);

        var t1 = hit1?.T ?? float.MaxValue;
        var t2 = hit2?.T ?? float.MaxValue;
        var t3 = hit3?.T ?? float.MaxValue;

        if (t1 < t2 && t1 < t3)
            return hit1;
        else if (t2 < t3)
            return hit2;
        else
            return hit3;
    }

    public HitRecord? HitSide(in Ray ray, float tMin, float tMax)
    {
        Ray side = new(ray.Origin with { Y = 0 }, ray.Direction with { Y = 0f });
        var a = Vector3.Dot(side.Direction, side.Direction);
        var b = Vector3.Dot(side.Origin, side.Direction);
        var c = Vector3.Dot(side.Origin, side.Origin) - Radius * Radius;

        var discriminant = b * b - a * c;
        if (discriminant < 0f)
        {
            return null;
        }

        var sqrtD = (float)Math.Sqrt(discriminant);
        var root = (-b - sqrtD) / a;
        if (root < tMin || root > tMax)
        {
            root = (-b + sqrtD) / a;
            if (root < tMin || root > tMax)
            {
                return null;
            }
        }

        var t = root;
        var point = ray.PointAt(t);
        if (point.Y < 0f || point.Y > Height)
        {
            return null;
        }
        var outwardNormal = Vector3.Normalize(point with { Y = 0f });
        var (frontFace, normal) = ray.OppositeNormal(outwardNormal);
        return new(t, point, normal, frontFace);
    }
}

readonly record struct Cone(float Radius, float Height) : IEntity
{
    private readonly float _ratio = Radius / Height;

    public HitRecord? Hit(in Ray ray, float tMin, float tMax)
    {
        var hit1 = HitSide(ray, tMin, tMax);
        var hit2 = new Circle<AxisY>(Radius, new()).Hit(ray, tMin, tMax);
        var t1 = hit1?.T ?? float.MaxValue;
        var t2 = hit2?.T ?? float.MaxValue;
        return t1 < t2 ? hit1 : hit2;
    }

    public HitRecord? HitSide(in Ray ray, float tMin, float tMax)
    {
        var tan = _ratio * _ratio;
        var D = Height - ray.Origin.Y;

        Ray side = new(ray.Origin with { Y = 0f }, ray.Direction with { Y = 0f });
        var a = Vector3.Dot(side.Direction, side.Direction) - tan * ray.Direction.Y * ray.Direction.Y;
        var b = Vector3.Dot(side.Origin, side.Direction) + tan * D * ray.Direction.Y;
        var c = Vector3.Dot(side.Origin, side.Origin) - tan * D * D;

        var discriminant = b * b - a * c;
        if (discriminant < 0f)
        {
            return null;
        }

        var sqrtD = (float)Math.Sqrt(discriminant);
        var root = (-b - sqrtD) / a;
        if (root < tMin || root > tMax)
        {
            root = (-b + sqrtD) / a;
            if (root < tMin || root > tMax)
            {
                return null;
            }
        }

        var t = root;
        var point = ray.PointAt(t);
        if (point.Y < 0f || point.Y > Height)
        {
            return null;
        }
        var outwardNormal = Vector3.Normalize(point with { Y = (float)Math.Sqrt(point.X * point.X + point.Z * point.Z) * _ratio });
        var (frontFace, normal) = ray.OppositeNormal(outwardNormal);
        return new(t, point, normal, frontFace);
    }
}

readonly record struct RectPrism(float Width, float Height, float Depth) : IEntity
{
    private readonly World _rects = new(new IEntity[]
    {
        new Apply<Rect<AxisZ>>(new(Width, Height, new()), new(0f, Height / 2f, -Depth / 2f)),
        new Apply<Rect<AxisX>>(new(Depth, Height, new()), new(-Width / 2f, Height / 2f, 0f)),
        new Rect<AxisY>(Width, Depth, new()),
        new Apply<Rect<AxisZ>>(new(Width, Height, new()), new(0f, Height / 2f, Depth / 2f)),
        new Apply<Rect<AxisX>>(new(Depth, Height, new()), new(Width / 2f, Height / 2f, 0f)),
        new Apply<Rect<AxisY>>(new(Width, Depth, new()), new(0f, Height, 0f)),
    });

    public HitRecord? Hit(in Ray ray, float tMin, float tMax) =>
        _rects.Hit(ray, tMin, tMax);
}
