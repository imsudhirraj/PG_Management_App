using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class AllocationsListViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private PGResponse? selectedPG;

    public ObservableCollection<PGResponse> MyPGs { get; } = new();
    public ObservableCollection<AllocationOverviewResponse> Allocations { get; } = new();

    public AllocationsListViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadMyPGsAsync()
    {
        var pgs = await _apiService.GetAsync<List<PGResponse>>("PG/my-pgs");
        MyPGs.Clear();
        if (pgs is not null) foreach (var pg in pgs) MyPGs.Add(pg);
        if (MyPGs.Count > 0) SelectedPG = MyPGs[0];
    }

    partial void OnSelectedPGChanged(PGResponse? value) => _ = LoadAllocationsAsync();

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
            foreach (var a in results) Allocations.Add(a);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load: {ex.Message}";
        }
        finally { IsBusy = false; }
    }
}