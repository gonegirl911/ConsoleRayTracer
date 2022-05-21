using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static RayTracer.Win32;

namespace RayTracer;

[SupportedOSPlatform("windows")]
public class WindowsTerminal<R> : ITerminal<R> where R : IRenderer
{
    private readonly IntPtr _handle;
    private CHAR_INFO[] _buf;
    private SMALL_RECT _rect;

    public WindowsTerminal(int width, int height, R renderer, string title)
    {
        Width = width;
        Height = height;
        Renderer = renderer;

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
    public R Renderer { get; }

    public void Set(int x, int y, char ch) => _buf[y * Width + x] = new(ch);

    public void Update()
    {
        WriteConsoleOutput(
            _handle,
            _buf,
            new((short)Width, (short)Height),
            new(0, 0),
            ref _rect
        );

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
        else if ((GetKeyState((VirtualKeyStates)0x59) & KEY_PRESSED) != 0)
            return ConsoleKey.Y;
        else if ((GetKeyState((VirtualKeyStates)0x4E) & KEY_PRESSED) != 0)
            return ConsoleKey.N;
        else
            return null;
    }
}
