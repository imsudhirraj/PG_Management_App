using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

[QueryProperty(nameof(pgId), "id")]
public partial class PGDetailViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private int pgId;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string address = string.Empty;
    [ObservableProperty] private string city = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private decimal latitude;
    [ObservableProperty] private decimal longitude;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool isEditing;

    public PGDetailViewModel(IApiService apiService) => _apiService = apiService;

    partial void OnPgIdChanged(int value) => _ = LoadAsync();

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var pg = await _apiService.GetAsync<PGResponse>($"PG/{PgId}");
            if (pg is null) { StatusMessage = "PG not found."; return; }
            Name = pg.Name; Address = pg.Address; City = pg.City;
            Description = pg.Description ?? string.Empty;
            Latitude = pg.Latitude; Longitude = pg.Longitude;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void ToggleEdit() => IsEditing = !IsEditing;

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsBusy = true;
        try
        {
            var success = await _apiService.PutAsync($"PG/{PgId}", new UpdatePGRequest
            {
                Name = Name,
                Address = Address,
                City = City,
                Description = Description,
                Latitude = Latitude,
                Longitude = Longitude
            });

            if (success)
            {
                IsEditing = false;
                await Shell.Current.DisplayAlert("Success", "PG updated.", "OK");
            }
            else
            {
                StatusMessage = "Update failed.";
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Delete PG", $"Delete '{Name}'? This cannot be undone.", "Delete", "Cancel");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            var success = await _apiService.DeleteAsync($"PG/{PgId}");
            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusMessage = "Delete failed.";
            }
        }
        finally { IsBusy = false; }
    }

}