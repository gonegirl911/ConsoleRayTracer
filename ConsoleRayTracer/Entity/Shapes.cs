namespace ConsoleRayTracer;

readonly record struct World(IEnumerable<IEntity> Entities) : IAnimatedEntity
{
    public HitRecord? Hit(in Ray ray)
    {
        var closest = float.MaxValue;
        HitRecord? hit = null;
        foreach (var entity in Entities)
        {
            if (entity.Hit(ray) is HitRecord record && record.T < closest)
            {
                closest = record.T;
                hit = record;
            }
        }
        return hit;
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

readonly record struct And<L, R>(L Left, R Right) : IEntity
    where L : IEntity
    where R : IEntity
{
    public HitRecord? Hit(in Ray ray)
    {
        var left = Left.Hit(ray);
        var right = Right.Hit(ray);
        return (right?.T ?? float.MaxValue) < (left?.T ?? float.MaxValue) ? right : left;
    }

    public float Illuminate<I>(in I entity, in HitRecord record) where I : IEntity =>
        Left.Illuminate(entity, record) + Right.Illuminate(entity, record);
}

readonly record struct Plane<A>(A Axis) : IEntity where A : IAxis
{
    public HitRecord? Hit(in Ray ray)
    {
        var t = -ray.Origin.Get(Axis.Axis) / ray.Direction.Get(Axis.Axis);
        return t > 0.001 ? new(t, ray.PointAt(t), Axis.Unit) : null;
    }
}

readonly record struct Circle<A>(float Radius, A Axis) : IEntity where A : IAxis
{
    public HitRecord? Hit(in Ray ray) =>
        new Plane<A>(Axis).Hit(ray) is HitRecord record
            ? Math.Sqrt(Vector3.Dot(record.Point, record.Point)) <= Radius ? record : null
            : null;
}

readonly record struct Rect<A>(float Width, float Height, A Axis) : IEntity where A : IAxis
{
    private readonly float _width = Width / 2f;
    private readonly float _height = Height / 2f;

    public HitRecord? Hit(in Ray ray) =>
        new Plane<A>(Axis).Hit(ray) is HitRecord record
            ? record.Point.Get(Axis.Main) < -_width
                || record.Point.Get(Axis.Main) > _width
                || record.Point.Get(Axis.Secondary) < -_height
                || record.Point.Get(Axis.Secondary) > _height
                ? null
                : record
            : null;
}

readonly record struct Sphere(float Radius) : IEntity
{
    public HitRecord? Hit(in Ray ray)
    {
        var a = Vector3.Dot(ray.Direction, ray.Direction);
        var b = Vector3.Dot(ray.Origin, ray.Direction);
        var c = Vector3.Dot(ray.Origin, ray.Origin) - Radius * Radius;
        var sqrtD = (float)Math.Sqrt(b * b - a * c);
        if (float.IsNaN(sqrtD))
        {
            return null;
        }
        
        var t = (-b - sqrtD) / a;
        if (t < 0.001)
        {
            t = (-b + sqrtD) / a;
            if (t < 0.001)
            {
                return null;
            }
        }
        var point = ray.PointAt(t);
        var normal = point / Radius;
        return new(t, point, normal);
    }
}

readonly record struct Cylinder(float Radius, float Height) : IEntity
{
    private readonly And<And<Apply<Circle<AxisY>>, Circle<AxisY>>, Lateral> _components = new(
        new(
            new(new(Radius, new()), new(0f, Height, 0f)),
            new(Radius, new())
        ),
        new(Radius, Height)
    );

    public HitRecord? Hit(in Ray ray) => _components.Hit(ray);

    readonly record struct Lateral(float Radius, float Height) : IEntity
    {
        public HitRecord? Hit(in Ray ray)
        {
            Ray side = new(ray.Origin with { Y = 0 }, ray.Direction with { Y = 0f });
            var a = Vector3.Dot(side.Direction, side.Direction);
            var b = Vector3.Dot(side.Origin, side.Direction);
            var c = Vector3.Dot(side.Origin, side.Origin) - Radius * Radius;
            var sqrtD = (float)Math.Sqrt(b * b - a * c);
            if (float.IsNaN(sqrtD))
            {
                return null;
            }

            var t = (-b - sqrtD) / a;
            if (t < 0.001)
            {
                t = (-b + sqrtD) / a;
                if (t < 0.001)
                {
                    return null;
                }
            }
            var point = ray.PointAt(t);
            if (point.Y < 0f || point.Y > Height)
            {
                return null;
            }
            var normal = Vector3.Normalize(point with { Y = 0f });
            return new(t, point, normal);
        }
    }
}

readonly record struct Cone(float Radius, float Height) : IEntity
{
    private readonly And<Circle<AxisY>, Lateral> _components = new(
        new(Radius, new()),
        new(Radius, Height)
    );

    public HitRecord? Hit(in Ray ray) => _components.Hit(ray);

    readonly record struct Lateral(float Radius, float Height) : IEntity
    {
        private readonly float _ratio = Radius / Height;

        public HitRecord? Hit(in Ray ray)
        {
            Ray side = new(ray.Origin with { Y = 0f }, ray.Direction with { Y = 0f });
            var tan = _ratio * _ratio;
            var D = Height - ray.Origin.Y;
            var a = Vector3.Dot(side.Direction, side.Direction) - tan * ray.Direction.Y * ray.Direction.Y;
            var b = Vector3.Dot(side.Origin, side.Direction) + tan * D * ray.Direction.Y;
            var c = Vector3.Dot(side.Origin, side.Origin) - tan * D * D;
            var sqrtD = (float)Math.Sqrt(b * b - a * c);
            if (float.IsNaN(sqrtD))
            {
                return null;
            }

            var t = (-b - sqrtD) / a;
            if (t < 0.001)
            {
                t = (-b + sqrtD) / a;
                if (t < 0.001)
                {
                    return null;
                }
            }
            var point = ray.PointAt(t);
            if (point.Y < 0f || point.Y > Height)
            {
                return null;
            }
            var normal = Vector3.Normalize(point with { Y = (float)Math.Sqrt(point.X * point.X + point.Z * point.Z) * _ratio });
            return new(t, point, normal);
        }
    }
}

readonly record struct RectPrism(float Width, float Height, float Depth) : IEntity
{
    private readonly And<And<And<Apply<Rect<AxisZ>>, Apply<Rect<AxisX>>>, Rect<AxisY>>, And<And<Apply<Rect<AxisZ>>, Apply<Rect<AxisX>>>, Apply<Rect<AxisY>>>> _components = new(
        new(
            new(
                new(new(Width, Height, new()), new(0f, Height / 2f, -Depth / 2f)),
                new(new(Depth, Height, new()), new(-Width / 2f, Height / 2f, 0f))
            ),
            new(Width, Depth, new())
        ),
        new(
            new(
                new(new(Width, Height, new()), new(0f, Height / 2f, Depth / 2f)),
                new(new(Depth, Height, new()), new(Width / 2f, Height / 2f, 0f))
            ),
            new(new(Width, Depth, new()), new(0f, Height, 0f))
        )
    );

    public HitRecord? Hit(in Ray ray) => _components.Hit(ray);
}
