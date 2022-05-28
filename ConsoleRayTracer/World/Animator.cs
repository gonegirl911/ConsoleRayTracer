namespace ConsoleRayTracer;

[JsonObject(MemberSerialization.Fields)]
public class Animator
{
    private readonly float _sensitivity;
    private float _speed;
    private bool _isRunning;
    private bool _acceptPause = true;
    private float _timeElapsed = 0f;

    public Animator(float sensitivity, float speed = 1f, bool isRunning = true)
    {
        _sensitivity = sensitivity;
        _speed = speed;
        _isRunning = isRunning;
    }

    public void Update<E>(in E entity, ConsoleKey? key, float dt) where E : IAnimatedEntity
    {
        (_speed, _isRunning, _acceptPause, var change) = (key, _isRunning, _acceptPause) switch
        {
            (ConsoleKey.P, _, true) => (_speed, !_isRunning, false, 0f),
            (ConsoleKey.P, _, false) => (_speed, _isRunning, false, 0f),
            (ConsoleKey.K, true, _) => (_speed - _sensitivity, _isRunning, true, 0f),
            (ConsoleKey.L, true, _) => (_speed + _sensitivity, _isRunning, true, 0f),
            (ConsoleKey.K, false, _) => (_speed, _isRunning, true, -_speed),
            (ConsoleKey.L, false, _) => (_speed, _isRunning, true, _speed),
            _ => (_speed, _isRunning, true, 0f),
        };
        _timeElapsed += dt * (_isRunning ? _speed : change);

        entity.Update(_timeElapsed);
    }

    public void Update<C>(in C camera, ConsoleKey? key, float dt, float aspectRatio)
        where C : IControllableCamera
    {
        camera.Adjust(aspectRatio);
        camera.Move(key, dt);
        camera.Rotate(key, dt);
    }

    public void Update<E, C>(Scene<E, C> scene, ConsoleKey? key, float dt, float aspectRatio)
        where E : IAnimatedEntity
        where C : IControllableCamera
    {
        Update(scene.Entity, key, dt);
        Update(scene.Camera, key, dt, aspectRatio);
    }
}
