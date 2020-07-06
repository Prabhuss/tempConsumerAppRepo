using PyConsumerApp.Views.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Transaction
{
    /// <summary>
    /// ViewModel for Payment page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class PaymentViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="PaymentViewModel" /> class.
        /// </summary>
        public PaymentViewModel()
        {
            PaymentSuccessIcon = "PaymentSuccess.svg";
            PaymentFailureIcon = "PaymentFailure.svg";
            ContinueShoppingCommand = new Command(ContinueShoppingClicked);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command that will be executed when track order button is clicked.
        /// </summary>
        public Command ContinueShoppingCommand { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Invoked when track order button is clicked.
        /// </summary>
        private void ContinueShoppingClicked(object obj)
        {
            Application.Current.MainPage = new NavigationPage(new BottomNavigationPage());
            BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
        }

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the payment success icon.
        /// </summary>
        public string PaymentSuccessIcon { get; set; }

        /// <summary>
        /// Gets or sets the payment failure icon.
        /// </summary>
        public string PaymentFailureIcon { get; set; }

        #endregion
    }
}
