using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class OwnerKycPage : ContentPage
{
    private readonly OwnerKycViewModel _vm;

    public OwnerKycPage(OwnerKycViewModel vm)
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