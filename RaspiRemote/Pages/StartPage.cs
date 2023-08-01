namespace RaspiRemote.Pages
{
    internal class StartPage : NavigationPage
    {
        public StartPage() : base(new SelectDevicePage())
        {
            BarTextColor = Colors.White;

            if (App.Current.Resources.TryGetValue("Primary", out var color))
            {
                BarBackgroundColor = (Color)color;
            }
        }
    }
}
