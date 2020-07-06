
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.Views.Transaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Transaction
{
    /// <summary>
    /// ViewModel for Checkout page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public class CheckoutPageViewModel : BaseViewModel
    {

        private string Stage = "TEST";
        private string AppId = "3259e36b680c168d9db036fa9523";
        private string secret = "97d1e574d1c7ba2021a9991bc598978d15c7ef9d";
        private string data = "";
           // { "tokenData" , "oGeDb2sML1YU8EcNOKsLHurG/H86MFoAws/cb7+1FUY="},

        private Dictionary<string, string> GetInputParams()
        {
            return new Dictionary<string, string>(){
            {"appId", "3259e36b680c168d9db036fa9523"},
            { "orderId", "Getpy01" },
            { "orderAmount", "10.00"},
            { "customerName", "SAMAR"},
            { "orderNote", "Order Note"},
            { "orderCurrency", "INR"},
            { "customerPhone", "9706821681"},
            { "customerEmail", "samar@getpy.biz"},
            { "returnUrl", "http://example.com"},
            { "notifyUrl", "https://test.gocashfree.com/notify"}
            };
        }

        public void onComplete(string result)
        {
            Debug.WriteLine($"SDK Result: {result}");
        }
        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="CheckoutPageViewModel" /> class.
        /// </summary>
        public CheckoutPageViewModel(ProfileDataService userDataService, CartDataService cartDataService,
            CatalogDataService catalogDataService)
        {
            this.userDataService = userDataService;
            this.cartDataService = cartDataService;
            this.catalogDataService = catalogDataService;

            DeliveryAddress = new ObservableCollection<Address>();
            PaymentModes = new ObservableCollection<Payment>();

            Device.BeginInvokeOnMainThread(() =>
            {
                FetchPaymentOptions();
                FetchCartList();
            });
            EditCommand = new Command(EditClicked);
            AddAddressCommand = new Command(AddAddressClicked);
            PlaceOrderCommand = new Command(PlaceOrderClicked);
            PaymentOptionCommand = new Command(PaymentOptionClicked);
            ApplyCouponCommand = new Command(ApplyCouponClicked);
            AddressChanged = new Command(addressChangedClicked);
            DeliveryOptionCommand = new Command(addressChangedClicked);
        }

        #endregion

        #region Fields
        public Address selectedDeliveryAddress;
        public Color SelectedAddressColor;
        private ObservableCollection<Address> deliveryAddress;
        public string currentLocationLatitude;
        public string currentLocationLongitude;

        private ObservableCollection<UserCart> orderedItems = new ObservableCollection<UserCart>();

        private ObservableCollection<Payment> paymentModes;

        private ObservableCollection<UserCart> cartDetails;
        public Dictionary<string, int> AddressTypeAndIdPair;

        private double? totalPrice;

        private double? discountPrice;

        private double discountPercent;

        private double percent;
        private readonly ProfileDataService userDataService;
        private readonly CartDataService cartDataService;
        private readonly CatalogDataService catalogDataService;
        private readonly MyOrdersDataService myOrdersDataService;

        private Command backButtonCommand;

        //IMyOrdersDataService myOrdersDataService;

        #endregion

        #region Public properties



        /// <summary>
        /// Gets or sets the property that has been bound with SfListView, which displays the delivery address.
        /// </summary>
        /// 
        public Command DeliveryOptionCommand { get; set; }
        public int DeliveryAddressId { get; set; }
        public string selectedAddressType { get; set; }
        public int selectedAddressId { get; set; }
        [DataMember(Name = "deliveryAddress")]
        public ObservableCollection<Address> DeliveryAddress
        {
            get => deliveryAddress;

            set
            {
                if (deliveryAddress == value) return;

                deliveryAddress = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with SfListView, which displays the payment modes.
        /// </summary>
        public ObservableCollection<Payment> PaymentModes
        {
            get => paymentModes;

            set
            {
                if (paymentModes == value) return;

                paymentModes = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a list view, which displays the cart details.
        /// </summary>
        public ObservableCollection<UserCart> OrderedItems
        {
            get => orderedItems;

            set
            {
                if (orderedItems == value) return;

                orderedItems = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a list view, which displays the cart details.
        /// </summary>
        public ObservableCollection<UserCart> CartDetails
        {
            get => cartDetails;

            set
            {
                if (cartDetails == value) return;

                cartDetails = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays total price.
        /// </summary>
        public double? TotalPrice
        {
            get => totalPrice;

            set
            {
                if (totalPrice == value) return;

                totalPrice = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays total discount price.
        /// </summary>
        public double? DiscountPrice
        {
            get => discountPrice;

            set
            {
                if (discountPrice == value) return;

                discountPrice = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays discount.
        /// </summary>
        public double DiscountPercent
        {
            get => discountPercent;

            set
            {
                if (discountPercent == value) return;

                discountPercent = value;
                NotifyPropertyChanged();
            }
        }
        [DataMember(Name = "selectedAddressType")]
        public string SelectedAddressType
        {
            get => selectedAddressType;

            set
            {
                if (selectedAddressType == value) return;

                selectedAddressType = value;
                NotifyPropertyChanged();
            }
        }
        [DataMember(Name = "currentLocationLongitude")]
        public string CurrentLocationLongitude
        {
            get => currentLocationLongitude;

            set
            {
                if (currentLocationLongitude == value) return;

                currentLocationLongitude = value;
                NotifyPropertyChanged();
            }
        }
        [DataMember(Name = "selectedDeliveryAddress")]
        public Address SelectedDeliveryAddress
        {
            get => selectedDeliveryAddress;

            set
            {
                if (selectedDeliveryAddress == value) return;

                selectedDeliveryAddress = value;
                NotifyPropertyChanged();
            }
        }
        
        [DataMember(Name = "currentLocationLatitude")]
        public string CurrentLocationLatitude
        {
            get => currentLocationLatitude;

            set
            {
                if (currentLocationLatitude == value) return;

                currentLocationLatitude = value;
                NotifyPropertyChanged();
            }
        }
        [DataMember(Name = "selectedAddressId")]
        public int SelectedAddressId
        {
            get => selectedAddressId;

            set
            {
                if (selectedAddressId == value) return;

                selectedAddressId = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Gets or sets the command that will be executed when the Edit button is clicked.
        /// </summary>
        public Command EditCommand { get; set; }
        public Command AddressChanged { get; set; }

        /// <summary>
        /// Gets or sets the command that will be executed when the Add new address button is clicked.
        /// </summary>
        public Command AddAddressCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that will be executed when the Edit button is clicked.
        /// </summary>
        public Command PlaceOrderCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that will be executed when the payment option button is clicked.
        /// </summary>
        public Command PaymentOptionCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that will be executed when the apply coupon button is clicked.
        /// </summary>
        public Command ApplyCouponCommand { get; set; }

        /// <summary>
        /// Gets or sets the command is executed when the back button is clicked.
        /// </summary>
        /// 

        [DataMember(Name = "backButtonCommand")]
        public Command BackButtonCommand => backButtonCommand ?? (backButtonCommand = new Command(BackButtonClicked));


        #endregion

        #region Methods

        /// <summary>
        /// This method is used to get the user address
        /// </summary>
        /// 

        public async void FetchAddresses()
        {
            DeliveryAddress.Clear();
            try
            {
                //if (App.CurrentUserId > 0)
                {
                    var addresses = await userDataService.GetAddresses();
                    if (addresses != null && addresses.Count > 0)
                        foreach (var address in addresses)
                        {
                            //AddressTypeAndIdPair.Add(address.TagName, address.Id);
                            DeliveryAddress.Add(address);
                        }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error2", ex.Message, "OK");
            }
        }

        /// <summary>
        /// This method is used to get the payment options and user card details
        /// </summary>
        private async void FetchPaymentOptions()
        {
            try
            {
                /*if (App.CurrentUserId > 0)
                {
                    var userCards = await userDataService.GetUserCardsAsync(App.CurrentUserId);
                    if (userCards != null && userCards.Count > 0)
                        foreach (var userCard in userCards)
                            PaymentModes.Add(new Payment
                            { PaymentMode = userCard.PaymentMode, CardNumber = userCard.CardNumber });
                }*/

                var paymentOptions = await catalogDataService.GetPaymentOptionsAsync();
                if (paymentOptions != null)
                    foreach (var paymentOption in paymentOptions)
                        PaymentModes.Add(new Payment
                        { PaymentMode = paymentOption.PaymentMode, CardTypeIcon = paymentOption.CardTypeIcon });

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error3", ex.Message, "OK");
            }
        }

        /// <summary>
        /// This method is used to get the cart products from database
        /// </summary>
        private async void FetchCartList()
        {
            try
            {
                /* var orderedData = await cartDataService.GetOrderedItemsAsync(App.CurrentUserId);
                 if (orderedData != null && orderedData.Count > 0)
                     OrderedItems = new ObservableCollection<UserCart>(orderedData);
                 */


                //var products = await cartDataService.GetCartItemAsync(App.CurrentUserId);
                var products = await cartDataService.GetCartItemAsync();
                if (products != null && products.Count > 0)
                {
                    CartDetails = new ObservableCollection<UserCart>(products);
                    UpdatePrice();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error4", ex.Message, "OK");
            }
        }

        /// <summary>
        /// This method is used to update the product total price discount price and percentage
        /// </summary>
        private void UpdatePrice()
        {
            ResetPriceValue();

            if (CartDetails != null && CartDetails.Count > 0)
            {
                foreach (var cartDetail in CartDetails)
                {
                    if (cartDetail.TotalQuantity == 0) cartDetail.TotalQuantity = 1;

                    this.TotalPrice += cartDetail.Product.ActualPrice * cartDetail.TotalQuantity;
                    this.DiscountPrice += cartDetail.Product.DiscountPrice * cartDetail.TotalQuantity;
                    percent += cartDetail.Product.DiscountPercent;
                }

                DiscountPercent = percent > 0 ? percent / CartDetails.Count : 0;
            }
        }

        /// <summary>
        /// This method is used to reset the price amount.
        /// </summary>
        private void ResetPriceValue()
        {
            TotalPrice = 0;
            DiscountPercent = 0;
            DiscountPrice = 0;
            percent = 0;
        }

        /// <summary>
        /// Invoked when the Edit button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void EditClicked(object obj)
        {
            await Task.Delay(100);
            await Navigation.PushAsync(new AddressEditPage());
            await Task.Delay(10);
        }

        /// <summary>
        /// Invoked when the Add address button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void AddAddressClicked(object obj)
        {
            await Task.Delay(100);
            await Navigation.PushAsync(new AddressEditPage());
            await Task.Delay(10);
        }

        /// <summary>
        /// Invoked when the Place order button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void addressChangedClicked(object obj)
        {
            await Application.Current.MainPage.DisplayAlert("Order Address", "Delivery Address Selected", "Ok");
            Address SelectedAddressOption = obj as Address;
            SelectedAddressId = SelectedAddressOption.Id;
            SelectedAddressType = SelectedAddressOption.TagName;
        }
        private async void PlaceOrderClicked(object obj)
        {

            IsBusy = true;
            try
            {
                var app = App.Current as App;
                ObservableCollection<InvoiceItem> invoiceItems = new ObservableCollection<InvoiceItem>();
                CreateOrder createOrder = new CreateOrder();
                OrderDetails orderDetails = new OrderDetails();
                Invoice invoice = new Invoice();
                InvoiceItem invoiceItem = new InvoiceItem();
                createOrder.access_key = app.SecurityAccessKey;
                createOrder.phone_number = app.UserPhoneNumber;
                createOrder.merchant_id = int.Parse(app.Merchantid);

                invoice.DiscountAmount = 0;
                invoice.TaxAmount = 1.55;
                invoice.TotalInvoiceAmount = this.TotalPrice;
                invoice.CouponCode = null;
                invoice.PayableAmount = this.DiscountPrice;
                invoice.InvoiceType = "GetPYApp";
                invoice.OrderStatus = "InProgress";
                invoice.PaymentMode = "COD";
                if(SelectedDeliveryAddress == null)
                {
                    await Application.Current.MainPage.DisplayAlert("No Address Selected", "Please select address from the list or add new address", "Ok");
                    return;
                }
                invoice.DeliverAddressId = SelectedDeliveryAddress.Id;
                orderDetails.Invoice = invoice;

                if (CartDetails != null && CartDetails.Count > 0)
                    foreach (var item in CartDetails)
                    {
                        invoiceItems.Add(new InvoiceItem()
                        {
                            quantity = item.TotalQuantity,
                            ProductName = item.Product.productName,
                            ProductId = item.Product.CitrineProdId,
                            UnitPrice = item.Product.mrp,
                            Discount = item.Product.discount,
                            UnitPriceAfterDiscount = item.Product.SellingPrice,
                            TotalPrice = item.Product.SellingPrice * item.TotalQuantity,
                            ProductImage = item.Product.PreviewImage
                        }); 
                    }
                orderDetails.InvoiceItem = invoiceItems.ToArray();
                createOrder.order_details = orderDetails;
                string json = JsonConvert.SerializeObject(createOrder, Formatting.Indented);
                bool isOrdered = await this.cartDataService.SaveOrdereInDb(json);
                if (isOrdered)
                {

                    var status = await cartDataService.RemoveCartItemsAsync();
                    if (status != null && status.IsSuccess)
                    {
                        await Task.Delay(100);
                        await Application.Current.MainPage.Navigation.PushAsync(new PaymentSuccessPage());
                        await Task.Delay(100);
                        IsBusy = false;
                    }
                    /*Keeping track of New Order 
                     */
                    Analytics.TrackEvent("New Order clicked", new Dictionary<string, string> {
                            { "UserPhoneNumber", app.UserPhoneNumber },
                            { "MerchantId", app.Merchantid },
                            { "InvoiceType ", "GetPY"},
                            { "PaymentMode", "COD"},
                            { "TotalAmount", this.TotalPrice.ToString()}
                            });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Oops!!!", "Ordere could not be process !!!", "Ok");
                    await Task.Delay(100);
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error1" + ex.Message);
                await Task.Delay(100);
                IsBusy = false;
            }


        }

        /// <summary>
        /// Invoked when an back button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
       private async void BackButtonClicked(object obj)
      //  private void BackButtonClicked(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();

            /*Dictionary<string, string> formParams = new Dictionary<string, string>();
            formParams.Add("appId", "3259e36b680c168d9db036fa9523");
            formParams.Add("orderId", "FEX101");
            formParams.Add("orderAmount", "10.00");
            formParams.Add("orderCurrency", "INR");
            formParams.Add("orderNote", "Test payment");
            formParams.Add("customerName", "Customer Name");
            formParams.Add("customerPhone", "9706821681");
            formParams.Add("customerEmail", "test@cashfree.com");
            formParams.Add("returnUrl", "http://example.com");
            formParams.Add("notifyUrl", "http://example.com");
            foreach (var kvp in formParams)
            {
                data = data + kvp.Key + kvp.Value;
            }
            string signature = CreateToken(data, secret);
            formParams.Add("tokenData", signature);
             Button btn = new Button
             {
                 Text = "Do Payment"
             };
             NavigationPage Cp = new NavigationPage(
                 new ContentPage
                 {
                     Title = "Cashfree Sample",
                     Content = new StackLayout
                     {
                         VerticalOptions = LayoutOptions.Center,
                         Children =
                         {
                             btn
                         }
                     }
                 }
             );
             btn.Clicked += (sender, args) => Cp.Navigation.PushAsync(new CFPaymentScreen(Stage, AppId, formParams, this));
             Application.Current.MainPage = Cp;
             */

           // await Application.Current.MainPage.Navigation.PushAsync(new CFPaymentScreen(Stage, AppId, GetInputParams(), this));

        }

        /// <summary>
        /// Invoked when the Payment option is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void PaymentOptionClicked(object obj)
        {
            if (obj is RowDefinition rowDefinition && rowDefinition.Height.Value == 0)
                rowDefinition.Height = GridLength.Auto;
        }

        /// <summary>
        /// Invoked when the Apply coupon button is selected.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void ApplyCouponClicked(object obj)
        {
            // Do something
        }
        private string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        #endregion
    }
}
