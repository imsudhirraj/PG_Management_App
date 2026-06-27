using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

public partial class AddPGViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string address = string.Empty;
    [ObservableProperty] private string city = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string latitude = string.Empty;
    [ObservableProperty] private string longitude = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    public AddPGViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task UseCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert(
                    "Permission Required",
                    "Location permission is required to get your current location.",
                    "OK");

                await Launcher.Default.OpenAsync("app-settings:");
                return;
            }

            Location? location = null;

            try
            {
                var request = new GeolocationRequest(
                    GeolocationAccuracy.High,
                    TimeSpan.FromSeconds(15));

                location = await Geolocation.Default.GetLocationAsync(request);

                // Fallback
                location ??= await Geolocation.Default.GetLastKnownLocationAsync();
            }
            catch (FeatureNotEnabledException)
            {
                await Shell.Current.DisplayAlert(
                    "GPS Disabled",
                    "Please enable Location Services (GPS) and try again.",
                    "OK");

                await Launcher.Default.OpenAsync("app-settings:");
                return;
            }

            if (location == null)
            {
                await Shell.Current.DisplayAlert(
                    "Location Unavailable",
                    "Unable to determine your current location.",
                    "OK");
                return;
            }

            Latitude = location.Latitude.ToString("F6");
            Longitude = location.Longitude.ToString("F6");

            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Couldn't get location: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Address) ||
            string.IsNullOrWhiteSpace(City) || !decimal.TryParse(Latitude, out var lat) ||
            !decimal.TryParse(Longitude, out var lng))
        {
            ErrorMessage = "Please fill all required fields with valid values.";
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _apiService.PostAsync<CreatePGRequest, object>("PG", new CreatePGRequest
            {
                Name = Name,
                Address = Address,
                City = City,
                Description = Description,
                Latitude = lat,
                Longitude = lng
            });

            await Shell.Current.DisplayAlert("Success", "PG added successfully.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}