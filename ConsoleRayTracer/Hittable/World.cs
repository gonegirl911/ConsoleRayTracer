namespace ConsoleRayTracer;

readonly struct World : IHittable
{
    public readonly IEnumerable<IHittable> Hittables { get; }

    public World(IEnumerable<IHittable> hittables) => Hittables = hittables;

    public HitRecord? Hit(in Ray ray, float tMin, float tMax)
    {
        var closest = tMax;
        HitRecord? hitRecord = null;
        foreach (var hittable in Hittables)
        {
            if (hittable.Hit(ray, tMin, closest) is HitRecord record)
            {
                closest = record.T;
                hitRecord = record;
            }
        }
        return hitRecord;
    }
}

readonly record struct Plane(Vector3 Normal, float Brightness, float Reflection) : IHittable
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
        return new(t, point, normal, frontFace, Brightness, Reflection);
    }
}

readonly record struct Sphere(float Radius, float Brightness, float Reflection) : IHittable
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
        return new(t, point, normal, frontFace, Brightness, Reflection);
    }
}
