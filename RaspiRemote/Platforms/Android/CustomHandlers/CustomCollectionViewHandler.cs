using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Handlers.Items;

namespace RaspiRemote.Platforms.Android.CustomHandlers
{
    public class CustomCollectionViewHandler : CollectionViewHandler
    {
        private IItemsLayout ItemsLayout { get; set; }

        protected override IItemsLayout GetItemsLayout()
        {
            return ItemsLayout;
        }

        protected override void ConnectHandler(RecyclerView platformView)
        {
            base.ConnectHandler(platformView);

            ItemsLayout = VirtualView.ItemsLayout;
        }
    }
}
