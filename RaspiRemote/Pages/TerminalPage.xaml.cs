using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class TerminalPage : ContentPage
{
    private TerminalPageViewModel _vm;

	public TerminalPage()
	{
		InitializeComponent();
		
		_vm = ServiceHelper.GetService<TerminalPageViewModel>();
		BindingContext = _vm;

        consoleWebview.Navigated += (s, e) => ConfigureViewModel();
    }

    private void ConfigureViewModel()
    {
        if (_vm.IsConsoleInitialized) return;

        _vm.ConsoleDataReceived += WriteToConsole;
        _vm.ConsoleDimensions = (consoleWebview.Width, consoleWebview.Height);
        _vm.IsConsoleInitialized = true;
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

    private void SetFocusForEntry(object sender, EventArgs e)
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