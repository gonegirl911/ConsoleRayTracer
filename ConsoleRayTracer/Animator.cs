namespace ConsoleRayTracer;

class Animator
{
    private readonly float _sensitivity;
    private readonly float _changeRate;

    private float _speed;
    private bool _isRunning;
    private bool _acceptPause = true;
    private float _timeElapsed = 0f;

    public Animator(float sensitivity, float speed = 1f, bool isRunning = true)
    {
        _sensitivity = sensitivity;
        _changeRate = sensitivity * 100;
        _speed = speed;
        _isRunning = isRunning;
    }

    public void Update<E, L>(ConsoleKey? key, in E entity, in L light, float dt)
        where E : IAnimatedEntity
        where L : IAnimatedEntity
    {
        (_isRunning, _acceptPause) = (key, _acceptPause) switch
        {
            (ConsoleKey.P, true) => (!_isRunning, false),
            (ConsoleKey.P, false) => (_isRunning, false),
            _ => (_isRunning, true),
        };
        (_speed, var change) = (key, _isRunning) switch
        {
            (ConsoleKey.K, true) => (_speed - _sensitivity, 0f),
            (ConsoleKey.L, true) => (_speed + _sensitivity, 0f),
            (ConsoleKey.K, false) => (_speed, -_changeRate),
            (ConsoleKey.L, false) => (_speed, _changeRate),
            _ => (_speed, 0f),
        };

        _timeElapsed += _isRunning ? dt * _speed : change;
        entity.Update(_timeElapsed);
        light.Update(_timeElapsed);
    }
}
