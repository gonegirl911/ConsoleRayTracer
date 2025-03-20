using System.Runtime.InteropServices;
using Windows.Win32.System.Console;

namespace ConsoleRayTracer;

readonly record struct Event(Variant Variant, Data Data)
{
    public KeyEvent? KeyEvent => Variant == Variant.Key ? Data.KeyEvent : null;
    public ResizeEvent? ResizeEvent => Variant == Variant.Resize ? Data.ResizeEvent : null;

    public Event(KeyEvent keyEvent) : this(Variant.Key, new(keyEvent)) { }
    public Event(ResizeEvent resizeEvent) : this(Variant.Resize, new(resizeEvent)) { }
}

enum Variant : byte
{
    Key,
    Resize,
}

[StructLayout(LayoutKind.Explicit)]
readonly struct Data
{
    [FieldOffset(0)]
    public readonly KeyEvent KeyEvent;
    [FieldOffset(0)]
    public readonly ResizeEvent ResizeEvent;

    public Data(KeyEvent keyEvent)
    {
        KeyEvent = keyEvent;
    }

    public Data(ResizeEvent resizeEvent)
    {
        ResizeEvent = resizeEvent;
    }
}

readonly record struct KeyEvent(ConsoleKey Key, KeyState State)
{
    public ConsoleKey? PressedKey => State == KeyState.Pressed ? Key : null;

    public KeyEvent(in INPUT_RECORD record)
        : this((ConsoleKey)record.Event.KeyEvent.wVirtualKeyCode, (KeyState)(byte)record.Event.KeyEvent.bKeyDown)
    { }
}

enum KeyState : byte
{
    Pressed = 1,
    Released = 0,
}

readonly record struct ResizeEvent(int Width, int Height)
{
    public float AspectRatio => (float)Width / Height;

    public ResizeEvent(in INPUT_RECORD record)
        : this(record.Event.WindowBufferSizeEvent.dwSize.X, record.Event.WindowBufferSizeEvent.dwSize.Y)
    { }
}
