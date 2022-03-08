using System.Runtime.InteropServices;

namespace ConsoleRayTracer;

static class Screen
{
    public const int WIDTH = 120;
    public const int HEIGHT = 100;

    private static readonly IntPtr _handle = Win32.GetStdHandle(-11);
    private static readonly CHAR_INFO[] _buf = new CHAR_INFO[WIDTH * HEIGHT];
    private static SMALL_RECT _rect = new(0, 0, WIDTH, HEIGHT);

    static Screen()
    {
        unsafe
        {
#pragma warning disable CA1416
            Console.SetWindowSize(WIDTH, HEIGHT);
            Console.SetBufferSize(WIDTH, HEIGHT);
#pragma warning restore CA1416

            var fontInfo = new CONSOLE_FONT_INFO_EX();
            fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
            Win32.GetCurrentConsoleFontEx(_handle, false, out fontInfo);
            fontInfo.dwFontSize = new(8, 8);
            Marshal.Copy("Terminal".ToCharArray(), 0, new(fontInfo.FaceName), 8);
            Win32.SetCurrentConsoleFontEx(_handle, false, fontInfo);
        }
    }

    public static void Set(int x, int y, float color) =>
        _buf[(HEIGHT - y - 1) * WIDTH + x] = new(color);

    public static void Draw()
    {
        Win32.WriteConsoleOutput(
            _handle,
            _buf,
            new(WIDTH, HEIGHT),
            new(0, 0),
            ref _rect
        );
    }
}

static class Win32
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetCurrentConsoleFontEx(
        IntPtr ConsoleOutput,
        bool MaximumWindow,
        out CONSOLE_FONT_INFO_EX ConsoleCurrentFont
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetCurrentConsoleFontEx(
        IntPtr ConsoleOutput,
        bool MaximumWindow,
        CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteConsoleOutput(
        IntPtr hConsoleOutput,
        CHAR_INFO[] lpBuffer,
        COORD dwBufferSize,
        COORD dwBufferCoord,
        ref SMALL_RECT lpWriteRegion
    );
}

[StructLayout(LayoutKind.Sequential)]
struct COORD
{
    public short X;
    public short Y;

    public COORD(short x, short y)
    {
        X = x;
        Y = y;
    }
}

[StructLayout(LayoutKind.Sequential)]
struct SMALL_RECT
{
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;

    public SMALL_RECT(short left, short top, short right, short bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}

[StructLayout(LayoutKind.Explicit)]
struct CHAR_INFO
{
    [FieldOffset(0)]
    public char UnicodeChar;
    [FieldOffset(0)]
    public char AsciiChar;
    [FieldOffset(2)]
    public ushort Attributes;

    private static readonly char[] _ascii = " .:+%#@".ToCharArray();

    public CHAR_INFO(float color)
    {
        UnicodeChar = '\0';
        AsciiChar = _ascii[(int)Math.Round(color * (_ascii.Length - 1))];
        Attributes = 7;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
unsafe struct CONSOLE_FONT_INFO_EX
{
    public uint cbSize;
    public uint nFont;
    public COORD dwFontSize;
    public ushort FontFamily;
    public ushort FontWeight;
    public fixed char FaceName[LF_FACESIZE];

    const int LF_FACESIZE = 32;
}
