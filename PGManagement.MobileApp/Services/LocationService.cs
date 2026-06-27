using Microsoft.Maui.ApplicationModel;

namespace PGManagement.MobileApp.Services;

public class LocationService
{
    public async Task<Location?> GetCurrentLocationAsync()
    {
        var status =
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
            return null;

        return await Geolocation.Default.GetLocationAsync(
            new GeolocationRequest(
                GeolocationAccuracy.High,
                TimeSpan.FromSeconds(10)));
    }
}