using CommunityToolkit.Maui.Views;
using RaspiRemote.Models;

namespace RaspiRemote.Popups;

public partial class DevicePopupBase : Popup
{
    protected RpiDevice _device;

    public DevicePopupBase(RpiDevice device, string title)
	{
		InitializeComponent();
        Size = new Size(DeviceDisplay.Current.MainDisplayInfo.Width, DeviceDisplay.Current.MainDisplayInfo.Height);

        _device = device;
        BindingContext = _device;

        titleLabel.Text = title;
    }

    protected void SaveBtnClicked(object sender, EventArgs e)
    {
        if (ValidateData())
        {
            Close(_device);
        }
    }

    protected void CancelBtnClicked(object sender, EventArgs e)
    {
        Close();
    }

    private bool ValidateData()
    {
        var message = string.Empty;

        if (nameEntry.Text is null || nameEntry.Text == string.Empty)
            message += "Device name is empty.\n";

        if (hostEntry.Text is null || hostEntry.Text == string.Empty)
            message += "Device host is empty.\n";

        if (portEntry.Text == string.Empty || int.TryParse(portEntry.Text, out var port) is false ||
            port < 0 || port > 65535)
            message += "Device port is invalid or empty.\n";

        if (usernameEntry.Text is null || usernameEntry.Text == string.Empty)
            message += "Device username is empty.\n";

        if (passwordEntry.Text is null || passwordEntry.Text == string.Empty)
            message += "Device password is empty.\n";

        if (message != string.Empty)
        {
            Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            return false;
        }
        else
        {
            return true;
        }
    }
}