namespace PGManagement.MobileApp.Views;

public partial class DashboardPage : ContentPage
{
    private readonly ViewModels.DashboardViewModel _vm;
    public DashboardPage(ViewModels.DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadStatsCommand.ExecuteAsync(null);
    }
}