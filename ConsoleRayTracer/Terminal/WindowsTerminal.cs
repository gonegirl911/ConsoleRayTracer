using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static ConsoleRayTracer.Win32;

namespace ConsoleRayTracer;

[SupportedOSPlatform("windows")]
class WindowsTerminal : ITerminal
{
    private static readonly char[] ASCII = " .:+%#@".ToCharArray();

    public short Width { get; }
    public short Height { get; }

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
        _buf[(Height - y - 1) * Width + x] = new(ASCII[(int)Math.Round(color * (ASCII.Length - 1))]);

    public void Draw()
    {
        WriteConsoleOutput(
            _handle,
            _buf,
            new(Width, Height),
            new(0, 0),
            ref _rect
        );
    }
}
