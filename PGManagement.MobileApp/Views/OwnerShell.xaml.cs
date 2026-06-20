namespace PGManagement.MobileApp.Views;

public partial class OwnerShell : Shell
{
    public OwnerShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("AddPGPage", typeof(Views.AddPGPage));
    }
}