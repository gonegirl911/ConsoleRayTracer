using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ConsoleRayTracer;

[SupportedOSPlatform("windows")]
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

    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
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
    public struct SMALL_RECT
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
    public struct CHAR_INFO
    {
        [FieldOffset(0)]
        public char UnicodeChar;
        [FieldOffset(0)]
        public char AsciiChar;
        [FieldOffset(2)]
        public ushort Attributes;

        public CHAR_INFO(char c)
        {
            UnicodeChar = '\0';
            AsciiChar = c;
            Attributes = 7;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct CONSOLE_FONT_INFO_EX
    {
        public uint cbSize;
        public uint nFont;
        public COORD dwFontSize;
        public ushort FontFamily;
        public ushort FontWeight;
        public fixed char FaceName[32];
    }
}
