using CommunityToolkit.Maui.Views;
using PGManagement.MobileApp.Views;

namespace PGManagement.MobileApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0; // Required for the counter click tracking

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckAndPromptLocationSettingsAsync();
        }

        private async Task CheckAndPromptLocationSettingsAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Lowest, TimeSpan.FromSeconds(1));
                    await Geolocation.Default.GetLocationAsync(request);
                }
            }
            catch (FeatureNotEnabledException)
            {
                var popup = new LocationPromptPopup();
                var result = await this.ShowPopupAsync(popup);

                if (result is bool baseResult && baseResult)
                {
                    AppInfo.Current.ShowSettingsUI();
                }
            }
            catch (Exception)
            {
                // Safety catch structure
            }
        }

        // FIXED: Re-added the missing event handler required by your MainPage.xaml
        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}