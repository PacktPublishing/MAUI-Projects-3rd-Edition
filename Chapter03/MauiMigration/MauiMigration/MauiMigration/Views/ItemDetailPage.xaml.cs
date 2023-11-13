using MauiMigration.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MauiMigration.Views
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