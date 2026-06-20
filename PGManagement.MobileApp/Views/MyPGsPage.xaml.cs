namespace PGManagement.MobileApp.Views;

public partial class MyPGsPage : ContentPage
{
    private readonly ViewModels.MyPGsViewModel _vm;

    public MyPGsPage(ViewModels.MyPGsViewModel vm)
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