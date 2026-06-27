using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;
using System.Collections.ObjectModel;

namespace PGManagement.MobileApp.ViewModels;

public partial class OwnerKycViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public ObservableCollection<KycDocumentResponse> Documents { get; } = new();

    public OwnerKycViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;

        StatusMessage = string.Empty;

        Documents.Clear();

        try
        {
            var docs =
                await _apiService.GetAsync<List<KycDocumentResponse>>(
                    "Kyc/by-owner");

            if (docs == null || docs.Count == 0)
            {
                StatusMessage = "No KYC requests available.";
                return;
            }

            foreach (var item in docs)
                Documents.Add(item);
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
    private async Task OpenDocumentAsync(KycDocumentResponse document)
    {
        try
        {
            var stream = await _apiService.DownloadKycAsync(document.Id);

            if (stream == null)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "Unable to download document.",
                    "OK");

                return;
            }

            var extension = Path.GetExtension(document.FileUrl);

            if (string.IsNullOrWhiteSpace(extension))
                extension = ".pdf";

            var filePath = Path.Combine(
                FileSystem.CacheDirectory,
                $"Kyc_{document.Id}{extension}");

            using (var file = File.Create(filePath))
            {
                await stream.CopyToAsync(file);
            }

            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
    }

    [RelayCommand]
    private async Task ApproveAsync(KycDocumentResponse document)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Approve KYC",
            $"Approve {document.FullName}'s KYC?",
            "Approve",
            "Cancel");

        if (!confirm)
            return;

        var success = await _apiService.PutAsync(
            $"Kyc/{document.Id}/verify",
            new VerifyKycRequest
            {
                Status = "Approved"
            });

        if (success)
        {
            await Shell.Current.DisplayAlert(
                "Success",
                "KYC Approved successfully.",
                "OK");

            await LoadAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Unable to approve KYC.",
                "OK");
        }
    }

    [RelayCommand]
    private async Task RejectAsync(KycDocumentResponse document)
    {
        string? reason = await Shell.Current.DisplayPromptAsync(
            "Reject KYC",
            "Enter rejection reason");

        if (string.IsNullOrWhiteSpace(reason))
            return;

        var success = await _apiService.PutAsync(
            $"Kyc/{document.Id}/verify",
            new VerifyKycRequest
            {
                Status = "Rejected",
                Remarks = reason
            });

        if (success)
        {
            await Shell.Current.DisplayAlert(
                "Success",
                "KYC Rejected.",
                "OK");

            await LoadAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Unable to reject KYC.",
                "OK");
        }
    }

}