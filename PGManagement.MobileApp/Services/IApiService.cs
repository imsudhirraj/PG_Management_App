using PGManagement.MobileApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
    Task<bool> UploadKycAsync(int bookingId, string documentType, Stream fileStream, string fileName);
    Task<Stream?> DownloadKycAsync(int id);
    Task<PaymentDetailsResponse?> GetPaymentDetailsAsync(int bookingId);

    Task<bool> UploadPaymentAsync(int bookingId, decimal amount, string transactionId, string paymentMethod, Stream screenshot, string fileName);
    Task<List<PaymentOverviewResponse>?> GetOwnerPaymentsAsync();
    Task<bool> VerifyPaymentAsync(int paymentId, VerifyPaymentRequest request);

    #region Review APIs
    Task<bool> CreateReviewAsync(CreateReviewRequest request);
    Task<List<ReviewResponse>?> GetReviewsAsync(int pgId);
    Task<bool> ReplyReviewAsync(int reviewId, OwnerReplyRequest request);
    #endregion

    #region Favourite APIs
    Task<bool> AddFavouriteAsync(int pgId);
    Task<bool> RemoveFavouriteAsync(int pgId);
    Task<List<FavouriteResponse>?> GetMyFavouritesAsync();
    #endregion

    #region Recently Viewed APIs
    Task<bool> AddRecentlyViewedAsync(int pgId);
    Task<List<RecentlyViewedResponse>?> GetRecentlyViewedAsync();
    #endregion

    #region Recommendation APIs
    Task<List<PGCardResponse>?> GetFeaturedPGsAsync();
    Task<List<PGCardResponse>?> GetNearbyPGsAsync(decimal latitude, decimal longitude, double radiusKm = 5);
    Task<List<PGCardResponse>?> GetRecommendedPGsAsync();
    Task<List<PGCardResponse>?> GetRecentPGsAsync();
    #endregion

    #region Front-End Bridge Methods
    // These maps the exact methods the Explore and Wishlist ViewModels are looking for
    Task<IEnumerable<PGCardResponse>?> GetAllPropertiesAsync();
    Task<IEnumerable<PGCardResponse>?> GetWishlistPropertiesAsync();
    Task<bool> RemoveFromWishlistAsync(int propertyId);
    Task<List<PGCardResponse>?> SearchPGsByNameAsync(string query);
    Task<List<PGCardResponse>?> GetRecentlyViewedPGsAsync();
    #endregion
}