using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class OwnerRevenuePage : ContentPage
{
    private readonly OwnerRevenueViewModel _viewModel;

    public OwnerRevenuePage(
        OwnerRevenueViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}