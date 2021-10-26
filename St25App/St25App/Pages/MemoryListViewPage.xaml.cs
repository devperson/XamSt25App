using St25App.Models;
using St25App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace St25App.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MemoryListViewPage : ContentPage
    {        
        public MemoryListViewPage()
        {
            InitializeComponent();
        }

        private void EditMenuItem_Clicked(object sender, EventArgs e)
        {
            var mi = (MenuItem)sender;
            var item = mi.CommandParameter as TagMemoryRow;

            if (item != null)
            {
                var vm = BindingContext as MemoryListViewModel;
                vm.EditRow(item);
            }
        }
    }
}
