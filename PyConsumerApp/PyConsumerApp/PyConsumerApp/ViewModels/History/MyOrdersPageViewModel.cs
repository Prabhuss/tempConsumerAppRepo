﻿using PyConsumerApp.DataService;
using PyConsumerApp.Models.History;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.History
{
    /// <summary>
    /// ViewModel for my orders page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public class MyOrdersPageViewModel : BaseViewModel
    {
        #region Fields
        //private ObservableCollection<Orders> orderDetails;
        //private ObservableCollection<Orders> myOrders;
        private readonly MyOrdersDataService orderDataService;

        private ObservableCollection<CustomerInvoiceDatum> orderDetails;
        private ObservableCollection<InvocieLineItem> productList;
        private ObservableCollection<CustomerInvoiceDatum> myOrders;
        private ObservableCollection<CustomerInvoiceDatum> completedOrders;
        private ObservableCollection<CustomerInvoiceDatum> canceledOrders;
        private ObservableCollection<CustomerInvoiceDatum> requestedOrders;
        private Command itemSelectedCommand;

        #endregion
        public MyOrdersPageViewModel()
        {

        }
        public MyOrdersPageViewModel(MyOrdersDataService myOrdersDataService)
        {
            this.orderDataService = myOrdersDataService;
            OrderDetails = new ObservableCollection<CustomerInvoiceDatum>();
            RequestedOrders = new ObservableCollection<CustomerInvoiceDatum>();
            CancelOrders = new ObservableCollection<CustomerInvoiceDatum>();
            CompleteOrders = new ObservableCollection<CustomerInvoiceDatum>();
            ProductList = new ObservableCollection<InvocieLineItem>();
            LoadItemsCommand = new Command(async () => await FetchOrders());
        }

        #region Properties
        /// <summary>
        /// Gets or sets the property that has been bound with list view, which displays the collection of orders from json.
        /// </summary>
        [DataMember(Name = "orders")]
        /* public ObservableCollection<Orders> MyOrders
         {
             get
             {
                 return this.myOrders;
             }

             set
             {
                 if (this.myOrders == value)
                 {
                     return;
                 }

                 this.myOrders = value;
                 this.NotifyPropertyChanged();
                 this.GetProducts(this.MyOrders);
             }
         }*/
        public ObservableCollection<CustomerInvoiceDatum> CompleteOrders
        {
            get
            {
                return this.completedOrders;
            }

            set
            {
                if (this.completedOrders == value)
                {
                    return;
                }

                this.completedOrders = value;
                this.NotifyPropertyChanged();
                //this.GetProducts(this.MyOrders);
            }
        }
        public ObservableCollection<CustomerInvoiceDatum> CancelOrders
        {
            get
            {
                return this.canceledOrders;
            }

            set
            {
                if (this.canceledOrders == value)
                {
                    return;
                }

                this.canceledOrders = value;
                this.NotifyPropertyChanged();
                //this.GetProducts(this.MyOrders);
            }
        }
        public ObservableCollection<CustomerInvoiceDatum> MyOrders
        {
            get
            {
                return this.myOrders;
            }

            set
            {
                if (this.myOrders == value)
                {
                    return;
                }

                this.myOrders = value;
                this.NotifyPropertyChanged();
                //this.GetProducts(this.MyOrders);
            }
        }
        /// <summary>
        /// Gets or sets the property that has been bound with a list view, which displays the order details in list.
        /// </summary>
       /* public ObservableCollection<Orders> OrderDetails
        {
            get
            {
                return this.orderDetails;
            }

            set
            {
                if (this.orderDetails == value)
                {
                    return;
                }

                this.orderDetails = value;
                this.NotifyPropertyChanged();
            }
        }*/
        public ObservableCollection<CustomerInvoiceDatum> OrderDetails
        {
            get
            {
                return this.orderDetails;
            }
            set
            {
                if (this.orderDetails == value)
                {
                    return;
                }

                this.orderDetails = value;
                this.NotifyPropertyChanged();
            }
        }
        public ObservableCollection<CustomerInvoiceDatum> RequestedOrders
        {
            get
            {
                return this.requestedOrders;
            }
            set
            {
                if (this.requestedOrders == value)
                {
                    return;
                }

                this.requestedOrders = value;
                this.NotifyPropertyChanged();
            }
        }

        public ObservableCollection<InvocieLineItem> ProductList
        {
            get
            {
                return this.productList;
            }
            set
            {
                if (this.productList == value)
                {
                    return;
                }

                this.productList = value;
                this.NotifyPropertyChanged();
            }
        }
        #endregion

        #region Command

        /// <summary>
        /// Gets the command that will be executed when an item is selected.
        /// </summary>
        public Command LoadItemsCommand { get; set; }
        public Command ItemSelectedCommand
        {
            get
            {
                return this.itemSelectedCommand ?? (this.itemSelectedCommand = new Command(this.ItemSelected));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoked when an item is selected.
        /// </summary>
        /// <param name="attachedObject">The Object</param>
        private void ItemSelected(object attachedObject)
        {
            // Do something
        }

        /// <summary>
        /// This method is used to get the ordered items from json.
        /// </summary>
        /// <param name="items">Ordered items</param>
       /* private void GetProducts(ObservableCollection<Orders> items)
        {
            this.OrderDetails = new ObservableCollection<Orders>();
            if (items != null && items.Count > 0)
            {
                this.OrderDetails = items;
            }
        }*/
        private void GetProducts(ObservableCollection<CustomerInvoiceDatum> items)
        {
           this.OrderDetails = new ObservableCollection<CustomerInvoiceDatum>();
            if (items != null && items.Count > 0)
            {
                //this.OrderDetails = items;
                //var isProductAlreadyAdded = items.Any<>

            }

        }

        public async Task FetchOrders()
        {
            try
            {
                OrderDetails.Clear();
                RequestedOrders.Clear();
                CancelOrders.Clear();
                CompleteOrders.Clear();
                ProductList.Clear();
                CustomerOrders orders = await this.orderDataService.GetOrderedItemsAsync();
                if (orders != null)
                {
                    foreach (var item in orders.Data.CustomerInvoiceData)
                    {
                        OrderDetails.Add(item);
                        if (!string.IsNullOrEmpty(item.OrderStatus))
                        {
                            if (item.OrderStatus.ToLower() == "inprogress")
                                RequestedOrders.Add(item);
                            else if (item.OrderStatus.ToLower() == "rejected")
                                CancelOrders.Add(item);
                            else if (item.OrderStatus.ToLower() == "delivered")
                                CompleteOrders.Add(item);
                        }
                    }

                    foreach (var item in orders.Data.InvocieLineItems)
                    {
                        ProductList.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("msg", "Unable to load order list", "ok");
            }
            finally
            {
                IsBusy = false;
            }

        }

        #endregion
    }
}
