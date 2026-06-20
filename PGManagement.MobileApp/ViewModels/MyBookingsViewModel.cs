using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class MyBookingsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<BookingResponse> Bookings { get; } = new();

    public MyBookingsViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadBookingsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = string.Empty;
        Bookings.Clear();

        try
        {
            var results = await _apiService.GetAsync<List<BookingResponse>>("Booking/my-bookings");
            if (results is null || results.Count == 0)
            {
                StatusMessage = "You don't have any bookings yet.";
                return;
            }
            foreach (var b in results) Bookings.Add(b);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load bookings: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}