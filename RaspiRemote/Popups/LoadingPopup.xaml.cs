using CommunityToolkit.Maui.Views;

namespace RaspiRemote.Popups;

public partial class LoadingPopup : Popup
{
	public LoadingPopup()
	{
		InitializeComponent();
		Size = new Size(DeviceDisplay.Current.MainDisplayInfo.Width, DeviceDisplay.Current.MainDisplayInfo.Height);
	}
}