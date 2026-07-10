using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.Views;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class WishlistViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isUserAnonymous;

    [ObservableProperty]
    private string emptyStateMessage = "Your wishlist is empty.";

    public ObservableCollection<PGCardResponse> WishlistItems { get; } = new();

    // --- MANUALLY DECLARE COMMANDS TO BYPASS SOURCE GENERATOR GLITCHES ---
    public IAsyncRelayCommand LoadWishlistAsyncCommand { get; }
    public IAsyncRelayCommand<PGCardResponse> RemoveFromWishlistAsyncCommand { get; }
    public IAsyncRelayCommand<PGCardResponse> GoToDetailsAsyncCommand { get; }
    public IAsyncRelayCommand RedirectToLoginAsyncCommand { get; }

    public WishlistViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;

        // Initialize commands directly linking to their underlying tasks
        LoadWishlistAsyncCommand = new AsyncRelayCommand(LoadWishlistAsync);
        RemoveFromWishlistAsyncCommand = new AsyncRelayCommand<PGCardResponse>(RemoveFromWishlistAsync);
        GoToDetailsAsyncCommand = new AsyncRelayCommand<PGCardResponse>(GoToDetailsAsync);
        RedirectToLoginAsyncCommand = new AsyncRelayCommand(RedirectToLoginAsync);
    }

    public async Task LoadWishlistAsync()
    {
        if (IsBusy) return;

        bool loggedIn = await _authService.IsLoggedInAsync();
        if (!loggedIn)
        {
            IsUserAnonymous = true;
            EmptyStateMessage = "Please log in to view your saved bookmarked properties.";
            WishlistItems.Clear();
            return;
        }

        IsUserAnonymous = false;
        IsBusy = true;

        try
        {
            // 1. Fetch data safely from the network decoupled API helper string string stream
            var items = await _apiService.GetWishlistPropertiesAsync();
            var safeListItems = items?.ToList() ?? new List<PGCardResponse>();

            // 2. Perform UI manipulations inside the Main Thread to avoid Object Disposed Collection crashes
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                WishlistItems.Clear();

                if (safeListItems.Any())
                {
                    foreach (var item in safeListItems)
                    {
                        WishlistItems.Add(item);
                    }
                }
                else
                {
                    EmptyStateMessage = "You haven't saved any PGs yet!";
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WISHLIST RECONCILIATION ERROR]: {ex.Message}");
            EmptyStateMessage = "Could not pull down your saved listings.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RemoveFromWishlistAsync(PGCardResponse pgItem)
    {
        if (pgItem == null || IsBusy) return;

        bool confirm = await Shell.Current.DisplayAlert("Remove Saved PG", "Are you sure you want to remove this property from your wishlist?", "Yes", "No");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            // Hits: DELETE api/Favourite/{pgId}
            bool isDeleted = await _apiService.RemoveFromWishlistAsync(pgItem.Id);

            if (isDeleted)
            {
                // Always modify ObservableCollections on the Main Thread to prevent structural UI thread crashes
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    WishlistItems.Remove(pgItem);

                    if (!WishlistItems.Any())
                    {
                        EmptyStateMessage = "You haven't saved any PGs yet!";
                    }
                });

                // Show our custom notification toast popup
                await Shell.Current.CurrentPage.ShowPopupAsync(new WishlistToastPopup(false));
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "The server refused to remove this item. Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WISHLIST REMOVAL CRASH]: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Failed to delete item: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToDetailsAsync(PGCardResponse pg)
    {
        if (pg == null) return;
        await Shell.Current.GoToAsync($"PGTenantDetailPage?id={pg.Id}");
    }

    private async Task RedirectToLoginAsync()
    {
        await Shell.Current.GoToAsync("LoginPage");
    }

    [RelayCommand]
    private async Task SelectWishlistItemAsync(PGCardResponse selectedPg)
    {
        if (selectedPg == null) return;

        // Matches your exact working Explore routing parameter: ?id={value}
        await Shell.Current.GoToAsync($"PGTenantDetailPage?id={selectedPg.Id}");
    }
}