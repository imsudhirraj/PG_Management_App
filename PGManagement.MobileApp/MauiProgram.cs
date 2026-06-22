using Microsoft.Extensions.Logging;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.ViewModels;
using PGManagement.MobileApp.Views;

namespace PGManagement.MobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<PGSearchViewModel>();
            builder.Services.AddTransient<MyBookingsViewModel>();
            builder.Services.AddTransient<MyPGsViewModel>();
            builder.Services.AddTransient<AddPGViewModel>();
            builder.Services.AddTransient<PGDetailViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<TenantListViewModel>();
            builder.Services.AddTransient<TenantDetailViewModel>(); 
            builder.Services.AddTransient<AllocationsListViewModel>();
            builder.Services.AddTransient<OwnerPaymentsViewModel>();
            builder.Services.AddTransient<OwnerComplaintsViewModel>();
            builder.Services.AddTransient<OwnerBookingsViewModel>();
            builder.Services.AddTransient<OwnerNoticesViewModel>();

            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<PGSearchPage>();
            builder.Services.AddTransient<MyBookingsPage>();
            builder.Services.AddTransient<MyPGsPage>();
            builder.Services.AddTransient<AddPGPage>();
            builder.Services.AddTransient<PGDetailPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<TenantListPage>();
            builder.Services.AddTransient<TenantDetailPage>();
            builder.Services.AddTransient<AllocationsListPage>();
            builder.Services.AddTransient<OwnerPaymentsPage>();
            builder.Services.AddTransient<OwnerComplaintsPage>();
            builder.Services.AddTransient<OwnerBookingsPage>();
            builder.Services.AddTransient<OwnerNoticesPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
