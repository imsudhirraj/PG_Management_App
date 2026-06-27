using PGManagement.MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Services;

public interface IApiService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body);
    Task<HttpResponseMessage> PostRawAsync<TRequest>(string endpoint, TRequest body);
    Task<bool> PutAsync<TRequest>(string endpoint, TRequest body);
    Task<bool> DeleteAsync(string endpoint);
    Task<HttpResponseMessage> PostEmptyBodyAsync(string endpoint);
    Task<bool> UploadKycAsync(
    int bookingId,
    string documentType,
    Stream fileStream,
    string fileName);
    Task<Stream?> DownloadKycAsync(int id);
    Task<PaymentDetailsResponse?> GetPaymentDetailsAsync(int bookingId);

    Task<bool> UploadPaymentAsync(
        int bookingId,
        decimal amount,
        string transactionId,
        string paymentMethod,
        Stream screenshot,
        string fileName);
    Task<List<PaymentOverviewResponse>?> GetOwnerPaymentsAsync();

    Task<bool> VerifyPaymentAsync(
        int paymentId,
        VerifyPaymentRequest request);
}
