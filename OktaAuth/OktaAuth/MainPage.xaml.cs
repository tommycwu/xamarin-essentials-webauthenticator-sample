using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OktaAuth
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly LoginService loginService = new LoginService();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void LoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var callbackUrl = new Uri(OktaConfiguration.Callback);
                var authUrl = new Uri(loginService.BuildAuthenticationUrl());
                var authenticationResult = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
                var token = loginService.ParseAuthenticationResult(authenticationResult);
                var nameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "given_name");
                if (nameClaim != null)
                {
                    WelcomeLabel.Text = $"Welcome to Xamarin.Forms {nameClaim.Value}!";
                    LogoutButton.IsVisible = !(LoginButton.IsVisible = false);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        private async void LogoutButtonClicked(object sender, EventArgs e)
        {
            WelcomeLabel.Text = "Welcome to Xamarin.Forms!";
            LogoutButton.IsVisible = !(LoginButton.IsVisible = true);
            await Browser.OpenAsync($"{OktaConfiguration.OrganizationUrl}/login/signout", BrowserLaunchMode.SystemPreferred);
        }
    }
}
