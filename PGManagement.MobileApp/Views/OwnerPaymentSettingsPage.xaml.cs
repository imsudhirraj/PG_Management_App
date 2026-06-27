using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class OwnerPaymentSettingsPage : ContentPage
{
    public OwnerPaymentSettingsPage(
        OwnerPaymentSettingsViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is OwnerPaymentSettingsViewModel vm)
            await vm.LoadAsync();
    }
}