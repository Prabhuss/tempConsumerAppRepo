﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PyConsumerApp.Models;
using PyConsumerApp.Models.History;
using PyConsumerApp.ViewModels.History;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.DataService
{
    /// <summary>
    /// Data service to load the data from json file.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class MyOrdersDataService : BaseService
    {
        #region fields

        private static MyOrdersDataService instance;

        private MyOrdersPageViewModel myOrderPageViewModel;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance for the <see cref="MyOrdersDataService"/> class.
        /// </summary>
        private MyOrdersDataService()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="MyOrdersDataService"/>.
        /// </summary>
        public static MyOrdersDataService Instance => instance ?? (instance = new MyOrdersDataService());

        /// <summary>
        /// Gets or sets the value of my orders page view model.
        /// </summary>
        public MyOrdersPageViewModel MyOrdersPageViewModel =>
            (this.myOrderPageViewModel = PopulateData<MyOrdersPageViewModel>("ecommerce.json"));

        #endregion

        #region Methods

        /// <summary>
        /// Populates the data for view model from json file.
        /// </summary>
        /// <typeparam name="T">Type of view model.</typeparam>
        /// <param name="fileName">Json file to fetch data.</param>
        /// <returns>Returns the view model object.</returns>
        private  static T PopulateData<T>(string fileName)
        {
            var file = "PyConsumerApp.Data." + fileName;

            var assembly = typeof(App).GetTypeInfo().Assembly;

            T obj;

            using (var stream = assembly.GetManifestResourceStream(file))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                obj = (T)serializer.ReadObject(stream);
            }

            return obj;
            
        }


        public async Task<CustomerOrders> GetOrderedItemsAsync()
        {
            //var UserCarts = new List<UserCart>();
            try
            {
                var app = App.Current as App;
                Dictionary<string, object> payload = new Dictionary<string, object>();
                payload.Add("access_key", app.SecurityAccessKey);
                payload.Add("merchant_id", app.Merchantid);
                payload.Add("phone_number", app.UserPhoneNumber);
                payload.Add("start_date", "2020-06-08 09:38:30");
                payload.Add("page_size", "5");
                payload.Add("page_number", "1");
                payload.Add("end_date", DateTime.Now);
                CustomerOrders robject = await this.Post<CustomerOrders>(this.getOrderUrl("GetOrder"), payload, null);
                //var robject = await this.Post<CustomerOrders>(this.getOrderUrl("GetOrder"), payload, null);
                if (robject.Status == "SUCCESS")
                {
                    return robject;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"err" + ex.Message);
                return null;
            }

            // return UserCarts;
        }
       /* public async Task<bool>SaveOrdereInDb(string Payload)
        {
            try
            {
                OrderListResponse<Dictionary<string,object>> robject = await this.Post<OrderListResponse<Dictionary<string, object>>>(this.getOrderUrl("CreateOrder"), Payload, null);
                if (robject.Status.ToLower() == "success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }*/

        #endregion
    }
    [DataContract]
    public class OrderListResponse<T>
    {
        [DataMember(Name = "status")]
        public string Status;
        [DataMember(Name = "data")]
        public ObservableCollection<T> Data;

    }
}