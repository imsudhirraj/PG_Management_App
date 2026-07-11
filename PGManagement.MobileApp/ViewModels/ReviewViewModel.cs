using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

public partial class ReviewViewModel
    : ObservableObject
{
    private readonly IApiService _apiService;

    public ObservableCollection<ReviewResponse>
        Reviews
    { get; } = new();

    [ObservableProperty]
    private int pgId;

    public ReviewViewModel(
        IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Reviews.Clear();

        var reviews =
            await _apiService.GetReviewsAsync(PgId);

        if (reviews == null)
            return;

        foreach (var item in reviews)
            Reviews.Add(item);
    }

    [RelayCommand]
    private async Task AddReviewAsync(
        CreateReviewRequest request)
    {
        await _apiService.CreateReviewAsync(request);

        await LoadAsync();
    }
}