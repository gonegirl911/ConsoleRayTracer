﻿using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.Console;

namespace ConsoleRayTracer;

[SupportedOSPlatform("windows")]
sealed class WindowsTerminal : ICanvas<WindowsTerminal>
{
    private readonly SafeFileHandle _stdin;
    private readonly SafeFileHandle _stdout;
    private CHAR_INFO[] _buf;
    private SMALL_RECT _rect;

    public WindowsTerminal(short width, short height, string title)
    {
        _stdin = new(PInvoke.GetStdHandle(STD_HANDLE.STD_INPUT_HANDLE), false);
        _stdout = new(PInvoke.GetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE), false);

        PInvoke.SetConsoleTitle(title);
        PInvoke.SetConsoleMode(_stdin, CONSOLE_MODE.ENABLE_WINDOW_INPUT);
        PInvoke.SetCurrentConsoleFontEx(_stdout, false, new() { cbSize = (uint)Marshal.SizeOf(new CONSOLE_FONT_INFOEX()), dwFontSize = new() { X = 8, Y = 8 }, FaceName = "Terminal" });
        PInvoke.SetConsoleCursorInfo(_stdout, new() { dwSize = 100, bVisible = false });

        var sizeLimits = PInvoke.GetLargestConsoleWindowSize(_stdout);

        width = width == 0 || width > sizeLimits.X ? sizeLimits.X : width;
        height = height == 0 || height > sizeLimits.Y ? sizeLimits.Y : height;

        _buf = new CHAR_INFO[width * height];
        _rect = new() { Left = 0, Top = 0, Right = (short)(width - 1), Bottom = (short)(height - 1) };

        PInvoke.SetConsoleWindowInfo(_stdout, true, new() { Left = 0, Top = 0, Right = 1, Bottom = 1 });
        PInvoke.SetConsoleScreenBufferSize(_stdout, new() { X = width, Y = height });
        PInvoke.SetConsoleWindowInfo(_stdout, true, _rect);
        PInvoke.GetNumberOfConsoleInputEvents(_stdin, out var eventsAvailable);
        PInvoke.ReadConsoleInput(_stdin, stackalloc INPUT_RECORD[(int)eventsAvailable - 1], out var _);
    }

    public int Width => _rect.Right + 1;
    public int Height => _rect.Bottom + 1;

    public Event? Refresh()
    {
        RefreshBufferSize();

        PInvoke.GetNumberOfConsoleInputEvents(_stdin, out var eventsAvailable);

        if (eventsAvailable != 0)
        {
            Span<INPUT_RECORD> events = stackalloc INPUT_RECORD[1];

            PInvoke.ReadConsoleInput(_stdin, events, out var _);

            ref var ev = ref MemoryMarshal.GetReference(events);

            if (ev.EventType == 0x0001)
            {
                return new(
                    new KeyEvent(
                        (ConsoleKey)ev.Event.KeyEvent.wVirtualKeyCode,
                        (KeyState)(byte)ev.Event.KeyEvent.bKeyDown
                    )
                );
            }
            else if (ev.EventType == 0x0004)
            {
                var width = ev.Event.WindowBufferSizeEvent.dwSize.X;
                var height = ev.Event.WindowBufferSizeEvent.dwSize.Y;

                _buf = new CHAR_INFO[width * height];
                _rect = new() { Left = 0, Top = 0, Right = (short)(width - 1), Bottom = (short)(height - 1) };

                return new(new ResizeEvent(width, height));
            }
        }

        return null;
    }

    public void Set(int x, int y, float color)
    {
        Set(x, y, Character(color));
    }

    public void Set(int x, int y, char ch)
    {
        _buf[y * Width + x] = new() { Char = new() { UnicodeChar = ch }, Attributes = 7 };
    }

    public void Commit()
    {
        PInvoke.WriteConsoleOutput(
            _stdout,
            MemoryMarshal.GetReference(_buf.AsSpan()),
            new() { X = (short)Width, Y = (short)Height },
            new() { X = 0, Y = 0 },
            ref _rect
        );
    }

    private void RefreshBufferSize()
    {
        PInvoke.GetConsoleScreenBufferInfo(_stdout, out var bufferInfo);

        var width = bufferInfo.srWindow.Right + 1;
        var height = bufferInfo.srWindow.Bottom + 1;

        if (width != Width || height != Height)
        {
            PInvoke.SetConsoleScreenBufferSize(_stdout, new() { X = (short)width, Y = (short)height });
        }
    }

    private char Character(float color)
    {
        const string ASCII = " .:+%#@";

        return ASCII[(int)float.Round(float.Clamp(color, 0f, 1f) * (ASCII.Length - 1))];
    }
}