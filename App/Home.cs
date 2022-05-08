namespace App;

public partial class Home : Form
{
    public Home()
    {
        InitializeComponent();
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        await Task.Run(WorldsManager.Worlds["Default"].Start);
    }
}
