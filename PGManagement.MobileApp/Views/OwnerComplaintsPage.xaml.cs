namespace PGManagement.MobileApp.Views;

public partial class OwnerComplaintsPage : ContentPage
{
    private readonly ViewModels.OwnerComplaintsViewModel _vm;
    public OwnerComplaintsPage(ViewModels.OwnerComplaintsViewModel vm)
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