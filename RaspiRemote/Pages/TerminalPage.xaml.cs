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
		// there was some problems with special characters when passing non-encoded content to javascript function
		var base64content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content));
		// Without invoking BeginInvokeOnMainThread() the Eval() method does not evaluate the script on Android
		MainThread.BeginInvokeOnMainThread(() =>
		{
            consoleWebview.Eval($"WriteToConsole(`{base64content}`);");
        });
	}

    private void ExecuteButtonClicked(object sender, EventArgs e)
    {
		commandEntry.Focus();
    }

    private void CommandEntryCompleted(object sender, EventArgs e)
    {
#if ANDROID
        // Soft keyboard is not showing up when Focus() is called. Workaround:
        commandEntry.IsEnabled = false;
        commandEntry.IsEnabled = true;
#endif
        commandEntry.Focus();
    }
}