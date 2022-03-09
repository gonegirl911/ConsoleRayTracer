namespace ConsoleRayTracer;

interface IRenderer
{
    float RenderPixel<H, C, L>(float s, float t, in H hittable, in C camera, in L light)
        where H : IHittable
        where C : ICamera
        where L : ILight;
}
