using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Plugin.Social.Facebook
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FacebookOAuthRequest : ContentPage
    {

        private string RedirectUrl { get; set; }
        private string ClientId { get; set; }
        private string Scope { get; set; }
        private string LoadingMessage { get; set; }

        /// <summary>
        /// Oauth Response
        /// </summary>
        public event EventHandler<FacebookOAuthResult> AccessTokenResult;

        /// <summary>
        /// Facebook oauth request
        /// </summary>
        /// <param name="clientid">AppID</param>
        /// <param name="redirecturl"></param>
        /// <param name="scope"></param>
        /// <param name="accesstokenresult">Response</param>
        /// <param name="loadingmessage"></param>
        public FacebookOAuthRequest(string clientid, string redirecturl = "https://www.facebook.com/connect/login_success.html", string scope = "email", string loadingmessage = "")
        {
            InitializeComponent();
            ProgressIndicatorText.Text = loadingmessage;
            ClientId = clientid;
            RedirectUrl = redirecturl;
            Scope = scope;
            LoadingMessage = loadingmessage;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            while (!(await IsServerReachable()))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ProgressIndicatorLayout.IsVisible = true;
                    ProgressIndicatorRing.IsVisible = false;
                    ProgressIndicatorText.Text = LoadingMessage;
                });
                await Task.Delay(TimeSpan.FromSeconds(15));
            }

            if (await IsServerReachable())
            {
                RequestAccessToken();
            }
        }

        private async Task<bool> IsServerReachable()
        {
            try
            {
                HttpClient client = new HttpClient();
                var getresponse = await client.GetAsync("https://www.facebook.com");
                if (getresponse == null) return false;
                return getresponse.IsSuccessStatusCode;
            }
            catch { }
            return false;
        }

        private void OnAccessTokenResult(FacebookOAuthResult oauthresult)
        {
            if (AccessTokenResult != null)
            {
                AccessTokenResult.Invoke(this, oauthresult);
            }
        }

        public void RequestAccessToken()
        {
            var apiurl = $"https://www.facebook.com/dialog/oauth?client_id={ClientId}&display=popup&response_type=token&redirect_uri={RedirectUrl}&scope={Scope}";
            WebClient.Navigated += WebClient_Navigated;
            WebClient.Source = apiurl;
        }

        private void WebClient_Navigated(object sender, WebNavigatedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ProgressIndicatorLayout.IsVisible = false;
                WebClient.IsVisible = true;
            });
            var accesstoken = ExtractAccessTokenFromUrl(e.Url);
            if (!string.IsNullOrEmpty(accesstoken))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ProgressIndicatorLayout.IsVisible = true;
                    WebClient.IsVisible = false;
                });
                OnAccessTokenResult(new FacebookOAuthResult
                {
                    AccessToken = accesstoken,
                    FacebookPage = this
                });
            }
        }

        private string ExtractAccessTokenFromUrl(string url)
        {
            if (url.Contains("access_token"))
            {
                var at = url.Replace($"{RedirectUrl}#access_token=", "");
                if (at.Contains("&expires_in=")) at = at.Remove(at.IndexOf("&expires_in="));
                if(at.Contains("&data_access_expiration_time=")) at = at.Remove(at.IndexOf("&data_access_expiration_time="));
                return at;
            }
            return string.Empty;
        }
    }
}
