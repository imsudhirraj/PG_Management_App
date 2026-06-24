using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PGManagement.MobileApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorageService _secureStorage;

    // IMPORTANT: change this to your machine's actual IP when testing on a
    // physical device or the Android emulator's host alias.
    // Android emulator → use 10.0.2.2 to reach your dev machine's localhost.
    // iOS simulator → localhost works directly.
    // Physical device → use your PC's LAN IP (e.g. 192.168.1.50).
    //private const string BaseUrl = "https://10.0.2.2:7259/api/";
    //private const string BaseUrl = "http://10.0.2.2:5296/api/";
    //private const string BaseUrl = "https://monalika-001-site1.ftempurl.com/api/";
   
    #if DEBUG
    private const string BaseUrl = "http://10.0.2.2:5296/api/";
    #else
        private const string BaseUrl = "http://monalika-001-site1.ftempurl.com/api/";
    #endif

    public ApiService(ISecureStorageService secureStorage)
    {
        _secureStorage = secureStorage;

#if DEBUG
        // Dev-only: bypass self-signed cert validation for local HTTPS testing.
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
        };
        _httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
#else
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
#endif
    }

    private async Task AttachTokenAsync()
    {
        var token = await _secureStorage.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    //public async Task<TResponse?> GetAsync<TResponse>(string endpoint)
    //{
    //    await AttachTokenAsync();
    //    var response = await _httpClient.GetAsync(endpoint);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        var errorBody = await response.Content.ReadAsStringAsync();
    //        throw new Exception($"{(int)response.StatusCode} {response.StatusCode}: {errorBody}");
    //    }

    //    return await response.Content.ReadFromJsonAsync<TResponse>();
    //}

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint)
    {
        await AttachTokenAsync();
        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"{(int)response.StatusCode} {response.StatusCode}: {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body)
    {
        await AttachTokenAsync();
        var response = await _httpClient.PostAsJsonAsync(endpoint, body);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<TResponse>();
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
}