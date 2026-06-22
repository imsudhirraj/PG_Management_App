using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using IntelliJ.Lang.Annotations;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class OwnerNoticesViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string newTitle = string.Empty;
    [ObservableProperty] private string newMessage = string.Empty;
    [ObservableProperty] private PGResponse? selectedPG;

    public ObservableCollection<PGResponse> MyPGs { get; } = new();
    public ObservableCollection<NoticeOverviewResponse> Notices { get; } = new();

    public OwnerNoticesViewModel(IApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        Notices.Clear();

        var pgs = await _apiService.GetAsync<List<PGResponse>>("PG/my-pgs");
        MyPGs.Clear();
        if (pgs is not null) foreach (var pg in pgs) MyPGs.Add(pg);
        if (MyPGs.Count > 0 && SelectedPG is null) SelectedPG = MyPGs[0];

        try
        {
            var results = await _apiService.GetAsync<List<NoticeOverviewResponse>>("Notice/by-owner");
            if (results is null || results.Count == 0) { StatusMessage = "No notices posted yet."; return; }
            foreach (var n in results) Notices.Add(n);
        }
        catch (Exception ex) { StatusMessage = $"Failed to load: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task PostNoticeAsync()
    {
        if (SelectedPG is null || string.IsNullOrWhiteSpace(NewTitle) || string.IsNullOrWhiteSpace(NewMessage)) return;

        var success = await _apiService.PostAsync<object, object>("Notice", new
        {
            PGId = SelectedPG.Id,
            Title = NewTitle,
            Message = NewMessage
        });

        NewTitle = string.Empty;
        NewMessage = string.Empty;
        await LoadAsync();
    }
}