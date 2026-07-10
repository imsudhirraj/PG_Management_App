using CommunityToolkit.Maui.Views;

namespace PGManagement.MobileApp.Views;

public partial class AuthPromptPopup : Popup
{
    public AuthPromptPopup()
    {
        InitializeComponent();
    }

    private void OnLoginClicked(object sender, EventArgs e) => Close("Log In");

    private void OnRegisterClicked(object sender, EventArgs e) => Close("Register");

    private void OnCancelClicked(object sender, EventArgs e) => Close("Cancel");
}