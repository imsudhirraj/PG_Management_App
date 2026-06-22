using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Services;
using System.Data;

namespace PGManagement.MobileApp.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly ISecureStorageService _secureStorage;

    [ObservableProperty] private string fullName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string role = string.Empty;

    public ProfileViewModel(IAuthService authService, ISecureStorageService secureStorage)
    {
        _authService = authService;
        _secureStorage = secureStorage;
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        var info = await _secureStorage.GetUserInfoAsync();
        if (info is not null)
        {
            FullName = info.Value.FullName;
            Role = info.Value.Role;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (!confirm) return;

        await _authService.LogoutAsync();
        Application.Current!.Windows[0].Page = new AppShell(); // back to Login
    }
}