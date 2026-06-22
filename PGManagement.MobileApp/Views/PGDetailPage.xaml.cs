namespace PGManagement.MobileApp.Views;

public partial class PGDetailPage : ContentPage
{
    public PGDetailPage(ViewModels.PGDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}