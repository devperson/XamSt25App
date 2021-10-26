using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using St25App.Droid.Utils;
using St25App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace St25App.Droid.Services
{
    public class NfcSettingsDroid : INfcSettings
    {
        public void ShowNfcSettings()
        {
            var activity = TagListenerDroid.Activity;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {                
                Intent intent = new Intent(Android.Provider.Settings.ActionNfcSettings);
                activity.StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionWirelessSettings);
                activity.StartActivity(intent);
            }
        }
    }
}