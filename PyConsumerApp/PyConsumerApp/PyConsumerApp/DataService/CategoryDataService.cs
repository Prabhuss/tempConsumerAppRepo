using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using PyConsumerApp.Models;
using PyConsumerApp.ViewModels.Catalog;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.DataService
{
    /// <summary>
    /// Data service to load the data from json file.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class CategoryDataService : BaseService
    {

        #region fields
        

        private static CategoryDataService instance;

        private CategoryPageViewModel categoryPageViewModel;


        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance for the <see cref="CategoryDataService"/> class.
        /// </summary>
        private CategoryDataService()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="CategoryDataService"/>.
        /// </summary>
        public static CategoryDataService Instance => instance ?? (instance = new CategoryDataService());

        #endregion

        #region Methods

        /// <summary>
        /// Populates the data for view model from json file.
        /// </summary>
        /// <typeparam name="T">Type of view model.</typeparam>
        /// <param name="fileName">Json file to fetch data.</param>
        /// <returns>Returns the view model object.</returns>
       /* public async Task<ObservableCollection<Category>> PopulateData()
        {
            
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("merchant_id", "180");
            ProductListResponse<Dictionary<string, string>> robject = await this.Get<ProductListResponse<Dictionary<string, string>>>(this.getAuthUrl("categoriesPicURL"), payload, null);
            ObservableCollection<Category> categories = new ObservableCollection<Category>();
            var c = 0;
            foreach (var item in robject.Data)
            {
                categories.Add(new Category()
                {
                    Icon = item["PicURL"],
                    Name = item["CategoryName"]
                });
                c++;
            }
            Log.Debug("[CategoryDataService]", "Retrieved categories " + robject.Data.Count);
            return categories;
        }*/
        public async Task<ObservableCollection<SubCategory>> PopulateData()
        {
            var app = App.Current as App;
            //string Merchantid = app.Merchantid;
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("merchant_id", app.Merchantid);
            payload.Add("category_id", "NULL");
            payload.Add("phone_number", app.UserPhoneNumber);
            payload.Add("access_key", app.SecurityAccessKey);
            ProductListResponse<Dictionary<string, string>> robject = await this.Post<ProductListResponse<Dictionary<string, string>>>(this.getAuthUrl("SubCategory"), payload, null);
            ObservableCollection<SubCategory> subCategories = new ObservableCollection<SubCategory>();
            var c = 0;
            foreach (var item in robject.Data)
            {
                subCategories.Add(new SubCategory()
                {
                    SubCategoryId = int.Parse(item["SubCategoryId"]),
                    Name = item["Name"],
                    Icon = item["PicURL"]
                });
                c++;
            }
            Log.Debug("[CategoryDataService]", "Retrieved categories " + robject.Data.Count);
            return subCategories;
        }
        /*
         TO get subcategory under a category
             */
        public async Task<ObservableCollection<SubCategory>> GetSubCategories(SubCategory category)
        {
            var app = App.Current as App;
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("merchant_id", app.Merchantid);
            payload.Add("category_id", category.SubCategoryId);
            payload.Add("phone_number", app.UserPhoneNumber);
            payload.Add("access_key", app.SecurityAccessKey);

            Analytics.TrackEvent("Category_Clicked", new Dictionary<string, string> {
                            { "MerchantBranchId", app.Merchantid},
                            { "UserPhoneNumber", app.UserPhoneNumber},
                            { "CategoryName", category.Name},
                            { "CategoryId", category.CategoryId.ToString()}
                            });
            ProductListResponse<Dictionary<string, string>> robject = await this.Post<ProductListResponse<Dictionary<string, string>>>(this.getAuthUrl("SubCategory"), payload, null);
            ObservableCollection<SubCategory> subCategories = new ObservableCollection<SubCategory>();
            var c = 0;
            if (robject.Data != null)
            {
                foreach (var item in robject.Data)
                {
                    subCategories.Add(new SubCategory()
                    {
                        SubCategoryId = int.Parse(item["SubCategoryId"]),
                        Name = item["Name"],
                        Icon = item["PicURL"]
                    });
                    c++;
                }
                Log.Debug("[CategoryDataService]", "Retrieved subcategories " + robject.Data.Count);
                return subCategories;
            }
            else
            {
                return null;
            }
        }

        //public async Task<ObservableCollection<Product>> GetItems(Category category)
        public async Task<ObservableCollection<Product>> GetItems(SubCategory category, int pageno)
        {
            try
            {
                var app = App.Current as App;
                Dictionary<string, object> payload = new Dictionary<string, object>();
                payload.Add("access_key", app.SecurityAccessKey);
                payload.Add("phone_number", app.UserPhoneNumber);
                payload.Add("merchant_id", app.Merchantid);
                payload.Add("page_size", 5);
                payload.Add("page_number", pageno);
                payload.Add("lastSyncDate", null);
                payload.Add("category_name", category.Name);
                ProductListResponse<Product> robject = await this.Post<ProductListResponse<Product>>(this.getProductUrl("getProductDetails"), payload, null);
                /*ProductListResponse<Dictionary<string, string>> robject = await this.Post<ProductListResponse<Dictionary<string, string>>>(this.getProductUrl("getProductDetails"), payload, null);
                ObservableCollection<Product> products = new ObservableCollection<Product>();
                foreach (var item in robject.Data)
                {
                    products.Add(new Product()
                    {
                       PreviewImage = item["productPicUrl"],
                       TotalQuantity = Convert.ToInt32(item["QuantityList"]),
                       discount = Convert.ToDouble(item["discount"]),
                       SellingPrice = Convert.ToDouble(item["mrp"]),
                       CitrineProdId = Convert.ToInt32(item["CitrineProdId"]),
                       productName = item["productName"],
                       productDesc = item["productDesc"],
                       Category = item["Category"],

                    });
                }
                return products;*/
                 if (robject.Status == "SUCCESS")
                {
                    return robject.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "ok");
                return null;
            }
            
        }
        public async Task<ObservableCollection<SearchProduct>> SearchItems(int pageno, string searchText)
        {
            try
            {
                var app = App.Current as App;
                Dictionary<string, object> payload = new Dictionary<string, object>();
                payload.Add("access_key", app.SecurityAccessKey);
                payload.Add("phone_number", app.UserPhoneNumber);
                payload.Add("merchant_id", app.Merchantid);
                payload.Add("page_size", 10);
                payload.Add("page_number", pageno);
                payload.Add("product_name", searchText);
                ProductListResponse<SearchProduct> robject = await this.Post<ProductListResponse<SearchProduct>>(this.getProductUrl("getProductBySearch"), payload, null);
                if (robject.Status == "SUCCESS")
                {
                    return robject.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "ok");
                return null;
            }
        }
        #endregion
    }

    [DataContract]
    public class ProductListResponse<T>
    {
        [DataMember(Name = "status")]
        public string Status;
        [DataMember(Name = "data")]
        public ObservableCollection<T> Data;

    }
}