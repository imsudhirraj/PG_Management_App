using PGManagement.MobileApp.Views;

namespace PGManagement.MobileApp;

public partial class AppShell : Shell
{
    // Added optional parameter to handle deep linking from login redirects
    public AppShell(string? initialRoute = null)
    {
        InitializeComponent();

        // 1. Explicit Route Registration Records
        Routing.RegisterRoute("HomePage", typeof(Views.HomePage));
        Routing.RegisterRoute("RegisterPage", typeof(Views.RegisterPage));
        Routing.RegisterRoute("TenantListPage", typeof(Views.TenantListPage));
        Routing.RegisterRoute(nameof(PGTenantDetailPage), typeof(PGTenantDetailPage));
        Routing.RegisterRoute(nameof(OwnerKycPage), typeof(OwnerKycPage));
        Routing.RegisterRoute(nameof(TenantKycPage), typeof(TenantKycPage));
        Routing.RegisterRoute(nameof(OwnerPaymentSettingsPage), typeof(OwnerPaymentSettingsPage));
        Routing.RegisterRoute(nameof(TenantPaymentPage), typeof(TenantPaymentPage));
        Routing.RegisterRoute(nameof(OwnerRevenuePage), typeof(OwnerRevenuePage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute("Profile", typeof(ProfilePage));

        // 2. Intercept and Instantly Route Before Rendering Lifecycle Visuals
        if (!string.IsNullOrWhiteSpace(initialRoute))
        {
            string decodedRoute = System.Uri.UnescapeDataString(initialRoute);

            // Execute clean layout transition directly onto UI execution slots
            Dispatcher.Dispatch(async () =>
            {
                await GoToAsync(decodedRoute, animate: false);
            });
        }
    }
}