﻿using System.Runtime.InteropServices;

namespace ConsoleRayTracer;

readonly record struct Event(Variant Variant, Data Data)
{
    public readonly KeyEvent? KeyEvent = Variant == Variant.Key ? Data.KeyEvent : null;
    public readonly ResizeEvent? ResizeEvent = Variant == Variant.Resize ? Data.ResizeEvent : null;

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
    public readonly ConsoleKey? PressedKey = State == KeyState.Pressed ? Key : null;
    public readonly ConsoleKey? ReleasedKey = State == KeyState.Released ? Key : null;
}

enum KeyState : byte
{
    Pressed = 1,
    Released = 0,
}

readonly record struct ResizeEvent(int Width, int Height)
{
    public readonly float AspectRatio = (float)Width / Height;
}
