namespace PGManagement.MobileApp.Views;

public partial class TenantShell : Shell
{
    public TenantShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("PGTenantDetailPage", typeof(Views.PGTenantDetailPage));

    }
    protected override bool OnBackButtonPressed()
    {
        if (CurrentPage == this.CurrentItem?.CurrentItem?.Navigation.NavigationStack.FirstOrDefault())
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool confirm = await DisplayAlert("Exit", "Are you sure you want to exit the app?", "Yes", "No");
                if (confirm)
                {
                    // Let the OS handle the actual close — don't force-kill cross-platform
                    base.OnBackButtonPressed();
                }
            });
            return true; // block the immediate default exit while we wait for the dialog
        }
        return base.OnBackButtonPressed();
    }
}