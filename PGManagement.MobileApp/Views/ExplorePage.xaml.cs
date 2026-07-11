using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class ExplorePage : ContentPage
{
    private readonly ExploreViewModel _viewModel;

    public ExplorePage(ExploreViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Trigger your internal API synchronization routines smoothly on page load
        if (_viewModel.LoadAllPropertiesAsyncCommand.CanExecute(null))
        {
            await _viewModel.LoadAllPropertiesAsyncCommand.ExecuteAsync(null);
        }
    }
}