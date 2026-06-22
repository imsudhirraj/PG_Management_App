namespace PGManagement.MobileApp.Views;

public partial class PGSearchPage : ContentPage
{
    public PGSearchPage(ViewModels.PGSearchViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}