using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views;

public partial class WishlistPage : ContentPage
{
    private readonly WishlistViewModel _viewModel;

    public WishlistPage(WishlistViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                if (_viewModel != null)
                {
                    await _viewModel.LoadWishlistAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WISHLIST PAGE VIEW LIFECYCLE CRASH]: {ex.Message}");
            }
        });
    }
}