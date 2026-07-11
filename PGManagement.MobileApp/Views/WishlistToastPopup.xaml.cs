using CommunityToolkit.Maui.Views;

namespace PGManagement.MobileApp.Views;

public partial class WishlistToastPopup : Popup
{
    public WishlistToastPopup(bool isAdded)
    {
        InitializeComponent();
        ToastMessage.Text = isAdded ? "Shortlisted to your Wishlist!" : "Removed from Wishlist";

        // Auto-dismiss handler
        Dispatcher.StartTimer(TimeSpan.FromSeconds(1.8), () =>
        {
            Close();
            return false;
        });
    }
}