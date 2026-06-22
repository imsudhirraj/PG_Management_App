namespace PGManagement.MobileApp.Views;

public partial class OwnerBookingsPage : ContentPage
{
    private readonly ViewModels.OwnerBookingsViewModel _vm;
    public OwnerBookingsPage(ViewModels.OwnerBookingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}