namespace ConsoleRayTracer;

public sealed class Animator : IEventHandler
{
    private enum Keys
    {
        P = 1 << 0,
        K = 1 << 1,
        L = 1 << 2,
    }

    private readonly float _sensitivity;
    private float _speed;
    private bool _isRunning;
    private float _timeElapsed;

    private bool _acceptPause;
    private Keys _keyHistory;
    private Keys _relevantKeys;

    public Animator(float sensitivity, float speed = 1f, bool isRunning = true)
    {
        _sensitivity = sensitivity / 1000f;
        _speed = speed;
        _isRunning = isRunning;
        _timeElapsed = 0f;

        _acceptPause = true;
        _keyHistory = 0;
        _relevantKeys = 0;
    }

    public void Handle(in Event? ev, float dt)
    {
        if (ev?.KeyEvent is KeyEvent keyEvent)
        {
            if (keyEvent.PressedKey is ConsoleKey.P && _acceptPause)
            {
                _isRunning = !_isRunning;
                _acceptPause = false;
            }
            else
            {
                _acceptPause = true;
            }
        }
        _timeElapsed += Convert.ToInt32(_isRunning) * _speed * dt;
    }

    public void MoveForward<E>(in E animatedEntity) where E : IAnimatedEntity
    {
        animatedEntity.Update(_timeElapsed);
    }
}
