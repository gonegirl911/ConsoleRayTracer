using RayTracer;

namespace App;

public partial class Home : Form
{
    private readonly WorldsManager manager = new("worlds.json");

    public Home() => InitializeComponent();

    private void buttonStart_Click(object sender, EventArgs e)
    {
        AppConfig<WindowsTerminal<RayTracer.RayTracer>, RayTracer.RayTracer> config = new(
            Width: int.Parse(textBoxWidth.Text),
            Height: int.Parse(textBoxHeight.Text)
        );
        new Thread(() => manager.Worlds[0].Start(config)).Start();
    }
}
