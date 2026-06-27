using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.Views;

namespace PGManagement.MobileApp.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private OwnerDashboardStats stats = new();

    public DashboardViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadStatsAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await _apiService.GetAsync<OwnerDashboardStats>("Dashboard/owner");
            if (result is not null) Stats = result;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load dashboard: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand] private async Task GoToAllocationsAsync() => await Shell.Current.GoToAsync("AllocationsListPage");
    [RelayCommand] private async Task GoToPaymentsAsync() => await Shell.Current.GoToAsync("OwnerPaymentsPage");
    [RelayCommand] private async Task GoToComplaintsAsync() => await Shell.Current.GoToAsync("OwnerComplaintsPage");
    [RelayCommand] private async Task GoToBookingsAsync() => await Shell.Current.GoToAsync("OwnerBookingsPage");
    [RelayCommand] private async Task GoToNoticesAsync() => await Shell.Current.GoToAsync("OwnerNoticesPage");
    [RelayCommand] private async Task GoToTenantAsync() => await Shell.Current.GoToAsync("TenantListPage");
    [RelayCommand]
    private async Task NavigateToKycAsync()
    {
        await Shell.Current.GoToAsync(nameof(OwnerKycPage));
    }
    [RelayCommand]
    private async Task PaymentSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(OwnerPaymentSettingsPage));
    }
}