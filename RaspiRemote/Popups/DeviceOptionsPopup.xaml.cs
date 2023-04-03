using CommunityToolkit.Maui.Views;

namespace RaspiRemote.Popups;

public enum DeviceOptionsActions { Edit, Delete }

public partial class DeviceOptionsPopup : Popup
{
	public DeviceOptionsPopup()
	{
		InitializeComponent();
	}

    private void EditBtnClicked(object sender, EventArgs e)
    {
        Close(DeviceOptionsActions.Edit);
    }

    private void DeleteBtnClicked(object sender, EventArgs e)
    {
        Close(DeviceOptionsActions.Delete);
    }
}