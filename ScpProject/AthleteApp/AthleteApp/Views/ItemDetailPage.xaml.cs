using AthleteApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace AthleteApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}