using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.ViewModels.Catalog;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PyConsumerApp.Views.Catalog
{
    /// <summary>
    /// The Category Tile page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryTilePage
    {
        //private CategoryPageViewModel vm;
        private SubCategoryPageViewModel vm;
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryTilePage" /> class.
        /// </summary>
        public CategoryTilePage()
        {
            InitializeComponent();
            //vm = new CategoryPageViewModel();
            vm = new SubCategoryPageViewModel();
            this.BindingContext = vm;
            new Task(async () => {
                await populateData();
            }).Start();
        }
        async private Task populateData()
        {
           // ObservableCollection<Category> categories = await CategoryDataService.Instance.PopulateData();
            ObservableCollection<SubCategory> categories = await CategoryDataService.Instance.PopulateData();
            if (categories != null)
            {
                vm.Categories = categories;
            }
        }

        
    }
}