using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string fullName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string selectedRole = "Tenant";
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    public List<string> Roles { get; } = new() { "Tenant", "Owner" };
    // SuperAdmin intentionally excluded — that role should be seeded/assigned manually, not self-registered

    public RegisterViewModel(IAuthService authService) => _authService = authService;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please fill in all fields.";
            return;
        }

        IsBusy = true;
        var (success, error) = await _authService.RegisterAsync(Email, Password, FullName, SelectedRole);
        IsBusy = false;

        if (!success)
        {
            ErrorMessage = error ?? "Registration failed.";
            return;
        }

        await Shell.Current.DisplayAlert("Success", "Account created. Please login.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private async Task GoToLoginAsync() => await Shell.Current.GoToAsync("//LoginPage");
}