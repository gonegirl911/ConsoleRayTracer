namespace ConsoleRayTracer;

readonly record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public Vector3 PointAt(float t) => Origin + Direction * t;

    public (bool, Vector3) OppositeNormal(Vector3 normal) =>
        Vector3.Dot(Direction, normal) < 0f
            ? (true, normal)
            : (false, -normal);
}
