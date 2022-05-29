namespace ConsoleRayTracer;

public sealed class Animator
{
    private readonly float _sensitivity;
    private float _speed;
    private bool _isRunning;
    private bool _acceptPause = true;
    private float _timeElapsed = 0f;

    public Animator(float sensitivity, float speed = 1f, bool isRunning = true)
    {
        _sensitivity = sensitivity / 1000f;
        _speed = speed;
        _isRunning = isRunning;
    }

    public void Progress(ConsoleKey? key, float dt)
    {
        (_speed, _isRunning, _acceptPause, var change) = (key, _isRunning, _acceptPause) switch
        {
            (ConsoleKey.P, _, true) => (_speed, !_isRunning, false, 0f),
            (ConsoleKey.P, _, false) => (_speed, _isRunning, false, 0f),
            (ConsoleKey.K, true, _) => (_speed - dt * _sensitivity, _isRunning, true, 0f),
            (ConsoleKey.L, true, _) => (_speed + dt * _sensitivity, _isRunning, true, 0f),
            (ConsoleKey.K, false, _) => (_speed, _isRunning, true, -_speed),
            (ConsoleKey.L, false, _) => (_speed, _isRunning, true, _speed),
            _ => (_speed, _isRunning, true, 0f),
        };
        _timeElapsed += dt * (_isRunning ? _speed : change);
    }

    public void MoveForward<E>(in E entity) where E : IAnimatedEntity => entity.Update(_timeElapsed);
}
