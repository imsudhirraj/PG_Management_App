using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class OwnerBookingsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<BookingOverviewResponse> Bookings { get; } = new();

    public OwnerBookingsViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        Bookings.Clear();
        try
        {
            var results = await _apiService.GetAsync<List<BookingOverviewResponse>>("Booking/by-owner");
            if (results is null || results.Count == 0) { StatusMessage = "No bookings yet."; return; }
            foreach (var b in results) Bookings.Add(b);
        }
        catch (Exception ex) { StatusMessage = $"Failed to load: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    //[RelayCommand]
    //private async Task ConfirmBookingAsync(BookingOverviewResponse booking)
    //{
    //    var success = await _apiService.PutAsync($"Booking/{booking.Id}/status", new { Status = "Confirmed" });
    //    if (success) await LoadAsync();
    //}

    [RelayCommand]
    private async Task ApproveBookingAsync(BookingOverviewResponse booking)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Approve Booking",
            $"Approve booking request from {booking.TenantName}?",
            "Approve",
            "Cancel");

        if (!confirm)
            return;

        var success = await _apiService.PutAsync(
            $"Booking/{booking.Id}/status",
            new { Status = "Approved" });

        if (success)
            await LoadAsync();
    }

    [RelayCommand]
    private async Task RejectBookingAsync(BookingOverviewResponse booking)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Reject Booking",
            $"Reject booking request from {booking.TenantName}?",
            "Reject",
            "Cancel");

        if (!confirm)
            return;

        var success = await _apiService.PutAsync(
            $"Booking/{booking.Id}/status",
            new { Status = "Rejected" });

        if (success)
            await LoadAsync();
    }
}