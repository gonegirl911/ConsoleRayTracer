using System.Runtime.InteropServices;

namespace ConsoleRayTracer;

public readonly record struct Event(EventVariant Variant, EventData Data)
{
    public readonly KeyEvent? KeyEvent = Variant is EventVariant.Key ? Data.KeyEvent : null;
    public readonly ResizeEvent? ResizeEvent = Variant is EventVariant.Resize ? Data.ResizeEvent : null;
}

public enum EventVariant : byte { Key, Resize }

[StructLayout(LayoutKind.Explicit)]
public readonly struct EventData
{
    [FieldOffset(0)]
    public readonly KeyEvent KeyEvent;
    [FieldOffset(0)]
    public readonly ResizeEvent ResizeEvent;

    public EventData(KeyEvent keyEvent)
    {
        KeyEvent = keyEvent;
    }

    public EventData(ResizeEvent resizeEvent)
    {
        ResizeEvent = resizeEvent;
    }
}

public readonly record struct KeyEvent(ConsoleKey Key, KeyState State)
{
    public readonly ConsoleKey? PressedKey = State is KeyState.Pressed ? Key : null;
    public readonly ConsoleKey? ReleasedKey = State is KeyState.Released ? Key : null;
}

public enum KeyState : byte { Released = 0, Pressed = 1 }

public readonly record struct ResizeEvent(int Width, int Height)
{
    public readonly float AspectRatio = (float)Width / Height;
}
