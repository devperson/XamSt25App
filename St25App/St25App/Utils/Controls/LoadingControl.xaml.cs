using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace St25App.Controls
{
    public partial class LoadingControl : ContentView
    {
        public LoadingControl()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty VisabilityProperty = BindableProperty.Create("Visability", typeof(bool), typeof(LoadingControl), false, propertyChanged: OnVisabilityChanged);
        public bool Visability
        {
            get
            {
                return (bool)GetValue(VisabilityProperty);
            }
            set
            {
                SetValue(VisabilityProperty, value);
            }
        }

        private static async void OnVisabilityChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var cp = (LoadingControl)bindable;
            if (cp != null)
            {
                var newVal = (bool)newvalue;
                if (newVal)
                {
                    cp.IsVisible = true;
                    await cp.FadeTo(0.75, 350);
                    await cp.loadingLabel.FadeTo(1, 400);
                }
                else
                {
                    await cp.FadeTo(0, 350);
                    cp.IsVisible = false;
                }
            }
        }
        
    }
}
