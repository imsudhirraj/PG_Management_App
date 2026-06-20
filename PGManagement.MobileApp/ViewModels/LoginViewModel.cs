using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

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
    private async Task GoToRegisterAsync() => await Shell.Current.GoToAsync("//RegisterPage");

    private async Task NavigateToRoleHomeAsync(string? role)
    {
        // We'll wire actual role-based shells in the next step.
        // For now, navigate to a placeholder home route.
        await Shell.Current.GoToAsync("HomePage");
    }
}