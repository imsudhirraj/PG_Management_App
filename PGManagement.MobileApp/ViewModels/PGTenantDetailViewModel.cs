using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

[QueryProperty(nameof(PgId), "id")]
public partial class PGTenantDetailViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private int pgId;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string address = string.Empty;
    [ObservableProperty] private string city = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string bookingMessage = string.Empty;

    public ObservableCollection<RoomResponse> Rooms { get; } = new();


    public PGTenantDetailViewModel(IApiService apiService) => _apiService = apiService;

    partial void OnPgIdChanged(int value) => _ = LoadAsync();

    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        Rooms.Clear();

        try
        {
            var pg = await _apiService.GetAsync<PGResponse>($"PG/{PgId}");
            if (pg is not null)
            {
                Name = pg.Name; Address = pg.Address; City = pg.City;
                Description = pg.Description ?? string.Empty;
            }

            var rooms = await _apiService.GetAsync<List<RoomResponse>>($"Room/pg/{PgId}");
            if (rooms is not null)
                foreach (var r in rooms) Rooms.Add(r);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task BookRoomAsync(RoomResponse room)
    {
        if (room.AvailableBeds <= 0)
        {
            await Shell.Current.DisplayAlert("Unavailable", "No beds available in this room.", "OK");
            return;
        }

        bool confirm = await Shell.Current.DisplayAlert("Confirm Booking",
            $"Book Room {room.RoomNumber} ({room.RoomType}) for ₹{room.RentAmount}/month?", "Book", "Cancel");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            var result = await _apiService.PostRawAsync(
                "Booking",
                new CreateBookingRequest
                {
                    RoomId = room.Id,
                    Message = BookingMessage
                }); if (result.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Success", "Booking request sent. The owner will confirm shortly.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                var body = await result.Content.ReadAsStringAsync();
                StatusMessage = $"Booking failed: {body}";
            }
        }
        finally { IsBusy = false; }
    }
}