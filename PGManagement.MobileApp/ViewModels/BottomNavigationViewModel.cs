using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Views;

namespace PGManagement.MobileApp.ViewModels;

public partial class BottomNavigationViewModel : ObservableObject
{
    [RelayCommand]
    async Task Home()
    {
        await Shell.Current.GoToAsync($"//{nameof(PGSearchPage)}");
    }

    [RelayCommand]
    async Task Explore()
    {
        await Shell.Current.GoToAsync(nameof(ExplorePage));
    }

    [RelayCommand]
    async Task Bookings()
    {
        await Shell.Current.GoToAsync(nameof(MyBookingsPage));
    }

    [RelayCommand]
    async Task Wishlist()
    {
        await Shell.Current.GoToAsync(nameof(WishlistPage));
    }

    [RelayCommand]
    async Task Profile()
    {
        await Shell.Current.GoToAsync(nameof(ProfilePage));
    }
}