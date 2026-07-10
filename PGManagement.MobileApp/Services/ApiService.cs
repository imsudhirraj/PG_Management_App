using PGManagement.MobileApp.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PGManagement.MobileApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorageService _secureStorage;

    private const string BaseUrl = "http://monalika-001-site1.ftempurl.com/api/";

    // Centralized options configuration to enforce number-handling across all generic and specific parsing streams
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
    };

    public ApiService(ISecureStorageService secureStorage)
    {
        _secureStorage = secureStorage;

    #if DEBUG
            // Dev-only: bypass self-signed cert validation for local HTTPS testing.
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(15)
            };
    #else
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };
    #endif
    }

    private async Task AttachTokenAsync()
    {
        var token = await _secureStorage.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    // =====================================================
    // CORE NETWORK METHODS (CRASH PROOFED WITH STRINGS)
    // =====================================================

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint)
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"{(int)response.StatusCode} {response.StatusCode}: {errorBody}");
        }

        // Fix ObjectDisposedException by loading stream into memory instantly
        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json)) return default;

        return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body)
    {
        await AttachTokenAsync();
        var response = await _httpClient.PostAsJsonAsync(endpoint, body);
        if (!response.IsSuccessStatusCode) return default;

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json)) return default;

        return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
    }

    public async Task<HttpResponseMessage> PostRawAsync<TRequest>(string endpoint, TRequest body)
    {
        await AttachTokenAsync();
        return await _httpClient.PostAsJsonAsync(endpoint, body);
    }

    public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest body)
    {
        await AttachTokenAsync();
        var response = await _httpClient.PutAsJsonAsync(endpoint, body);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        await AttachTokenAsync();
        var response = await _httpClient.DeleteAsync(endpoint);
        return response.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> PostEmptyBodyAsync(string endpoint)
    {
        await AttachTokenAsync();
        return await _httpClient.PostAsync(endpoint, null);
    }

    // =====================================================
    // KYC & PAYMENTS
    // =====================================================

    public async Task<bool> UploadKycAsync(int bookingId, string documentType, Stream fileStream, string fileName)
    {
        await AttachTokenAsync();
        using var form = new MultipartFormDataContent();

        form.Add(new StringContent(bookingId.ToString()), "BookingId");
        form.Add(new StringContent(documentType), "DocumentType");

        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(streamContent, "File", fileName);

        var response = await _httpClient.PostAsync("Kyc", form);
        return response.IsSuccessStatusCode;
    }

    public async Task<Stream?> DownloadKycAsync(int id)
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync($"Kyc/{id}/download");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<PaymentDetailsResponse?> GetPaymentDetailsAsync(int bookingId)
    {
        await AttachTokenAsync();
        var endpoint = $"Payment/details/{bookingId}";
        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Status: {(int)response.StatusCode}\nEndpoint: {endpoint}\n\n{error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaymentDetailsResponse>(json, _jsonOptions);
    }

    public async Task<bool> UploadPaymentAsync(int bookingId, decimal amount, string transactionId, string paymentMethod, Stream screenshot, string fileName)
    {
        await AttachTokenAsync();
        using var form = new MultipartFormDataContent();

        form.Add(new StringContent(bookingId.ToString()), "BookingId");
        form.Add(new StringContent(amount.ToString()), "Amount");
        form.Add(new StringContent("Advance"), "Type");
        form.Add(new StringContent(transactionId), "TransactionId");
        form.Add(new StringContent(paymentMethod), "PaymentMethod");

        var fileContent = new StreamContent(screenshot);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "Screenshot", fileName);

        var response = await _httpClient.PostAsync("Payment", form);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<PaymentOverviewResponse>?> GetOwnerPaymentsAsync()
    {
        await AttachTokenAsync();
        var endpoint = "Payment/by-owner";
        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"GET {endpoint}\n\nStatus : {(int)response.StatusCode}\n\n{error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PaymentOverviewResponse>>(json, _jsonOptions);
    }

    public async Task<bool> VerifyPaymentAsync(int paymentId, VerifyPaymentRequest request)
    {
        await AttachTokenAsync();
        var endpoint = $"Payment/{paymentId}/verify";
        var response = await _httpClient.PutAsJsonAsync(endpoint, request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert("API Error", $"PUT {endpoint}\n\nStatus : {(int)response.StatusCode}\n\n{error}", "OK");
            });
            return false;
        }
        return true;
    }

    // =====================================================
    // REVIEW APIS
    // =====================================================

    public async Task<bool> CreateReviewAsync(CreateReviewRequest request)
    {
        await AttachTokenAsync();
        var response = await _httpClient.PostAsJsonAsync("Review", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ReviewResponse>?> GetReviewsAsync(int pgId)
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync($"Review/pg/{pgId}");
        if (!response.IsSuccessStatusCode) return new List<ReviewResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ReviewResponse>>(json, _jsonOptions);
    }

    public async Task<bool> ReplyReviewAsync(int reviewId, OwnerReplyRequest request)
    {
        await AttachTokenAsync();
        var response = await _httpClient.PutAsJsonAsync($"Review/{reviewId}/reply", request);
        return response.IsSuccessStatusCode;
    }

    // =====================================================
    // BACKEND CHANNELS: EXPLICIT ROUTE SYNCHRONIZATION
    // =====================================================

    public async Task<bool> AddFavouriteAsync(int pgId)
    {
        await AttachTokenAsync();
        var response = await _httpClient.PostAsync($"Favourite/{pgId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveFavouriteAsync(int pgId)
    {
        await AttachTokenAsync();
        var response = await _httpClient.DeleteAsync($"Favourite/{pgId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<FavouriteResponse>?> GetMyFavouritesAsync()
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync("Favourite");
        if (!response.IsSuccessStatusCode) return new List<FavouriteResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<FavouriteResponse>>(json, _jsonOptions);
    }

    // =====================================================
    // RECENTLY VIEWED & RECOMMENDATIONS
    // =====================================================

    public async Task<bool> AddRecentlyViewedAsync(int pgId)
    {
        await AttachTokenAsync();
        var response = await _httpClient.PostAsync($"RecentlyViewed/{pgId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<PGCardResponse>?> GetNearbyPGsAsync(decimal latitude, decimal longitude, double radiusKm = 5)
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync($"Recommendation/nearby?latitude={latitude}&longitude={longitude}&radiusKm={radiusKm}");
        if (!response.IsSuccessStatusCode) return new List<PGCardResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PGCardResponse>>(json, _jsonOptions);
    }

    public async Task<List<RecentlyViewedResponse>?> GetRecentlyViewedAsync()
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync("RecentlyViewed");
        if (!response.IsSuccessStatusCode) return new List<RecentlyViewedResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<RecentlyViewedResponse>>(json, _jsonOptions);
    }

    public async Task<List<PGCardResponse>?> GetFeaturedPGsAsync()
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync("Recommendation/featured");
        if (!response.IsSuccessStatusCode) return new List<PGCardResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PGCardResponse>>(json, _jsonOptions);
    }

    public async Task<List<PGCardResponse>?> GetRecommendedPGsAsync()
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync("Recommendation/recommended");
        if (!response.IsSuccessStatusCode) return new List<PGCardResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PGCardResponse>>(json, _jsonOptions);
    }

    public async Task<List<PGCardResponse>?> GetRecentPGsAsync()
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync("Recommendation/recent");
        if (!response.IsSuccessStatusCode) return new List<PGCardResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PGCardResponse>>(json, _jsonOptions);
    }

    // =====================================================
    // FRONT-END UI BRIDGE INTERFACE METHODS
    // =====================================================

    public async Task<IEnumerable<PGCardResponse>?> GetAllPropertiesAsync()
    {
        return await GetRecentPGsAsync();
    }

    public async Task<IEnumerable<PGCardResponse>?> GetWishlistPropertiesAsync()
    {
        var favourites = await GetMyFavouritesAsync();
        if (favourites == null) return new List<PGCardResponse>();

        return favourites.Select(f => new PGCardResponse
        {
            Id = f.PGId,
            Name = f.PGName,
            Address = f.Address,
            City = f.Address.Contains(",")
                ? f.Address.Split(',').Last().Trim()
                : f.Address,
            StartingRent = f.StartingRent,
            CoverImageUrl = f.CoverImageUrl ?? "placeholder_house.png"
        }).ToList();
    }

    public async Task<bool> RemoveFromWishlistAsync(int propertyId)
    {
        return await RemoveFavouriteAsync(propertyId);
    }

    public async Task<List<PGCardResponse>?> SearchPGsByNameAsync(string query)
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync($"PG/search-by-text?query={Uri.EscapeDataString(query)}");
        if (!response.IsSuccessStatusCode) return new List<PGCardResponse>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PGCardResponse>>(json, _jsonOptions);
    }
    public async Task<List<PGCardResponse>?> GetRecentlyViewedPGsAsync()
    {
        var recent = await GetRecentlyViewedAsync();
        if (recent == null) return new List<PGCardResponse>();

        return recent.Select(r => new PGCardResponse
        {
            Id = r.PGId,
            Name = r.PGName,
            Address = r.Address,
            City = r.Address.Contains(",") ? r.Address.Split(',').Last().Trim() : r.Address,
            StartingRent = r.StartingRent,
            Rating = r.Rating,
            ReviewCount = r.ReviewCount,
            CoverImageUrl = r.CoverImageUrl ?? "placeholder_house.png",
            ViewedAt = r.ViewedAt
        }).ToList();
    }
}