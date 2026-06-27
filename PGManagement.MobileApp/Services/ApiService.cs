using PGManagement.MobileApp.Models;
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

    //#if DEBUG
    //private const string BaseUrl = "http://10.0.2.2:5296/api/";
    //#else
    //private const string BaseUrl = "http://monalika-001-site1.ftempurl.com/api/";
    //#endif

    private const string BaseUrl = "http://monalika-001-site1.ftempurl.com/api/";

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

    public async Task<HttpResponseMessage> PostEmptyBodyAsync(string endpoint)
    {
        await AttachTokenAsync();
        // Sends a POST request with an empty string content to match the backend expectation
        return await _httpClient.PostAsync(endpoint, null);
    }

    public async Task<bool> UploadKycAsync(
    int bookingId,
    string documentType,
    Stream fileStream,
    string fileName)
    {
        await AttachTokenAsync();

        using var form = new MultipartFormDataContent();

        form.Add(
            new StringContent(bookingId.ToString()),
            "BookingId");

        form.Add(
            new StringContent(documentType),
            "DocumentType");

        var streamContent = new StreamContent(fileStream);

        streamContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/octet-stream");

        form.Add(
            streamContent,
            "File",
            fileName);

        var response = await _httpClient.PostAsync(
            "Kyc",
            form);

        return response.IsSuccessStatusCode;
    }

    public async Task<Stream?> DownloadKycAsync(int id)
    {
        await AttachTokenAsync();

        var response = await _httpClient.GetAsync(
            $"Kyc/{id}/download");

        if (!response.IsSuccessStatusCode)
            return null;

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

            throw new Exception(
                $"Status: {(int)response.StatusCode}\n" +
                $"Endpoint: {endpoint}\n\n" +
                error);
        }

        return await response.Content.ReadFromJsonAsync<PaymentDetailsResponse>();
    }
    public async Task<bool> UploadPaymentAsync(
    int bookingId,
    decimal amount,
    string transactionId,
    string paymentMethod,
    Stream screenshot,
    string fileName)
    {
        await AttachTokenAsync();

        using var form = new MultipartFormDataContent();

        form.Add(
            new StringContent(bookingId.ToString()),
            "BookingId");

        form.Add(
            new StringContent(amount.ToString()),
            "Amount");

        form.Add(
            new StringContent("Advance"),
            "Type");

        form.Add(
            new StringContent(transactionId),
            "TransactionId");

        form.Add(
            new StringContent(paymentMethod),
            "PaymentMethod");

        var fileContent = new StreamContent(screenshot);

        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        form.Add(
            fileContent,
            "Screenshot",
            fileName);

        var response = await _httpClient.PostAsync(
            "Payment",
            form);

        return response.IsSuccessStatusCode;
    }
}