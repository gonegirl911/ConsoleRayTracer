using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static ConsoleRayTracer.Win32;

namespace ConsoleRayTracer;

[SupportedOSPlatform("windows")]
class WindowsTerminal : ITerminal
{
    public short Width { get; }
    public short Height { get; }

    private static readonly char[] ASCII = " .:+%#@".ToCharArray();

    private readonly IntPtr _handle;
    private readonly CHAR_INFO[] _buf;
    private SMALL_RECT _rect;

    public WindowsTerminal(short width, short height)
    {
        Width = width;
        Height = height;

        _handle = GetStdHandle(-11);
        _buf = new CHAR_INFO[width * height];
        _rect = new(0, 0, width, height);

        Console.SetWindowSize(width, height);
        Console.SetBufferSize(width, height);

        var fontInfo = new CONSOLE_FONT_INFO_EX();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        GetCurrentConsoleFontEx(_handle, false, out fontInfo);
        fontInfo.dwFontSize = new(8, 8);
        unsafe
        {
            Marshal.Copy("Terminal".ToCharArray(), 0, new(fontInfo.FaceName), 8);
        }
        SetCurrentConsoleFontEx(_handle, false, fontInfo);
    }

    public void SetPixel(int x, int y, float color) =>
        _buf[(Height - y - 1) * Width + x] = new(ASCII[(int)(color * ASCII.Length - 1e-12)]);

    public void Draw() =>
        WriteConsoleOutput(
            _handle,
            _buf,
            new(Width, Height),
            new(0, 0),
            ref _rect
        );

    public ConsoleKey? KeyPressed()
    {
        if ((GetKeyState((VirtualKeyStates)0x57) & KEY_PRESSED) != 0)
            return ConsoleKey.W;
        else if ((GetKeyState((VirtualKeyStates)0x41) & KEY_PRESSED) != 0)
            return ConsoleKey.A;
        else if ((GetKeyState((VirtualKeyStates)0x53) & KEY_PRESSED) != 0)
            return ConsoleKey.S;
        else if ((GetKeyState((VirtualKeyStates)0x44) & KEY_PRESSED) != 0)
            return ConsoleKey.D;
        else if ((GetKeyState(VirtualKeyStates.VK_UP) & KEY_PRESSED) != 0)
            return ConsoleKey.UpArrow;
        else if ((GetKeyState(VirtualKeyStates.VK_LEFT) & KEY_PRESSED) != 0)
            return ConsoleKey.LeftArrow;
        else if ((GetKeyState(VirtualKeyStates.VK_DOWN) & KEY_PRESSED) != 0)
            return ConsoleKey.DownArrow;
        else if ((GetKeyState(VirtualKeyStates.VK_RIGHT) & KEY_PRESSED) != 0)
            return ConsoleKey.RightArrow;
        else if ((GetKeyState(VirtualKeyStates.VK_SPACE) & KEY_PRESSED) != 0)
            return ConsoleKey.Spacebar;
        else if ((GetKeyState((VirtualKeyStates)0x5A) & KEY_PRESSED) != 0)
            return ConsoleKey.Z;
        else if ((GetKeyState((VirtualKeyStates)0x50) & KEY_PRESSED) != 0)
            return ConsoleKey.P;
        else if ((GetKeyState((VirtualKeyStates)0x4B) & KEY_PRESSED) != 0)
            return ConsoleKey.K;
        else if ((GetKeyState((VirtualKeyStates)0x4C) & KEY_PRESSED) != 0)
            return ConsoleKey.L;
        else
            return null;
    }
}
