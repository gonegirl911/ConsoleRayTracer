using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static ConsoleRayTracer.Win32;

namespace ConsoleRayTracer;

[SupportedOSPlatform("windows")]
public sealed class WindowsTerminal : ICanvas
{
    private readonly IntPtr _handle;
    private CHAR_INFO[] _buf;
    private SMALL_RECT _rect;

    public WindowsTerminal(int width, int height, string title)
    {
        Width = width;
        Height = height;

        AllocConsole();
        _handle = GetStdHandle(-11);
        _buf = new CHAR_INFO[width * height];
        _rect = new(0, 0, (short)width, (short)height);

        CONSOLE_FONT_INFO_EX fontInfo = new();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        fontInfo.dwFontSize = new(8, 8);
        fontInfo.FaceName = "Terminal";
        SetCurrentConsoleFontEx(_handle, false, fontInfo);

        Console.Title = title;
        Console.SetWindowSize(width, height);
        SetConsoleScreenBufferSize(_handle, new((short)width, (short)height));
    }

    public int Width { get; private set; }
    public int Height { get; private set; }

    public void Set(int x, int y, float color)
    {
        const string ASCII = " .:+%#@";
        Set(x, y, ASCII[(int)(Math.Clamp(color, 0f, 1f) * ASCII.Length - 1e-12)]);
    }

    public void Set(int x, int y, char ch)
    {
        _buf[y * Width + x] = new(ch);
    }

    public void Commit()
    {
        WriteConsoleOutput(
            _handle,
            _buf,
            new((short)Width, (short)Height),
            new(0, 0),
            ref _rect
        );
    }

    public void Refresh()
    {
        var width = Console.WindowWidth;
        var height = Console.WindowHeight;
        if (width != Width || height != Height)
        {
            Width = width;
            Height = height;
            _buf = new CHAR_INFO[width * height];
            _rect = new(0, 0, (short)width, (short)height);
            SetConsoleScreenBufferSize(_handle, new((short)width, (short)height));
        }
    }
}
