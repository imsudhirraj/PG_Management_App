using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

public partial class FavouriteViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    public ObservableCollection<FavouriteResponse>
        Favourites
    { get; } = new();

    public FavouriteViewModel(
        IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Favourites.Clear();

        var list =
            await _apiService.GetMyFavouritesAsync();

        if (list == null)
            return;

        foreach (var item in list)
            Favourites.Add(item);
    }

    [RelayCommand]
    private async Task RemoveAsync(
        FavouriteResponse item)
    {
        await _apiService.RemoveFavouriteAsync(item.PGId);

        Favourites.Remove(item);
    }
}