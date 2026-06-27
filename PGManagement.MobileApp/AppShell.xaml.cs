using PGManagement.MobileApp.Views;

namespace PGManagement.MobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("HomePage", typeof(Views.HomePage));
        Routing.RegisterRoute("RegisterPage", typeof(Views.RegisterPage));
        Routing.RegisterRoute("TenantListPage", typeof(Views.TenantListPage));
        Routing.RegisterRoute(nameof(PGTenantDetailPage), typeof(PGTenantDetailPage));
        Routing.RegisterRoute(nameof(OwnerKycPage), typeof(OwnerKycPage));
        Routing.RegisterRoute(nameof(TenantKycPage), typeof(TenantKycPage));
        Routing.RegisterRoute(nameof(OwnerPaymentSettingsPage), typeof(OwnerPaymentSettingsPage));
        Routing.RegisterRoute(nameof(TenantPaymentPage), typeof(TenantPaymentPage));
        Routing.RegisterRoute(nameof(OwnerRevenuePage), typeof(OwnerRevenuePage));

    }
}   