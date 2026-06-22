namespace PGManagement.MobileApp.Views;

public partial class AddPGPage : ContentPage
{
    public AddPGPage(ViewModels.AddPGViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}