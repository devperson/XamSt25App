using St25App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace St25App.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditMemoryRowPage : ContentPage
    {
        public EditMemoryRowPage()
        {
            InitializeComponent();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            var vm = this.BindingContext as EditMemoryRowViewModel;
            vm.OnApplyChangesCommand();
        }
    }
}