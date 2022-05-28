using App;

if (OperatingSystem.IsWindows())
{
    MyApp app = new();
    app.Run();
}
throw new InvalidOperationException("app supported only on Windows");
