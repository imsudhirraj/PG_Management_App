using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using PGManagement.MobileApp.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Views
{
    public partial class PGSearchPage : ContentPage
    {
        private bool _userInitiatedSearch = false;

        public PGSearchPage(PGSearchViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            viewModel.PropertyChanged += OnViewModelPropertyChanged;

            SearchPropertiesButton.Clicked += OnSearchPropertiesButtonClicked;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is not PGSearchViewModel vm || vm.IsBusy)
                return;

            // First time the page appears (or after a manual pull-to-refresh
            // reset HasLoadedOnce), do the full load. On every subsequent
            // appearance (e.g. coming back from the PG detail page), only
            // refresh "Recently Viewed" instead of re-fetching everything —
            // this removes a full redundant network round-trip on every
            // back-navigation, which was the main source of perceived
            // slowness after the first click.
            if (!vm.HasLoadedOnce)
            {
                vm.LoadHomePageCommand.Execute(null);
            }
            else
            {
                vm.RefreshRecentOnlyCommand.Execute(null);
            }
        }

        private void OnSearchPropertiesButtonClicked(object sender, EventArgs e)
        {
            _userInitiatedSearch = true;
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(PGSearchViewModel.IsBusy))
                return;

            if (BindingContext is not PGSearchViewModel vm || vm.IsBusy || !_userInitiatedSearch)
                return;

            // Reset immediately so a rapid second property-change (or a tap
            // that navigates away) can't re-enter this block.
            _userInitiatedSearch = false;

            try
            {
                // Wrapped in try/catch because this is an async void event
                // handler — any unhandled exception here previously crashed
                // the whole app rather than just this operation (e.g. if the
                // user tapped a PG card and navigated away while this was
                // still pending, ScrollToAsync could throw on a
                // torn-down/re-laid-out view).
                await Task.Delay(150);

                // Bail out if the page is no longer the active page (user
                // already navigated away) or the views are gone.
                if (Handler == null || MainScrollView?.Handler == null || NearbySection?.Handler == null)
                    return;

                await MainScrollView.ScrollToAsync(NearbySection, ScrollToPosition.Start, true);
            }
            catch
            {
                // Swallow — a failed cosmetic scroll should never crash the app.
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is PGSearchViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
            }
            SearchPropertiesButton.Clicked -= OnSearchPropertiesButtonClicked;
        }
    }
}