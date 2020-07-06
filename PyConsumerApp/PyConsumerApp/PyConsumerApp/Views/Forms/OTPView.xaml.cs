using PyConsumerApp.Events;
using PyConsumerApp.ViewModels.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static PyConsumerApp.Events.GlobalEvents;

namespace PyConsumerApp.Views.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OTPView : ContentPage
    {
        OTPViewModel vModel;
        public OTPView()
        {
            InitializeComponent();          
            vModel = new OTPViewModel(Navigation);
            BindingContext = vModel;
            GlobalEvents.OnSMSReceived += GlobalEvents_OnSMSReceived;
        }

        public void Initialize(object loginData)
        {
            vModel.Initialize(loginData, this);
        }

        private void GlobalEvents_OnSMSReceived(object sender, SMSEventArgs e)
        {
            Log.Debug("GlobalEvents_OnSMSReceived", "Message received is :" + e.Message + "-Phone:" + e.PhoneNumber);
            //EntryMessage.Text = e.Message;
            if (e.Message.Contains("Your OTP to redeem point is"))
            {
                OTP.Text = e.Message.Replace("Your OTP to redeem point is", "").Trim();
            }
        }
    }
}