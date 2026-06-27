using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class PGSearchViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private double radiusKm = 50;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<PGSearchResult> Results { get; } = new();

    private double? _userLat;
    private double? _userLng;

    public PGSearchViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task SearchNearbyAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = "Getting your location...";
        Results.Clear();

        try
        {
            var location = await GetCurrentLocationAsync();
            if (location is null)
            {
                StatusMessage = "Couldn't get your location. Check location permissions.";
                return;
            }

            _userLat = location.Latitude;
            _userLng = location.Longitude;

            await Shell.Current.DisplayAlert(
            "My Location",
            $"{_userLat}, {_userLng}",
            "OK");

            StatusMessage = "Searching nearby PGs...";
            var query = $"PG/search?lat={_userLat}&lng={_userLng}&radiusKm={RadiusKm}";
            var results = await _apiService.GetAsync<List<PGSearchResult>>(query);

            if (results is null || results.Count == 0)
            {
                StatusMessage = $"No PGs found within {RadiusKm} km with available beds.";
                return;
            }

            foreach (var r in results) Results.Add(r);
            StatusMessage = $"Found {results.Count} PG(s) nearby.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Search failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            System.Diagnostics.Debug.WriteLine($"Permission status: {status}");
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert(
                    "Permission Required",
                    "Location permission is required to find nearby PGs.",
                    "OK");

                await Launcher.Default.OpenAsync("app-settings:");
                return null;
            }
            // Try a fresh fix first
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            Location? location = null;

            try
            {
                location = await Geolocation.Default.GetLocationAsync(request);
            }
            catch (FeatureNotEnabledException)
            {
                await Shell.Current.DisplayAlert(
                    "GPS Disabled",
                    "Please turn on Location Services (GPS).",
                    "OK");

                await Launcher.Default.OpenAsync("app-settings:");
                return null;
            }
            // Fallback to last known location (more reliable on emulators)
            location ??= await Geolocation.Default.GetLastKnownLocationAsync();

            System.Diagnostics.Debug.WriteLine(
                location is null ? "Location is null after both attempts" : $"Got location: {location.Latitude}, {location.Longitude}");

            return location;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Location error: {ex}");
            return null;
        }
    }
    [RelayCommand]
    private async Task GoToPGDetailAsync(PGSearchResult pg) => await Shell.Current.GoToAsync($"PGTenantDetailPage?id={pg.Id}");
}