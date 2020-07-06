using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Transaction
{

    [Preserve(AllMembers = true)]
    [DataContract]
    class AddressEditViewodel : BaseViewModel
    {
        public Command ChangeAddressInfo { get; set; }
        public Command UseMyLocationCommand { get; set; }
        
        public Address customerAddress;

        public ObservableCollection<string> addressTypesList;
        public ObservableCollection<string> AddressTypesList
        {
            get { return addressTypesList; }
            set
            {
                if (addressTypesList != value)
                {
                    addressTypesList = value;
                    OnPropertyChanged();
                }
            }
        }

        string selectedAddressType;
        [DataMember(Name = "selectedAddressType")]
        public string SelectedAddressType
        {
            get { return selectedAddressType; }
            set
            {
                if (selectedAddressType != value)
                {
                    selectedAddressType = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Name = "customerAddress")]
        public Address CustomerAddress
        {
            get => customerAddress;

            set
            {
                if (customerAddress == value) return;
                customerAddress = value;
                NotifyPropertyChanged();
            }
        }

        public AddressEditViewodel()
        {
            customerAddress = new Address();
            addressTypesList = new ObservableCollection<string>();
            addressTypesList.Add("Office");
            addressTypesList.Add("Home");
            addressTypesList.Add("Other");
            selectedAddressType = "";
            ChangeAddressInfo = new Command(this.ChangeAddress_Clicked);
            UseMyLocationCommand = new Command(this.UseMyLocation_Clicked); 
        }

        private Command backButtonCommand;
        [DataMember(Name = "backButtonCommand")]
        public Command BackButtonCommand => backButtonCommand ?? (backButtonCommand = new Command(BackButtonClicked));

        private async void BackButtonClicked(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }


        private async void UseMyLocation_Clicked(object obj)
        {
            if(CustomerAddress.FirstName== null || CustomerAddress.Address2 == null || CustomerAddress.TagName == null)
            {
                await Application.Current.MainPage.DisplayAlert("To use location", "Please fill all the above fields", "Ok");
                return;
            }
            try
            {
                CustomerAddress.Address1 = "Location is given as Delivery Address";
                CustomerAddress.SocietyBuildingNo = "";
                CustomerAddress.FlatNoDoorNo = "";
                CustomerAddress.City = "";
                CustomerAddress.State = "";

                var app = App.Current as App;
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    if (location != null)
                    {
                        CustomerAddress.Latitude = location.Latitude.ToString();
                        CustomerAddress.Longitude = location.Longitude.ToString();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Location Error1", "Unable to fetch Current location", "Ok");
                    }
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    await Application.Current.MainPage.DisplayAlert("Location Error2", "Unable to fetch Current location", "Ok");
                }
                catch (PermissionException pEx)
                {
                    await Application.Current.MainPage.DisplayAlert("Location Error3", "Unable to fetch Current location", "Ok");
                }
                catch (System.Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Location Error4", "Unable to fetch Current location", "Ok");
                }

                bool resonse = await CartDataService.Instance.SaveAddressInfo(CustomerAddress);
                if (resonse == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Message", "Address changed successfully", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error Message", "Somthing went Wrong", "Ok");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error101", e.Message, "OK");
            }
        }
        private async void ChangeAddress_Clicked(object obj)
        {
            if (CustomerAddress.FirstName == null || CustomerAddress.Address2 == null || CustomerAddress.TagName == null
                || CustomerAddress.Address1 == null || CustomerAddress.PostalCodeZipCode == null)
            {
                await Application.Current.MainPage.DisplayAlert("Fields Empty Error", "Please fill all the Mandatory fields", "Ok");
                return;
            }
            try
            {
                bool resonse = await CartDataService.Instance.SaveAddressInfo(CustomerAddress);
                if (resonse == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Message", "Address changed successfully", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error Message", "Somthing went Wrong", "Ok");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error102", e.Message, "OK");
            }
        }

    }
}
