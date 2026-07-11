using CommunityToolkit.Maui.Views;

namespace PGManagement.MobileApp.Views;

public partial class LocationPromptPopup : Popup
{
    public LocationPromptPopup()
    {
        InitializeComponent();
    }

    private void OnAllowClicked(object sender, EventArgs e)
    {
        // Close passing true as confirmation payload 
        Close(true);
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Close passing false to cancel interaction fallback
        Close(false);
    }
}