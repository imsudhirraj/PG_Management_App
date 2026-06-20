using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class MyPGsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<PGResponse> MyPGs { get; } = new();

    public MyPGsViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadMyPGsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = string.Empty;
        MyPGs.Clear();

        try
        {
            var results = await _apiService.GetAsync<List<PGResponse>>("PG/my-pgs");
            if (results is null || results.Count == 0)
            {
                StatusMessage = "You haven't added any PGs yet. Tap + to add one.";
                return;
            }
            foreach (var pg in results) MyPGs.Add(pg);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToAddPGAsync() => await Shell.Current.GoToAsync("AddPGPage");
}