using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class PGSearchPage : ContentPage
{
    public PGSearchPage(PGSearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}