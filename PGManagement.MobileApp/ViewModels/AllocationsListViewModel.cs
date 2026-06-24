using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class AllocationsListViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string allocateError = string.Empty;
    [ObservableProperty] private PGResponse? selectedPG;

    [ObservableProperty] private RoomResponse? selectedRoom;
    [ObservableProperty] private UnallocatedTenantResponse? selectedTenant; // Matches your model name
    [ObservableProperty] private DateTime allocatedFrom = DateTime.Today;

    public ObservableCollection<PGResponse> MyPGs { get; } = new();
    public ObservableCollection<AllocationOverviewResponse> Allocations { get; } = new();
    public ObservableCollection<RoomResponse> Rooms { get; } = new();
    public ObservableCollection<UnallocatedTenantResponse> UnallocatedTenants { get; } = new(); // Matches your model name

    public AllocationsListViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadMyPGsAsync()
    {
        try
        {
            var pgs = await _apiService.GetAsync<List<PGResponse>>("PG/my-pgs");
            MyPGs.Clear();
            if (pgs is not null) foreach (var pg in pgs) MyPGs.Add(pg);
            if (MyPGs.Count > 0) SelectedPG = MyPGs[0];
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load PGs: {ex.Message}";
        }
    }

    partial void OnSelectedPGChanged(PGResponse? value)
    {
        _ = LoadAllocationsAsync();
        _ = LoadFormLookupDataAsync();
    }

    [RelayCommand]
    private async Task LoadAllocationsAsync()
    {
        if (SelectedPG is null) return;
        IsBusy = true;
        StatusMessage = string.Empty;
        Allocations.Clear();

        try
        {
            var results = await _apiService.GetAsync<List<AllocationOverviewResponse>>($"RoomAllocation/by-pg/{SelectedPG.Id}");

            if (results is null || results.Count == 0)
            {
                StatusMessage = "No allocations for this PG yet.";
                return;
            }

            // 🌟 FIX: Filter the results list to only keep items where IsActive is true
            var activeAllocations = results.Where(a => a.IsActive);

            // Check if there are any active ones left after filtering
            if (!activeAllocations.Any())
            {
                StatusMessage = "No active allocations for this PG.";
                return;
            }

            foreach (var a in activeAllocations)
            {
                Allocations.Add(a);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load allocations: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private async Task LoadFormLookupDataAsync()
    {
        if (SelectedPG is null) return;

        AllocateError = string.Empty;
        Rooms.Clear();
        UnallocatedTenants.Clear();

        try
        {
            // 1. Fetch Rooms for this property
            var roomsList = await _apiService.GetAsync<List<RoomResponse>>($"Room/pg/{SelectedPG.Id}");
            if (roomsList is not null)
            {
                foreach (var r in roomsList) Rooms.Add(r);
            }

            // 2. Fetch Unallocated Tenants via your brand new endpoint
            var tenantsList = await _apiService.GetAsync<List<UnallocatedTenantResponse>>("Tenant/unallocated");
            if (tenantsList is not null)
            {
                foreach (var tenant in tenantsList)
                {
                    UnallocatedTenants.Add(tenant);
                }
            }
        }
        catch (Exception ex)
        {
            AllocateError = $"Lookup load error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task AllocateAsync()
    {
        if (SelectedRoom is null || SelectedTenant is null)
        {
            AllocateError = "Please select a valid Room and Tenant.";
            return;
        }

        AllocateError = string.Empty;
        IsBusy = true;

        try
        {
            var request = new
            {
                RoomId = SelectedRoom.Id,
                TenantId = SelectedTenant.Id,
                AllocatedFrom = AllocatedFrom
            };

            var response = await _apiService.PostRawAsync("RoomAllocation/allocate", request);
            if (response.IsSuccessStatusCode)
            {
                SelectedRoom = null;
                SelectedTenant = null;
                AllocatedFrom = DateTime.Today;

                await Shell.Current.DisplayAlert("Success", "Room allocated successfully!", "OK");

                await LoadAllocationsAsync();
                await LoadFormLookupDataAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                AllocateError = $"Allocation failed: {error}";
            }
        }
        catch (Exception ex)
        {
            AllocateError = $"An error occurred: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task VacateAsync(AllocationOverviewResponse allocation)
    {
        if (allocation is null) return;

        bool confirm = await Shell.Current.DisplayAlert("Vacate Bed",
            $"Vacate {allocation.TenantName} from Room {allocation.RoomNumber}?", "Vacate", "Cancel");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            var response = await _apiService.PostEmptyBodyAsync($"RoomAllocation/{allocation.Id}/vacate");
            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Success", "Tenant marked as vacated.", "OK");
                await LoadAllocationsAsync();
                await LoadFormLookupDataAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("Error", $"Vacate failed: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        finally { IsBusy = false; }
    }
}