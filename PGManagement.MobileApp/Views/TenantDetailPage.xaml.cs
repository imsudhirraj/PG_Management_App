namespace PGManagement.MobileApp.Views;

public partial class TenantDetailPage : ContentPage
{
    public TenantDetailPage(ViewModels.TenantDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}