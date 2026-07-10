using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

[QueryProperty(nameof(TargetRoute), "TargetRoute")]
public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    // Manually define the property to completely avoid source generator timing errors
    private string _targetRoute = string.Empty;
    public string TargetRoute
    {
        get => _targetRoute;
        set => SetProperty(ref _targetRoute, value);
    }

    public LoginViewModel(IAuthService authService) => _authService = authService;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both email and password.";
            return;
        }

        IsBusy = true;
        var (success, error) = await _authService.LoginAsync(Email, Password);
        IsBusy = false;

        if (!success)
        {
            ErrorMessage = error ?? "Login failed.";
            return;
        }

        var role = await _authService.GetCurrentRoleAsync();
        await NavigateToRoleHomeAsync(role);
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        string routeParam = !string.IsNullOrWhiteSpace(TargetRoute)
            ? $"?TargetRoute={System.Uri.EscapeDataString(TargetRoute)}"
            : string.Empty;

        await Shell.Current.GoToAsync($"RegisterPage{routeParam}");
    }

    private async Task NavigateToRoleHomeAsync(string? role)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Shell newShell = role switch
            {
                "Owner" => new Views.OwnerShell(),

                // Pass the custom route explicitly into the shell constructor
                "Tenant" => new AppShell(TargetRoute),

                _ => new AppShell(TargetRoute)
            };

            // This instantly assigns the pre-routed shell instance as the primary UI root
            Application.Current!.MainPage = newShell;
        });

        await Task.CompletedTask;
    }

    private static T CreateShell<T>() where T : Shell, new() => new T();
}