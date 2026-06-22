using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class TenantListViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<TenantWithRoomResponse> Tenants { get; } = new();

    public TenantListViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadTenantsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = string.Empty;
        Tenants.Clear();

        try
        {
            var results = await _apiService.GetAsync<List<TenantWithRoomResponse>>("Tenant/by-owner");
            if (results is null || results.Count == 0)
            {
                StatusMessage = "No tenants found across your PGs yet.";
                return;
            }
            foreach (var t in results) Tenants.Add(t);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task GoToTenantDetailAsync(TenantWithRoomResponse tenant)
        => await Shell.Current.GoToAsync($"TenantDetailPage?id={tenant.Id}");
}