namespace PGManagement.MobileApp.Views;

public partial class AllocationsListPage : ContentPage
{
    private readonly ViewModels.AllocationsListViewModel _vm;
    public AllocationsListPage(ViewModels.AllocationsListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadMyPGsCommand.ExecuteAsync(null);
    }
}