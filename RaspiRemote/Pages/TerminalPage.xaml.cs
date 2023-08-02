using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class TerminalPage : ContentPage
{
	public TerminalPage()
	{
		InitializeComponent();

		BindingContext = ServiceHelper.GetService<TerminalPageViewModel>();
	}
}