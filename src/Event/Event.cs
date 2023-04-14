using System.Runtime.InteropServices;

namespace ConsoleRayTracer;

public readonly record struct Event(Variant Variant, Data Data)
{
    public readonly KeyEvent? KeyEvent = Variant is Variant.Key ? Data.KeyEvent : null;
    public readonly ResizeEvent? ResizeEvent = Variant is Variant.Resize ? Data.ResizeEvent : null;

    public Event(KeyEvent keyEvent) : this(Variant.Key, new(keyEvent)) { }
    public Event(ResizeEvent resizeEvent) : this(Variant.Resize, new(resizeEvent)) { }
}

public enum Variant : byte
{
    Key,
    Resize,
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct Data
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

public readonly record struct KeyEvent(ConsoleKey Key, KeyState State)
{
    public readonly ConsoleKey? PressedKey = State is KeyState.Pressed ? Key : null;
    public readonly ConsoleKey? ReleasedKey = State is KeyState.Released ? Key : null;
}

public enum KeyState : byte
{
    Pressed = 1,
    Released = 0,
}

public readonly record struct ResizeEvent(int Width, int Height)
{
    public readonly float AspectRatio = (float)Width / Height;
}
