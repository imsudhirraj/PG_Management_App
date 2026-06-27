using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class TenantKycPage : ContentPage
{
    public TenantKycPage(TenantKycViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}