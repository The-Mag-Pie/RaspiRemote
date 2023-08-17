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
        if (_vm.IsWebViewLoaded) return;

        _vm.InitializeConsoleFunction = InitXterm;
        _vm.ReloadConsoleFunction = consoleWebview.Reload;
        _vm.ConsoleDimensions = (consoleWebview.Width, consoleWebview.Height);
        _vm.IsWebViewLoaded = true;
    }

    // Without invoking BeginInvokeOnMainThread() the Eval() method does not evaluate the script on Android
    private void InitXterm() => MainThread.BeginInvokeOnMainThread(() =>
    {
        consoleWebview.Eval($"Initialize();");
    });
}