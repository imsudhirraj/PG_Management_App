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
            //builder.Services.AddTransient<RegisterViewModel>();

            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
