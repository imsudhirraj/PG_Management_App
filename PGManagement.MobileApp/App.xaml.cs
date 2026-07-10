using System.Globalization;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.Views;

namespace PGManagement.MobileApp;

public partial class App : Application
{
    private readonly IAuthService _authService;

    public App(IAuthService authService)
    {
        InitializeComponent();

        _authService = authService;
        MainPage = new AppShell();
    }

    [RelayCommand]
    private async Task Navigate(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
            return;

        // 1. PUBLIC INVENTORY ROUTING CHANNELS
        if (destination == "Home")
        {
            await Shell.Current.GoToAsync("///PGSearchPage");
            return;
        }

        if (destination == "Explore")
        {
            await Shell.Current.GoToAsync("///ExplorePage");
            return;
        }

        // 2. PROTECTED INTERCEPT CHANNELS (Bookings, Wishlist, Profile)
        bool loggedIn = await _authService.IsLoggedInAsync();

        if (!loggedIn)
        {
            // Resolve the real absolute Shell route string corresponding to the friendly name token
            string actualShellRoute = GetShellRouteFromDestination(destination);

            // Create instance of custom designed dialog popup
            var authPopup = new AuthPromptPopup();

            // Display the popup over the active shell stream window asynchronously
            var result = await Shell.Current.CurrentPage.ShowPopupAsync(authPopup);

            if (result is string choice)
            {
                // URL Encode the absolute shell path so it can be passed safely as a query property
                string encodedTarget = System.Uri.EscapeDataString(actualShellRoute);

                if (choice == "Log In")
                {
                    await Shell.Current.GoToAsync($"LoginPage?TargetRoute={encodedTarget}");
                }
                else if (choice == "Register")
                {
                    await Shell.Current.GoToAsync($"RegisterPage?TargetRoute={encodedTarget}");
                }
            }

            return;
        }

        // 3. SECURE REDIRECT ROUTING FOR ACTIVE REGISTERED USERS
        string route = GetShellRouteFromDestination(destination);
        if (!string.IsNullOrEmpty(route))
        {
            await Shell.Current.GoToAsync(route);
        }
    }

    /// <summary>
    /// Maps the descriptive navigation string token to its corresponding actual Shell page absolute route.
    /// </summary>
    private string GetShellRouteFromDestination(string destination)
    {
        return destination switch
        {
            "Bookings" => "///MyBookingsPage",
            "Wishlist" => "///WishlistPage",
            "Profile" => "///ProfilePage",
            "Home" => "///PGSearchPage",
            "Explore" => "///ExplorePage",
            _ => destination // Fallback if destination was already an actual path string
        };
    }
}

public class WishlistColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isSaved = value is bool state && state;

        return isSaved
            ? Color.FromArgb("#4A154B")   // Deep brand purple when saved/filled
            : Color.FromArgb("#94A3B8");   // Muted slate outline when unsaved/blank
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class WishlistGlyphConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool isSaved && isSaved)
            ? "\x2665"   // Solid Filled Heart: ♥
            : "\x2661";   // Clear Outlined Heart: ♡
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class EmptyCollectionToVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int count && count == 0;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}