using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class OwnerComplaintsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<ComplaintOverviewResponse> Complaints { get; } = new();

    public OwnerComplaintsViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        Complaints.Clear();
        try
        {
            var results = await _apiService.GetAsync<List<ComplaintOverviewResponse>>("Complaint/by-owner");
            if (results is null || results.Count == 0) { StatusMessage = "No complaints raised yet."; return; }
            foreach (var c in results) Complaints.Add(c);
        }
        catch (Exception ex) { StatusMessage = $"Failed to load: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task MarkResolvedAsync(ComplaintOverviewResponse complaint)
    {
        var success = await _apiService.PutAsync($"Complaint/{complaint.Id}/status", new { Status = "Resolved" });
        if (success) await LoadAsync();
    }
}