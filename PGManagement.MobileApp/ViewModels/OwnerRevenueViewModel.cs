using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class OwnerRevenueViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    public ObservableCollection<PaymentOverviewResponse> Payments { get; }
        = new();

    public OwnerRevenueViewModel(
        IApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    string statusMessage = "";

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            StatusMessage = "Loading payments...";

            Payments.Clear();

            var list =
                await _apiService.GetOwnerPaymentsAsync();

            if (list == null)
            {
                StatusMessage = "Unable to load payments.";
                return;
            }

            foreach (var item in list)
            {
                Payments.Add(item);
            }

            if (Payments.Count == 0)
                StatusMessage = "No payment requests found.";
            else
                StatusMessage = $"{Payments.Count} payment(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ViewScreenshotAsync(PaymentOverviewResponse payment)
    {
        if (payment == null)
            return;

        if (string.IsNullOrWhiteSpace(payment.ScreenshotUrl))
        {
            await Shell.Current.DisplayAlert(
                "Screenshot",
                "Screenshot not available.",
                "OK");

            return;
        }

        try
        {
            await Launcher.Default.OpenAsync(payment.ScreenshotUrl);
        }
        catch
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Unable to open screenshot.",
                "OK");
        }
    }

    [RelayCommand]
    async Task ApproveAsync(PaymentOverviewResponse payment)
    {
        if (payment == null)
            return;

        bool result =
            await Shell.Current.DisplayAlert(
                "Approve Payment",
                $"Approve ₹{payment.Amount} payment from {payment.TenantName}?",
                "Approve",
                "Cancel");

        if (!result)
            return;

        IsBusy = true;

        try
        {
            var success =
                await _apiService.VerifyPaymentAsync(
                    payment.Id,
                    new VerifyPaymentRequest
                    {
                        Status = "Approved"
                    });

            if (success)
            {
                await Shell.Current.DisplayAlert(
                    "Success",
                    "Payment approved successfully.",
                    "OK");

                await LoadAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Failed",
                    "Unable to approve payment.",
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task RejectAsync(PaymentOverviewResponse payment)
    {
        if (payment == null)
            return;

        string remarks =
            await Shell.Current.DisplayPromptAsync(
                "Reject Payment",
                "Enter rejection reason");

        if (string.IsNullOrWhiteSpace(remarks))
            return;

        IsBusy = true;

        try
        {
            var success =
                await _apiService.VerifyPaymentAsync(
                    payment.Id,
                    new VerifyPaymentRequest
                    {
                        Status = "Rejected",
                        Remarks = remarks
                    });

            if (success)
            {
                await Shell.Current.DisplayAlert(
                    "Success",
                    "Payment rejected.",
                    "OK");

                await LoadAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Failed",
                    "Unable to reject payment.",
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}