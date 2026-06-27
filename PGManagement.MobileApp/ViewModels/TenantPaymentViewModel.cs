using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using QRCoder;

namespace PGManagement.MobileApp.ViewModels;

[QueryProperty(nameof(BookingId), "bookingId")]
public partial class TenantPaymentViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    private FileResult? _selectedScreenshot;

    public TenantPaymentViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    private int bookingId;

    [ObservableProperty]
    private string pgName = "";

    [ObservableProperty]
    private string roomNumber = "";

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private string accountHolderName = "";

    [ObservableProperty]
    private string upiId = "";

    [ObservableProperty]
    private string transactionId = "";

    [ObservableProperty]
    private string selectedFileName = "No screenshot selected";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private ImageSource? qrImage;

    [ObservableProperty]
    private string statusMessage = "";

    public async Task LoadAsync()
    {
        try
        {
            StatusMessage = $"Loading Booking {BookingId}";

            var payment =
                await _apiService.GetPaymentDetailsAsync(BookingId);

            if (payment == null)
            {
                StatusMessage = "API returned NULL";
                return;
            }

            StatusMessage = "Payment details received.";

            PgName = payment.PGName;
            RoomNumber = payment.RoomNumber;
            Amount = payment.Amount;
            AccountHolderName = payment.AccountHolderName;
            UpiId = payment.UpiId;

            StatusMessage =
                $"UPI Loaded : {UpiId}";

            GenerateQr();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.ToString();

            await Shell.Current.DisplayAlert(
                "ERROR",
                ex.ToString(),
                "OK");
        }
    }

    partial void OnBookingIdChanged(int value)
    {
        StatusMessage = $"BookingId received: {value}";

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await LoadAsync();
        });
    }

    [RelayCommand]
    private async Task PickScreenshotAsync()
    {
        var file = await FilePicker.Default.PickAsync();

        if (file == null)
            return;

        _selectedScreenshot = file;
        SelectedFileName = file.FileName;
    }

    [RelayCommand]
    private async Task SubmitPaymentAsync()
    {
        if (_selectedScreenshot == null)
        {
            await Shell.Current.DisplayAlert("Payment", "Please select payment screenshot.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(TransactionId))
        {
            await Shell.Current.DisplayAlert("Payment", "Enter transaction id.", "OK");
            return;
        }

        IsBusy = true;

        using var stream = await _selectedScreenshot.OpenReadAsync();

        var success = await _apiService.UploadPaymentAsync(
            BookingId,
            Amount,
            TransactionId,
            "UPI",
            stream,
            _selectedScreenshot.FileName);

        IsBusy = false;

        if (success)
        {
            await Shell.Current.DisplayAlert("Success", "Payment submitted successfully.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Payment upload failed.", "OK");
        }
    }

    private void GenerateQr()
    {
        var upiUrl =
            $"upi://pay?pa={UpiId}" +
            $"&pn={Uri.EscapeDataString(AccountHolderName)}" +
            $"&am={Amount:0.00}" +
            $"&cu=INR";

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(upiUrl, QRCodeGenerator.ECCLevel.Q);

        var qr = new PngByteQRCode(data);

        byte[] bytes = qr.GetGraphic(20);

        QrImage = ImageSource.FromStream(() => new MemoryStream(bytes));
    }
    [RelayCommand]
    private async Task OpenUpiAsync()
    {
        var upiUrl =
            $"upi://pay?" +
            $"pa={UpiId}" +
            $"&pn={Uri.EscapeDataString(AccountHolderName)}" +
            $"&am={Amount:0.00}" +
            $"&tn=Advance Payment" +
            $"&cu=INR";

        try
        {
            await Launcher.Default.OpenAsync(upiUrl);
        }
        catch
        {
            await Shell.Current.DisplayAlert(
                "UPI",
                "No UPI app found.",
                "OK");
        }
    }
}