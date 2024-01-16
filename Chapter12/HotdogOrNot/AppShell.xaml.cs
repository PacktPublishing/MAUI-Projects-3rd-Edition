namespace HotdogOrNot;

public partial class AppShell : Shell
{
	public AppShell()
	{
        Routing.RegisterRoute("Result", typeof(HotdogOrNot.Views.ResultView));

        InitializeComponent();
	}
}
