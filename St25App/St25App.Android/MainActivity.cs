using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using St25App.Droid.Utils;
using Prism;
using Prism.Ioc;
using St25App.Services;
using St25App.Droid.Services;

namespace St25App.Droid
{
    [Activity(Label = "St25App", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        TagListenerDroid nfcListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new AndroidInitializer()));

            nfcListener = new TagListenerDroid(this);
        }

        protected override void OnResume()
        {
            base.OnResume();

            nfcListener.StartListening();
        }

        protected override void OnPause()
        {
            base.OnPause();

            nfcListener.StopListening();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            nfcListener.ProcessNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            // Register any platform specific implementations
            container.Register<INfcSettings, NfcSettingsDroid>();
            container.Register<ITagReadWriteMemory, TagReadWriteMemDroid>();            
        }
    }
}