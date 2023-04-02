using CommunityToolkit.Maui.Views;
using RaspiRemote.Models;

namespace RaspiRemote.Popups;

public partial class DevicePopupBase : Popup
{
    protected RpiDevice _device;

    public DevicePopupBase(RpiDevice device, string title)
	{
		InitializeComponent();

        _device = device;
        BindingContext = _device;

        titleLabel.Text = title;
    }

    protected void SaveBtnClicked(object sender, EventArgs e)
    {
        Close(_device);
    }

    protected void CancelBtnClicked(object sender, EventArgs e)
    {
        Close();
    }
}