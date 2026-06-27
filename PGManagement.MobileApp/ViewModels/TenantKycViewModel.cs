using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

[QueryProperty(nameof(BookingId), "bookingId")]
public partial class TenantKycViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    public TenantKycViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }
    private FileResult? _selectedFile;

    [ObservableProperty]
    private int bookingId;

    [ObservableProperty]
    private string selectedDocumentType = "Aadhaar";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private string selectedFileName = "No file selected";

    [RelayCommand]
    private async Task PickDocumentAsync()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select KYC Document",
            FileTypes = FilePickerFileType.Pdf
        });

        if (result == null)
            return;

        _selectedFile = result;

        SelectedFileName = result.FileName;
        StatusMessage = "Document selected.";
    }

    [RelayCommand]
    private async Task UploadKycAsync()
    {
        if (IsBusy)
            return;

        if (_selectedFile == null)
        {
            await Shell.Current.DisplayAlert(
                "KYC",
                "Please choose a document first.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var stream = await _selectedFile.OpenReadAsync();

            var success = await _apiService.UploadKycAsync(
                BookingId,
                SelectedDocumentType,
                stream,
                _selectedFile.FileName);

            if (success)
            {
                StatusMessage = "KYC uploaded successfully.";

                await Shell.Current.DisplayAlert(
                    "Success",
                    "Your KYC has been uploaded successfully.",
                    "OK");

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusMessage = "Upload failed.";

                await Shell.Current.DisplayAlert(
                    "Error",
                    "Failed to upload KYC.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;

            await Shell.Current.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }


}