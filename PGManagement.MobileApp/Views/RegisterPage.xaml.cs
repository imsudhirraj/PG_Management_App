namespace PGManagement.MobileApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(ViewModels.RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}