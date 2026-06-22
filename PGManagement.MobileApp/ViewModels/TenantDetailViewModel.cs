using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

[QueryProperty(nameof(TenantId), "id")]
public partial class TenantDetailViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private int tenantId;
    [ObservableProperty] private string fullName = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string emergencyContactName = string.Empty;
    [ObservableProperty] private string emergencyContactPhone = string.Empty;
    [ObservableProperty] private string roomNumber = string.Empty;
    [ObservableProperty] private string pgName = string.Empty;
    [ObservableProperty] private DateTime? checkInDate;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool isEditing;

    public TenantDetailViewModel(IApiService apiService) => _apiService = apiService;

    partial void OnTenantIdChanged(int value) => _ = LoadAsync();

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var tenants = await _apiService.GetAsync<List<TenantWithRoomResponse>>("Tenant/by-owner");
            var tenant = tenants?.FirstOrDefault(t => t.Id == TenantId);
            if (tenant is null) { StatusMessage = "Tenant not found."; return; }

            FullName = tenant.FullName;
            Phone = tenant.Phone;
            EmergencyContactName = tenant.EmergencyContactName ?? string.Empty;
            EmergencyContactPhone = tenant.EmergencyContactPhone ?? string.Empty;
            RoomNumber = tenant.RoomNumber;
            PgName = tenant.PGName;
            CheckInDate = tenant.CheckInDate;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void ToggleEdit() => IsEditing = !IsEditing;

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsBusy = true;
        try
        {
            var success = await _apiService.PutAsync($"Tenant/{TenantId}", new UpdateTenantRequest
            {
                FullName = FullName,
                Phone = Phone,
                EmergencyContactName = EmergencyContactName,
                EmergencyContactPhone = EmergencyContactPhone
            });

            if (success)
            {
                IsEditing = false;
                await Shell.Current.DisplayAlert("Success", "Tenant details updated.", "OK");
            }
            else StatusMessage = "Update failed.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Checkout Tenant",
            $"Check out {FullName}? This will free up their bed.", "Checkout", "Cancel");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            var success = await _apiService.PostRawAsync<object?>($"Tenant/{TenantId}/checkout", null);
            if (success.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Done", "Tenant checked out.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else StatusMessage = "Checkout failed.";
        }
        finally { IsBusy = false; }
    }
}