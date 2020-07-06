using Microsoft.AppCenter.Analytics;
using PyConsumerApp.DataService;
using PyConsumerApp.Views.Forms;
using PyConsumerApp.Views.Navigation;
using Syncfusion.SfBusyIndicator.XForms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Forms
{
    /// <summary>
    /// ViewModel for login page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class LoginPageViewModel : LoginViewModel
    {
        #region Fields

        private string password;

        private Page page;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="LoginPageViewModel" /> class.
        /// </summary>
        public LoginPageViewModel(INavigation _navigation)
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
            this.SkipCommand = new Command(this.SkipClicked);
            this.ForgotPasswordCommand = new Command(this.ForgotPasswordClicked);
            this.SocialMediaLoginCommand = new Command(this.SocialLoggedIn);
            busyIndicator = new SfBusyIndicator()
            {
                AnimationType = AnimationTypes.Ball,
                ViewBoxHeight = 100,
                ViewBoxWidth = 100,
                Title = "Loading ...",
                TextColor = Color.White
            };
        }

        public void Initailize(Page _page)
        {
            page = _page;
        }
        #endregion

        #region property

        /// <summary>
        /// Gets or sets the property that is bound with an entry that gets the password from user in the login page.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (this.password == value)
                {
                    return;
                }

                this.password = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Gets or sets the command that is executed when the Log In button is clicked.
        /// </summary>
        public Command LoginCommand { get; set; }
        public Command SkipCommand { get; set; }
        /// <summary>
        /// Gets or sets the command that is executed when the Sign Up button is clicked.
        /// </summary>
        public Command SignUpCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the Forgot Password button is clicked.
        /// </summary>
        public Command ForgotPasswordCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the social media login button is clicked.
        /// </summary>
        public Command SocialMediaLoginCommand { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Invoked when the Log In button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void LoginClicked(object obj)
        {
            //var app = App.Current as App;
            ShowHideBusyIndicator(true);
            Debug.WriteLine(@"Invoking the rest  generate otp APi ");
            //if (otp != null)
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                await page.DisplayAlert("Incorrect Number", "Please enter your phone number first", "OK");
                return;
            }
            if (PhoneNumber.Length != 10)
            {
                await page.DisplayAlert("Incorrect Number", "Please enter a 10 digit Phone Number", "OK");
                return;
            }
            Analytics.TrackEvent("LoginClicked", new Dictionary<string, string> {
                            { "UserPhoneNumber", PhoneNumber },
                            { "MerchantBranchId", "180"}
                            });
            Debug.WriteLine(@"Invoking the rest  generate otp APi ");
            var app = Application.Current as App;
            string NotificationToken = app.FireBaseToken;
            Debug.WriteLine(@"Registration Token " + NotificationToken);
            Dictionary<string, string> generateotp_response = await OTPDataService.Instance.GenerateOtp("180", PhoneNumber,NotificationToken);
            Debug.WriteLine(@"Response received " + generateotp_response);
            if (generateotp_response["status"].ToLower() == "success")
            {
                app.SecurityAccessKey = generateotp_response["accessKey"];
                OTPView view = new OTPView();
                view.Initialize(new Dictionary<string, string>()
                {
                    { "phoneNumber", PhoneNumber }
                });
                await Navigation.PushAsync(view);
                await Task.Delay(10);
                ShowHideBusyIndicator(false);
            }
            else
            {
                await page.DisplayAlert("INFO", "Unable to generate OTP. Please try again.", "Ok");
            }
        }

        private async void SkipClicked(object obj)
        {
            Debug.WriteLine("TestMode is enabled, enabling default login");
            await TestLogin();
            return;
        }
        private async Task TestLogin()
        {
            OTPView view = new OTPView();
            view.Initialize(new Dictionary<string, string>()
                {
                    { "phoneNumber", "7635850811" }
            });
            await Task.Delay(100);
            await Navigation.PushAsync(new BottomNavigationPage());
            await Task.Delay(10);
        }

        /// <summary>
        /// Invoked when the Sign Up button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void SignUpClicked(object obj)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when the Forgot Password button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void ForgotPasswordClicked(object obj)
        {
            var label = obj as Label;
            label.BackgroundColor = Color.FromHex("#70FFFFFF");
            await Task.Delay(100);
            label.BackgroundColor = Color.Transparent;
        }

        /// <summary>
        /// Invoked when social media login button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void SocialLoggedIn(object obj)
        {
            // Do something
        }

        private SfBusyIndicator busyIndicator;
        private void ShowHideBusyIndicator(bool isShow)
        {
            busyIndicator.IsVisible = isShow;
        }

        #endregion
    }
}