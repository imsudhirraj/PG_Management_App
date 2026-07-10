using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.Views;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class ExploreViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    private readonly List<PGCardResponse> _allPropertiesCache = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedGender = "All";

    [ObservableProperty]
    private decimal maxRent = 25000;

    [ObservableProperty]
    private string resultSummary = "Loading properties...";

    public ObservableCollection<PGCardResponse> FilteredProperties { get; } = new();
    public List<string> GenderOptions { get; } = new() { "All", "Boys", "Girls", "Family" };

    // --- MANUALLY DECLARE COMMANDS TO BYPASS SOURCE GENERATOR GLITCHES ---
    public IAsyncRelayCommand LoadAllPropertiesAsyncCommand { get; }
    public IRelayCommand ApplyFacetedFiltersCommand { get; }
    public IAsyncRelayCommand<PGCardResponse> GoToDetailsAsyncCommand { get; }

    public ExploreViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
        // Initialize commands directly linking to their underlying tasks
        LoadAllPropertiesAsyncCommand = new AsyncRelayCommand(LoadAllPropertiesAsync);
        ApplyFacetedFiltersCommand = new RelayCommand(ApplyFacetedFilters);
        GoToDetailsAsyncCommand = new AsyncRelayCommand<PGCardResponse>(GoToDetailsAsync);
    }

    public async Task LoadAllPropertiesAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            _allPropertiesCache.Clear();
            FilteredProperties.Clear();

            var properties = await _apiService.GetAllPropertiesAsync();
            if (properties != null)
            {
                _allPropertiesCache.AddRange(properties);
            }

            ApplyFacetedFilters();
        }
        catch (Exception ex)
        {
            ResultSummary = "Error loading exploration feed.";
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void ApplyFacetedFilters()
    {
        IEnumerable<PGCardResponse> subset = _allPropertiesCache;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            subset = subset.Where(p =>
                p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (p.Address != null && p.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                (p.City != null && p.City.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        if (SelectedGender != "All")
        {
            subset = subset.Where(p => p.GenderType != null && p.GenderType.Equals(SelectedGender, StringComparison.OrdinalIgnoreCase));
        }

        subset = subset.Where(p => p.StartingRent <= MaxRent);

        FilteredProperties.Clear();
        foreach (var item in subset)
        {
            FilteredProperties.Add(item);
        }

        ResultSummary = $"Showing {FilteredProperties.Count} properties matching filters.";
    }

    private async Task GoToDetailsAsync(PGCardResponse pg)
    {
        if (pg == null) return;
        bool loggedIn = await _authService.IsLoggedInAsync();
        if (!loggedIn)
        {
            // 1. Get the current active page route location dynamically
            string destination = Shell.Current.CurrentState.Location.ToString();

            // Create instance of custom designed dialog popup
            var authPopup = new AuthPromptPopup();

            // Display the popup over the active shell stream window asynchronously
            var result = await Shell.Current.CurrentPage.ShowPopupAsync(authPopup);

            if (result is string choice)
            {
                if (choice == "Log In")
                {
                    // 2. Properly URL Encode the route to safely pass path segments as a query param
                    await Shell.Current.GoToAsync($"LoginPage?TargetRoute={System.Uri.EscapeDataString(destination)}");
                }
                else if (choice == "Register")
                {
                    await Shell.Current.GoToAsync($"RegisterPage?TargetRoute={System.Uri.EscapeDataString(destination)}");
                }
            }

            return;
        }
        await Shell.Current.GoToAsync($"PGTenantDetailPage?id={pg.Id}");
    }
}