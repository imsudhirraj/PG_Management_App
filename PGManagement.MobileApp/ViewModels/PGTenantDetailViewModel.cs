using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.Views;
using System.Collections.ObjectModel;

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

    // Tracks if this specific property is saved
    [ObservableProperty] private bool isWishlisted;

    public ObservableCollection<RoomResponse> Rooms { get; } = new();

    public PGTenantDetailViewModel(IApiService apiService) => _apiService = apiService;

    partial void OnPgIdChanged(int value) => _ = LoadAsync();

    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        Rooms.Clear();

        // 1. Fetch PG General Info
        try
        {
            var pg = await _apiService.GetAsync<PGResponse>($"PG/{PgId}");
            if (pg is not null)
            {
                Name = pg.Name;
                Address = pg.Address;
                City = pg.City;
                Description = pg.Description ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API ERROR - PG INFO]: {ex.Message}");
            StatusMessage = $"Failed to load PG details: {ex.Message}";
            await Shell.Current.DisplayAlert("Server Error (PG Info)", "The backend crashed while fetching general PG details.", "OK");
        }

        // 2. Optimized Direct Check against the Favourite Controller Exists Endpoint
        try
        {
            // Hits: GET api/Favourite/{pgId}/exists
            var existsInBackend = await _apiService.GetAsync<bool>($"Favourite/{PgId}/exists");
            IsWishlisted = existsInBackend;
        }
        catch (Exception wishlistEx)
        {
            System.Diagnostics.Debug.WriteLine($"[WISHLIST CHECK FAILED]: {wishlistEx.Message}");
            IsWishlisted = false; // Graceful fallback if user is guest or token is missing
        }

        // 3. Fetch Room Info separately
        try
        {
            var rooms = await _apiService.GetAsync<List<RoomResponse>>($"Room/pg/{PgId}");
            if (rooms is not null)
            {
                foreach (var r in rooms)
                    Rooms.Add(r);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API ERROR - ROOMS]: {ex.Message}");
            await Shell.Current.DisplayAlert("Server Error (Rooms)", "The backend crashed while fetching room variations.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
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

    [RelayCommand]
    private async Task ToggleWishlistAsync()
    {
        if (IsBusy) return;

        try
        {
            if (IsWishlisted)
            {
                // Clean bool response wrapper - No live HTTP streams returned!
                bool success = await _apiService.RemoveFromWishlistAsync(PgId);
                if (success)
                {
                    IsWishlisted = false;
                    await Shell.Current.CurrentPage.ShowPopupAsync(new WishlistToastPopup(false));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Action Failed", "Could not remove from wishlist.", "OK");
                }
            }
            else
            {
                // Uses your custom built-in method targeting: POST api/Favourite/{pgId}
                bool success = await _apiService.AddFavouriteAsync(PgId);
                if (success)
                {
                    IsWishlisted = true;
                    await Shell.Current.CurrentPage.ShowPopupAsync(new WishlistToastPopup(true));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Action Failed", "Could not save to wishlist.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WISHLIST TOGGLE CRASH]: {ex.Message}");
            await Shell.Current.DisplayAlert("Connection Error", "Session context timed out. Please try again.", "OK");
        }
    }
}