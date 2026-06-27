namespace PGManagement.MobileApp.Views;

public partial class PGTenantDetailPage : ContentPage
{
    public PGTenantDetailPage(ViewModels.PGTenantDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}