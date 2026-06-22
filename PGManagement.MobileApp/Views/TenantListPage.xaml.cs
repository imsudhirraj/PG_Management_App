namespace PGManagement.MobileApp.Views;

public partial class TenantListPage : ContentPage
{
    private readonly ViewModels.TenantListViewModel _vm;
    public TenantListPage(ViewModels.TenantListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadTenantsCommand.ExecuteAsync(null);
    }
}