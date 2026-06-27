using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.Views;
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
            var token = await Microsoft.Maui.Storage.SecureStorage.Default.GetAsync("auth_token");
            System.Diagnostics.Debug.WriteLine($"Token before my-pgs call: {token ?? "NULL/EMPTY"}");

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

    [RelayCommand]
    private async Task OpenKycAsync(BookingResponse booking)
    {
        await Shell.Current.GoToAsync(
            $"TenantKycPage?bookingId={booking.Id}");
    }

    [RelayCommand]
    private async Task OpenPaymentAsync(BookingResponse booking)
    {
        await Shell.Current.GoToAsync(
            $"TenantPaymentPage?bookingId={booking.Id}");
    }

    [RelayCommand]
    private async Task OpenChatAsync(BookingResponse booking)
    {
        await Shell.Current.GoToAsync(
            $"TenantChatPage?bookingId={booking.Id}");
    }

    [RelayCommand]
    private async Task UploadKycAsync(BookingResponse booking)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(TenantKycPage)}?bookingId={booking.Id}");
    }

    [RelayCommand]
    private async Task PayAdvanceAsync(BookingResponse booking)
    {
        await Shell.Current.GoToAsync(
            nameof(TenantPaymentPage),
            new Dictionary<string, object>
            {
                ["bookingId"] = booking.Id
            });
    }

    [RelayCommand]
    private async Task ChatWithOwnerAsync(BookingResponse booking)
    {
        await Shell.Current.DisplayAlert(
            "Coming Soon",
            "Chat module will be implemented later.",
            "OK");
    }
}