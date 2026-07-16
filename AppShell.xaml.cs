using MauiHighFidelityDashboard.Views;

namespace MauiHighFidelityDashboard;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("detail", typeof(DetailPage));
	}
}
