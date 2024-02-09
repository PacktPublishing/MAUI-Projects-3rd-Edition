namespace MeTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override void OnResume()
    {
        base.OnResume();

        MainPage = new AppShell();
    }
}
