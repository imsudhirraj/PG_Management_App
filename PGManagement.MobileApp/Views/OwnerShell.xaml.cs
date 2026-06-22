namespace PGManagement.MobileApp.Views;

public partial class OwnerShell : Shell
{
    public OwnerShell()
{
    InitializeComponent();
    Routing.RegisterRoute("AddPGPage", typeof(Views.AddPGPage));
    Routing.RegisterRoute("PGDetailPage", typeof(Views.PGDetailPage));
    Routing.RegisterRoute("TenantDetailPage", typeof(Views.TenantDetailPage));
    Routing.RegisterRoute("AllocationsListPage", typeof(Views.AllocationsListPage));
    Routing.RegisterRoute("OwnerPaymentsPage", typeof(Views.OwnerPaymentsPage));
    Routing.RegisterRoute("OwnerComplaintsPage", typeof(Views.OwnerComplaintsPage));
    Routing.RegisterRoute("OwnerBookingsPage", typeof(Views.OwnerBookingsPage));
    Routing.RegisterRoute("OwnerNoticesPage", typeof(Views.OwnerNoticesPage));    
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