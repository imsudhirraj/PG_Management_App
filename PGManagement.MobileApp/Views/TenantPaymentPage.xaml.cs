using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class TenantPaymentPage : ContentPage
{
    private readonly TenantPaymentViewModel _viewModel;

    public TenantPaymentPage(TenantPaymentViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
        _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Shell.Current.DisplayAlert(
            "DEBUG",
            $"BookingId = {_viewModel.BookingId}",
            "OK");

        if (_viewModel.BookingId > 0)
        {
            await _viewModel.LoadAsync();
        }
    }
}