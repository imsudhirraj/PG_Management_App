using System;
using Microsoft.Maui.Controls;
using PGManagement.MobileApp.ViewModels;

namespace PGManagement.MobileApp.Views
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        private async void OnOpeningAnimationFinished(object sender, EventArgs e)
        {
            // 1. Pull the registered LoginPage completely configured with its DI setup
            var loginPage = Handler?.MauiContext?.Services.GetService<LoginPage>();

            if (loginPage != null)
            {
                Application.Current!.MainPage = new NavigationPage(loginPage);
            }
            else
            {
                // 2. Fallback safety backup using the container to automatically resolve viewmodel requirements
                var vm = Handler?.MauiContext?.Services.GetService<LoginViewModel>();
                if (vm != null)
                {
                    Application.Current!.MainPage = new NavigationPage(new LoginPage(vm));
                }
            }
        }
    }
}