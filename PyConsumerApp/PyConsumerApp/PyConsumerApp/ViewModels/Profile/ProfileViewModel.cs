using Microsoft.AppCenter.Analytics;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.Views.Forms;
using PyConsumerApp.Views.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Profile
{
    /// <summary>
    /// ViewModel for health profile page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public class ProfileViewModel : BaseViewModel
    {

        //private ObservableCollection<CustomerProfile> profile;
        private CustomerProfile profile;
        public Command LogOutCommand { get; set; }
        public Command ChangeCustomerInfo { get; set; }
        App app;
        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="ProfileViewModel" /> class.
        /// </summary>
        public ProfileViewModel()
        {
            app = App.Current as App;
            this.EditProfileCommand = new Command(this.EditProfileClicked);
            // this.ProfileImage = App.BaseImageUrl + "ProfileImage16.png";
            this.ProfileImage = App.profileImage;
            LogOutCommand = new Command(this.Logout_Clicked);
            ChangeCustomerInfo = new Command(this.ChangeInfo_Clicked);
            GetUserProfile();

        }
        #endregion


        private async void Logout_Clicked(object obj)
        {
            var app = Application.Current as App;
            bool answer = await Application.Current.MainPage.DisplayAlert("Log Out", "Do you really want to log out?", "Yes", "no");
            if (answer)
            {
                app.IsLoggedIn = false;
                app.UserPhoneNumber = null;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
            }
        }

        private async void ChangeInfo_Clicked(object obj)
        {
            Analytics.TrackEvent("ModifyUserInfo_Clicked", new Dictionary<string, string> {
                            { "MerchantBranchId", app.Merchantid},
                            { "UserPhoneNumber", app.UserPhoneNumber},
                            });
            bool resonse = await ProfileDataService.Instance.SaveCustomerInfo(Profile);
            GetUserProfile();
            if(resonse == true)
            {
                await Application.Current.MainPage.DisplayAlert("Message", "Profile changed successfully", "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error Message", "Somthing went Wrong", "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }
        public async void GetUserProfile()
        {
            try
            {
                CustomerProfile customerProfile = await ProfileDataService.Instance.GetUserInfo();
                if (customerProfile != null)
                {
                    Profile = customerProfile;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"error "+ex.Message);
            }
            
        }
        [DataMember(Name = "profile")]
        public CustomerProfile Profile
        {
            get => profile;

            set
            {
                if (profile == value) return;

                profile = value;
                NotifyPropertyChanged();
            }
        }
       /* public ObservableCollection<CustomerProfile> Profile
        {
            get { return this.profile; }
            set
            {
                if (this.profile == value)
                {
                    return;
                }

                this.profile = value;
                this.NotifyPropertyChanged();
            }
        }*/
        private async void EditProfileClicked()
        {
            ProfileEditPage view = new ProfileEditPage();
            await Navigation.PushAsync(view);
            await Task.Delay(10);
        }

        public Command EditProfileCommand { get; set; }

        public string ProfileImage { get; set; }
    }
}