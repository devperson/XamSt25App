using Prism;
using Prism.Ioc;
using St25App.Models;
using St25App.Pages;
using St25App.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace St25App
{
    public partial class App
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(Pages.MainPage)}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {            
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainViewModel>();
            containerRegistry.RegisterForNavigation<MemoryListViewPage, MemoryListViewModel>();
            containerRegistry.RegisterForNavigation<EditMemoryRowPage, EditMemoryRowViewModel>();            
        }

        public static Action<TagInfo> TagDiscoveredAction { get; set; }
        public static Action NfcDisabledAction { get; set; }
    }
}
