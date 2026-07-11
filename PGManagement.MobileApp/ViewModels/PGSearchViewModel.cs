using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using PGManagement.MobileApp.Views;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class PGSearchViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    public PGSearchViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
        SeedCategories();
    }

    [ObservableProperty]
    private bool isBusy;

    // Dedicated to RefreshView.IsRefreshing ONLY. Deliberately NOT the same
    // flag as IsBusy — binding RefreshView's IsRefreshing to a flag that is
    // also flipped by other flows (page load, search, locate) creates a
    // well-known MAUI feedback loop: RefreshView treats any IsRefreshing
    // transition to true as a refresh trigger, including ones caused by our
    // own binding, so it can end up re-invoking RefreshCommand and never
    // settling — visually an "infinite loop" that never finishes loading.
    // Only RefreshAsync ever touches this property.
    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private double radiusKm = 10;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private double? userLatitude;

    [ObservableProperty]
    private double? userLongitude;

    public ObservableCollection<CategoryChip> Categories { get; } = new();
    public ObservableCollection<PGCardResponse> FeaturedPGs { get; } = new();
    public ObservableCollection<PGCardResponse> NearbyPGs { get; } = new();
    public ObservableCollection<PGCardResponse> RecommendedPGs { get; } = new();
    public ObservableCollection<PGCardResponse> RecentPGs { get; } = new();

    private List<PGCardResponse> _rawNearbyCache = new();

    // Tracks which mode _rawNearbyCache was populated from (text search vs
    // GPS-based nearby). Since this ViewModel is registered as a Singleton
    // (needed to avoid a full reload every time the user returns to the
    // home page), _rawNearbyCache can otherwise survive across a mode
    // switch — e.g. the user types a name search, then clears it and taps
    // Locate. Without this check the stale text-search results (which come
    // from a different backend query with no Room join, so rent/beds are
    // never populated) would get reused and shown as if they were fresh
    // GPS-nearby results, alongside a stale/incorrect UserLatitude /
    // UserLongitude used for the displayed distance.
    private bool? _cacheIsFromTextSearch;

    // Tracks whether the home page has ever completed a successful load.
    // Lets OnAppearing avoid a full network reload every time the user
    // navigates back from a detail page.
    private bool _hasLoadedOnce;

    // Re-entrancy guards. Prevent duplicate command execution when the same
    // gesture fires the command more than once (e.g. a Button inside a
    // Border that also has a TapGestureRecognizer bound to the same command
    // can cause both handlers to fire from a single tap on some platforms).
    private bool _isNavigatingToDetail;
    private bool _isSearching;

    private void SeedCategories()
    {
        Categories.Clear();
        Categories.Add(new CategoryChip { Name = "All", Icon = "🏠", IsSelected = true });
        Categories.Add(new CategoryChip { Name = "Boys", Icon = "👦" });
        Categories.Add(new CategoryChip { Name = "Girls", Icon = "👧" });
        Categories.Add(new CategoryChip { Name = "Family", Icon = "👨‍👩‍👧" });
        Categories.Add(new CategoryChip { Name = "AC", Icon = "❄️" });
        Categories.Add(new CategoryChip { Name = "Food", Icon = "🍽️" });
        Categories.Add(new CategoryChip { Name = "WiFi", Icon = "📶" });
        Categories.Add(new CategoryChip { Name = "Parking", Icon = "🚗" });
    }

    // Public command entry point. Owns IsBusy / StatusMessage for the
    // "search only" flow (Search Properties button, category tap). This is
    // an explicit user action, so it's allowed to prompt for location
    // permission if needed.
    [RelayCommand]
    private async Task SearchNearbyAsync()
    {
        if (_isSearching) return;
        _isSearching = true;

        bool ownsBusyFlag = !IsBusy;
        if (ownsBusyFlag) IsBusy = true;

        try
        {
            await SearchNearbyInternalAsync(allowLocationPrompt: true);
        }
        finally
        {
            _isSearching = false;
            if (ownsBusyFlag) IsBusy = false;
        }
    }

    // Internal worker. Does NOT touch IsBusy itself, so it can be safely
    // awaited alongside the other home-page loads inside Task.WhenAll
    // without one flow prematurely flipping IsBusy off for the others.
    //
    // allowLocationPrompt controls whether we're allowed to show the OS
    // location-permission dialog. Page load passes false so the app never
    // asks for location on startup; only an explicit user tap (Locate /
    // Search Properties / category filter) passes true.
    private async Task SearchNearbyInternalAsync(bool allowLocationPrompt)
    {
        bool hasSearchText = !string.IsNullOrWhiteSpace(SearchText);

        NearbyPGs.Clear();

        try
        {
            // ==========================================================
            // TEXT SEARCH (City/Name)
            // ==========================================================
            if (hasSearchText)
            {
                StatusMessage = "Searching...";

                var searchResults = await _apiService.SearchPGsByNameAsync(SearchText.Trim());

                var filteredList = searchResults?.AsEnumerable() ?? Enumerable.Empty<PGCardResponse>();

                // Apply category filter
                var selectedChip = Categories.FirstOrDefault(c => c.IsSelected);
                if (selectedChip != null && selectedChip.Name != "All")
                {
                    filteredList = filteredList.Where(x => selectedChip.Name switch
                    {
                        "Boys" => x.GenderType?.Equals("Boys", StringComparison.OrdinalIgnoreCase) == true,
                        "Girls" => x.GenderType?.Equals("Girls", StringComparison.OrdinalIgnoreCase) == true,
                        "Family" => x.GenderType?.Equals("Family", StringComparison.OrdinalIgnoreCase) == true,
                        "AC" => x.PropertyType?.Contains("AC", StringComparison.OrdinalIgnoreCase) == true,
                        "Food" => x.PropertyType?.Contains("Food", StringComparison.OrdinalIgnoreCase) == true,
                        "WiFi" => x.PropertyType?.Contains("WiFi", StringComparison.OrdinalIgnoreCase) == true,
                        "Parking" => x.PropertyType?.Contains("Parking", StringComparison.OrdinalIgnoreCase) == true,
                        _ => true
                    });
                }

                foreach (var item in filteredList)
                {
                    item.DistanceKm = 0;   // Distance not applicable for text search
                    NearbyPGs.Add(item);
                }

                StatusMessage = $"{NearbyPGs.Count} PG(s) found matching criteria.";
                return;
            }

            // ==========================================================
            // GPS + RADIUS SEARCH
            // ==========================================================

            StatusMessage = "Searching nearby PGs...";

            var location = await GetCurrentLocationAsync(allowLocationPrompt);

            if (location == null)
            {
                StatusMessage = allowLocationPrompt
                    ? "Unable to get your location."
                    : "Tap Locate to find nearby PGs.";
                return;
            }

            UserLatitude = location.Latitude;
            UserLongitude = location.Longitude;

            var nearbyResults = await _apiService.GetNearbyPGsAsync(
                (decimal)location.Latitude,
                (decimal)location.Longitude,
                RadiusKm);

            if (nearbyResults == null || !nearbyResults.Any())
            {
                StatusMessage = "No PGs found matching criteria.";
                return;
            }

            var center = new Location(location.Latitude, location.Longitude);

            foreach (var pg in nearbyResults)
            {
                try
                {
                    if (Convert.ToDouble(pg.Latitude) != 0 &&
                        Convert.ToDouble(pg.Longitude) != 0)
                    {
                        pg.DistanceKm = Location.CalculateDistance(
                            center,
                            new Location(
                                Convert.ToDouble(pg.Latitude),
                                Convert.ToDouble(pg.Longitude)),
                            DistanceUnits.Kilometers);
                    }
                }
                catch
                {
                    // Ignore distance calculation failures
                }
            }

            IEnumerable<PGCardResponse> finalList =
                nearbyResults.Where(x => x.DistanceKm <= RadiusKm || x.DistanceKm == 0);

            // Apply category filter
            var chip = Categories.FirstOrDefault(c => c.IsSelected);

            if (chip != null && chip.Name != "All")
            {
                finalList = finalList.Where(x => chip.Name switch
                {
                    "Boys" => x.GenderType?.Equals("Boys", StringComparison.OrdinalIgnoreCase) == true,
                    "Girls" => x.GenderType?.Equals("Girls", StringComparison.OrdinalIgnoreCase) == true,
                    "Family" => x.GenderType?.Equals("Family", StringComparison.OrdinalIgnoreCase) == true,
                    "AC" => x.PropertyType?.Contains("AC", StringComparison.OrdinalIgnoreCase) == true,
                    "Food" => x.PropertyType?.Contains("Food", StringComparison.OrdinalIgnoreCase) == true,
                    "WiFi" => x.PropertyType?.Contains("WiFi", StringComparison.OrdinalIgnoreCase) == true,
                    "Parking" => x.PropertyType?.Contains("Parking", StringComparison.OrdinalIgnoreCase) == true,
                    _ => true
                });
            }

            foreach (var item in finalList)
            {
                NearbyPGs.Add(item);
            }

            StatusMessage = $"{NearbyPGs.Count} PG(s) found matching criteria.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;

            if (allowLocationPrompt && Shell.Current?.CurrentPage != null)
            {
                await Shell.Current.DisplayAlert(
                    "Search Failed",
                    ex.Message,
                    "OK");
            }
        }
    }

    //[RelayCommand]
    //private async Task RadiusChangedAsync()
    //{
    //    if (_isSearching || IsBusy) return;
    //    if (!string.IsNullOrWhiteSpace(SearchText)) return; // radius is meaningless for a text search
    //    if (!UserLatitude.HasValue || !UserLongitude.HasValue) return; // no location yet — nothing to re-run

    //    _isSearching = true;
    //    IsBusy = true;
    //    try
    //    {
    //        // Force a fresh server fetch at the new radius rather than
    //        // re-filtering the old cached result set, since a wider radius
    //        // can include PGs the previous fetch never returned at all.
    //        _rawNearbyCache.Clear();
    //        _cacheIsFromTextSearch = null;
    //        await SearchNearbyInternalAsync(allowLocationPrompt: false);
    //    }
    //    finally
    //    {
    //        _isSearching = false;
    //        IsBusy = false;
    //    }
    //}

    // Bound to the "Locate" button. This — and only this, plus the
    // Search Properties button / category filters — is allowed to trigger
    // the OS location-permission prompt.
    [RelayCommand]
    private async Task LocateMeAsync()
    {
        if (_isSearching) return;
        _isSearching = true;

        bool ownsBusyFlag = !IsBusy;
        if (ownsBusyFlag) IsBusy = true;

        try
        {
            SearchText = string.Empty;
            _rawNearbyCache.Clear();
            _cacheIsFromTextSearch = null;
            await SearchNearbyInternalAsync(allowLocationPrompt: true);
        }
        finally
        {
            _isSearching = false;
            if (ownsBusyFlag) IsBusy = false;
        }
    }

    // Full home page load — Featured / Recommended / Recent / Nearby now run
    // truly in parallel (previously Nearby ran sequentially after the other
    // three, adding a full extra network round-trip to every load).
    [RelayCommand]
    private async Task LoadHomePageAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = "Loading...";

        try
        {
            FeaturedPGs.Clear();
            RecommendedPGs.Clear();
            RecentPGs.Clear();
            _rawNearbyCache.Clear();
            _cacheIsFromTextSearch = null;

            var featuredTask = _apiService.GetFeaturedPGsAsync();
            var recommendedTask = _apiService.GetRecommendedPGsAsync();
            var recentTask = _apiService.GetRecentlyViewedPGsAsync();
            // allowLocationPrompt: false — startup must never show the OS
            // location dialog. If permission was already granted in a
            // previous session, GetCurrentLocationAsync will still silently
            // use it and populate Nearby; otherwise Nearby just stays empty
            // until the user taps Locate.
            var nearbyTask = SearchNearbyInternalAsync(allowLocationPrompt: false);

            await Task.WhenAll(featuredTask, recommendedTask, recentTask, nearbyTask);

            if (featuredTask.Result != null)
                foreach (var item in featuredTask.Result) FeaturedPGs.Add(item);

            if (recommendedTask.Result != null)
                foreach (var item in recommendedTask.Result) RecommendedPGs.Add(item);

            if (recentTask.Result != null)
                foreach (var item in recentTask.Result) RecentPGs.Add(item);

            StatusMessage = string.Empty;
            _hasLoadedOnce = true;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            if (Shell.Current?.CurrentPage != null)
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Lightweight version used when returning to the page (back navigation).
    // Only refreshes "Recently Viewed" (which genuinely changes after a
    // detail visit) instead of re-fetching everything.
    [RelayCommand]
    private async Task RefreshRecentOnlyAsync()
    {
        if (IsBusy) return;
        try
        {
            var recent = await _apiService.GetRecentlyViewedPGsAsync();
            RecentPGs.Clear();
            if (recent != null)
                foreach (var item in recent) RecentPGs.Add(item);
        }
        catch { /* non-critical background refresh, ignore failures */ }
    }

    public bool HasLoadedOnce => _hasLoadedOnce;

    [RelayCommand]
    private async Task RefreshAsync()
    {
        // Guard against RefreshView re-invoking the command while a refresh
        // (or any other load) is already in flight.
        if (IsRefreshing || IsBusy) return;

        IsRefreshing = true;
        try
        {
            _hasLoadedOnce = false; // pull-to-refresh should force a real reload
            await LoadHomePageAsync();
        }
        finally
        {
            // Small delay before clearing IsRefreshing gives the native
            // Android SwipeRefreshLayout time to finish its own animation
            // cycle before we drop the flag — without this, clearing it
            // immediately after a very fast load can race with the native
            // control and cause it to re-arm instead of settling.
            await Task.Delay(250);
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task AppearingAsync() => await LoadHomePageAsync();

    // allowPrompt = false (page load): only uses location if permission was
    // already granted in a previous session — never shows the OS dialog,
    // never shows our own "Permission Required" alert.
    // allowPrompt = true (Locate / Search Properties / category tap): shows
    // the OS permission dialog if not yet decided, and our alert if denied.
    private async Task<Location?> GetCurrentLocationAsync(bool allowPrompt)
    {
        try
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (permission != PermissionStatus.Granted)
            {
                if (!allowPrompt)
                {
                    // Silent path: don't ask, just report "no location available".
                    return null;
                }

                permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (permission != PermissionStatus.Granted)
            {
                if (allowPrompt && Shell.Current?.CurrentPage != null)
                    await Shell.Current.DisplayAlert("Permission Required", "Location permission is required to find nearby listings.", "OK");
                return null;
            }

            try
            {
                var location = await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(8)));

                return location ?? await Geolocation.Default.GetLastKnownLocationAsync();
            }
            catch (FeatureNotEnabledException)
            {
                // Permission is granted but the device's GPS/location
                // toggle itself is off. Android will normally show its own
                // "Turn on device location" prompt here; this catch handles
                // the case where the user dismisses it instead of enabling it.
                if (allowPrompt && Shell.Current?.CurrentPage != null)
                    await Shell.Current.DisplayAlert("Location Off", "Please turn on location/GPS on your device to find nearby PGs.", "OK");
                return null;
            }
        }
        catch
        {
            return null;
        }
    }

    [RelayCommand]
    private async Task GoToPGDetailAsync(PGCardResponse pg)
    {
        // Guards against the same tap firing this command twice (e.g. a
        // Button's Command and its parent Border's TapGestureRecognizer both
        // reacting to one physical tap), which previously caused
        // ShowPopupAsync / GoToAsync to be entered re-entrantly and crash.
        if (pg == null || _isNavigatingToDetail) return;

        var currentPage = Shell.Current?.CurrentPage;
        if (currentPage == null) return; // navigation not settled yet — avoid crash on very first tap

        _isNavigatingToDetail = true;
        try
        {
            bool loggedIn = await _authService.IsLoggedInAsync();

            if (!loggedIn)
            {
                var authPopup = new AuthPromptPopup();
                var result = await currentPage.ShowPopupAsync(authPopup);

                if (result is string choice)
                {
                    if (choice == "Log In")
                        await Shell.Current.GoToAsync($"LoginPage?TargetRoute=PGTenantDetailPage&TargetId={pg.Id}");
                    else if (choice == "Register")
                        await Shell.Current.GoToAsync($"RegisterPage?TargetRoute=PGTenantDetailPage&TargetId={pg.Id}");
                }
                return;
            }

            try
            {
                await _apiService.AddRecentlyViewedAsync(pg.Id);
            }
            catch { /* Suppress telemetry errors silently */ }

            await Shell.Current.GoToAsync($"PGTenantDetailPage?id={pg.Id}");
        }
        finally
        {
            _isNavigatingToDetail = false;
        }
    }

    [RelayCommand]
    private async Task SelectCategoryAsync(CategoryChip chip)
    {
        if (chip == null || chip.IsSelected) return;

        foreach (var item in Categories)
            item.IsSelected = false;

        chip.IsSelected = true;
        await SearchNearbyAsync();
    }

    [RelayCommand]
    private async Task GoToLoginAsync() => await Shell.Current.GoToAsync("LoginPage");
}