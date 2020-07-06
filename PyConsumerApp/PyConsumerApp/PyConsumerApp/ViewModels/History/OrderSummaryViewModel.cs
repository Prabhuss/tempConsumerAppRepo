using PyConsumerApp.Models.History;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.History
{
    [Preserve(AllMembers = true)]
    [DataContract]
    public class OrderSummaryViewModel
    {
        [DataMember(Name = "LineItemList")]
        public ObservableCollection<InvocieLineItem> LineItemList { get; set; }
        [DataMember(Name = "TotalItems")]
        public int TotalItems { get; set; }
        public string totalAmount;
        public string totalMRP;
        public string discount;
        public string productImageUrl;
        public OrderSummaryViewModel(CustomerInvoiceDatum InvoiceDetails, ObservableCollection<InvocieLineItem> LineitemFromCloud)
        {
            TotalAmount = InvoiceDetails.PayableAmount.ToString();
            TotalMRP = InvoiceDetails.TotalInvoiceAmount.ToString();
            Discount = InvoiceDetails.DiscountAmount.ToString();
            LineItemList = new ObservableCollection<InvocieLineItem>();
            InvoiceItems(InvoiceDetails.CustomerInvoiceId, LineitemFromCloud);
        }

        //properties

        [DataMember(Name = "TotalAmount")]
        public string TotalAmount
        {
            get
            {
                return this.totalAmount;
            }

            set
            {
                if (this.totalAmount == value)
                {
                    return;
                }

                this.totalAmount = value;
            }
        }
        [DataMember(Name = "productImageUrl")]
        public string ProductImageUrl
        {
            get
            {
                return this.productImageUrl;
            }

            set
            {
                if (this.productImageUrl == value)
                {
                    return;
                }

                this.productImageUrl = value;
            }
        }
        
        [DataMember(Name = "TotalMRP")]
        public string TotalMRP
        {
            get
            {
                return this.totalMRP;
            }

            set
            {
                if (this.totalMRP == value)
                {
                    return;
                }

                this.totalMRP = value;
            }
        }
        [DataMember(Name = "Discount")]
        public string Discount
        {
            get
            {
                return this.discount;
            }

            set
            {
                if (this.discount == value)
                {
                    return;
                }

                this.discount = value;
            }
        }

        private Command backButtonCommand;
        [DataMember(Name = "backButtonCommand")]
        public Command BackButtonCommand => backButtonCommand ?? (backButtonCommand = new Command(BackButtonClicked));

        private async void BackButtonClicked(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }





        public void InvoiceItems(long CustomerInvoiceId, ObservableCollection<InvocieLineItem> LineitemFromCloud)
        {
            try
            {
                this.LineItemList.Clear();
                int itemCount = 0;
                foreach (var lineitem in LineitemFromCloud)
                {
                    if (lineitem.CustomerInvoiceId == CustomerInvoiceId)
                    {
                        this.LineItemList.Add(lineitem);
                        itemCount++;
                    }
                }
                TotalItems = itemCount;
                Console.WriteLine("Total items :" + TotalItems);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception raised while retrieving Product List " + e.Message);
            }
        }

    }

}
