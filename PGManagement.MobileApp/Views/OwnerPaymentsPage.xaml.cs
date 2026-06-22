namespace PGManagement.MobileApp.Views;

public partial class OwnerPaymentsPage : ContentPage
{
    private readonly ViewModels.OwnerPaymentsViewModel _vm;
    public OwnerPaymentsPage(ViewModels.OwnerPaymentsViewModel vm)
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