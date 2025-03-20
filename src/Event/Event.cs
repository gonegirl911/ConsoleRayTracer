using System.Runtime.InteropServices;
using Windows.Win32.System.Console;

namespace ConsoleRayTracer;

readonly struct Event
{
    readonly Variant _variant;
    readonly Data _data;

    public Event(KeyEvent keyEvent)
    {
        _variant = Variant.Key;
        _data = new(keyEvent);
    }

    public Event(ResizeEvent resizeEvent)
    {
        _variant = Variant.Resize;
        _data = new(resizeEvent);
    }

    public KeyEvent? KeyEvent => _variant == Variant.Key ? _data._keyEvent : null;
    public ResizeEvent? ResizeEvent => _variant == Variant.Resize ? _data._resizeEvent : null;

    enum Variant : byte
    {
        Key,
        Resize,
    }

    [StructLayout(LayoutKind.Explicit)]
    readonly struct Data
    {
        [FieldOffset(0)]
        public readonly KeyEvent _keyEvent;
        [FieldOffset(0)]
        public readonly ResizeEvent _resizeEvent;

        public Data(KeyEvent keyEvent)
        {
            _keyEvent = keyEvent;
        }

        public Data(ResizeEvent resizeEvent)
        {
            _resizeEvent = resizeEvent;
        }
    }
}

readonly record struct KeyEvent(ConsoleKey Key, KeyState State)
{
    public KeyEvent(in INPUT_RECORD record)
        : this((ConsoleKey)record.Event.KeyEvent.wVirtualKeyCode, (KeyState)(byte)record.Event.KeyEvent.bKeyDown)
    { }

    public ConsoleKey? PressedKey => State == KeyState.Pressed ? Key : null;
}

enum KeyState : byte
{
    Pressed = 1,
    Released = 0,
}

readonly record struct ResizeEvent(int Width, int Height)
{
    public ResizeEvent(in INPUT_RECORD record)
        : this(record.Event.WindowBufferSizeEvent.dwSize.X, record.Event.WindowBufferSizeEvent.dwSize.Y)
    { }

    public float AspectRatio => (float)Width / Height;
}
