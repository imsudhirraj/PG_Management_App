namespace PGManagement.MobileApp.Views;

public partial class OwnerNoticesPage : ContentPage
{
    private readonly ViewModels.OwnerNoticesViewModel _vm;
    public OwnerNoticesPage(ViewModels.OwnerNoticesViewModel vm)
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