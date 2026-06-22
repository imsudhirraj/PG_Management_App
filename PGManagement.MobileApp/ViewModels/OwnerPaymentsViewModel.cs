using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class OwnerPaymentsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<PaymentOverviewResponse> Payments { get; } = new();

    public OwnerPaymentsViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        Payments.Clear();
        try
        {
            var results = await _apiService.GetAsync<List<PaymentOverviewResponse>>("Payment/by-owner");
            if (results is null || results.Count == 0) { StatusMessage = "No payments recorded yet."; return; }
            foreach (var p in results) Payments.Add(p);
        }
        catch (Exception ex) { StatusMessage = $"Failed to load: {ex.Message}"; }
        finally { IsBusy = false; }
    }
}