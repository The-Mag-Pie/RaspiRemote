using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class TerminalPage : ContentPage
{
	public TerminalPage()
	{
		InitializeComponent();

		var vm = ServiceHelper.GetService<TerminalPageViewModel>();
		BindingContext = vm;

		consoleWebview.Navigated += (s, e) =>
		{
			vm.ConsoleDataReceived += WriteToConsole;
		};
    }

    private void WriteToConsole(string content)
	{
		var base64content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content));
		consoleWebview.Eval($"writeToConsole(`{base64content}`);");
	}
}