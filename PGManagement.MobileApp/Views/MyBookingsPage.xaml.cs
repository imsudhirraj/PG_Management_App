namespace PGManagement.MobileApp.Views;

public partial class MyBookingsPage : ContentPage
{
    private readonly ViewModels.MyBookingsViewModel _vm;

    public MyBookingsPage(ViewModels.MyBookingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadBookingsCommand.ExecuteAsync(null);
    }
}