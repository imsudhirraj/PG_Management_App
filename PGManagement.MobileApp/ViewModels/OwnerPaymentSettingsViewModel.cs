using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PGManagement.MobileApp.Models;
using PGManagement.MobileApp.Services;

namespace PGManagement.MobileApp.ViewModels;

public partial class OwnerPaymentSettingsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    public OwnerPaymentSettingsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    private string accountHolderName = "";

    [ObservableProperty]
    private string upiId = "";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "";

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;

            var data =
                await _apiService.GetAsync<OwnerPaymentSettingsResponse>(
                    "OwnerPaymentSettings");

            if (data != null)
            {
                AccountHolderName = data.AccountHolderName;
                UpiId = data.UpiId;
            }
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
    public async Task SaveAsync()
    {
        try
        {
            IsBusy = true;

            var success =
                await _apiService.PostAsync<
                    OwnerPaymentSettingsRequest,
                    object>(
                    "OwnerPaymentSettings",
                    new OwnerPaymentSettingsRequest
                    {
                        AccountHolderName = AccountHolderName,
                        UpiId = UpiId
                    });

            await Shell.Current.DisplayAlert(
                "Success",
                "Payment settings saved successfully.",
                "OK");
        }
        catch (Exception ex)
        {
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