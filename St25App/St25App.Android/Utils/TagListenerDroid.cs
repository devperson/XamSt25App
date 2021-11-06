using Android;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Com.ST.St25sdk;
using Java.Lang;
using St25App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;

namespace St25App.Droid.Utils
{
    public class TagListenerDroid
    {
        private NfcAdapter nfcAdapter;        
		private PendingIntent pendingIntent;
		private byte[] UNTRACEABLE_UID = new byte[] { (byte)0xE0, (byte)0x02, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00 };
		
		public TagListenerDroid(Activity activity)
        {
			Activity = activity;

			nfcAdapter = NfcAdapter.GetDefaultAdapter(Activity);

			var intent = new Intent(Activity, Activity.GetType()).AddFlags(ActivityFlags.SingleTop);
			pendingIntent = PendingIntent.GetActivity(Activity, 0, intent, 0);
		}

		public static TagInfoDroid TagInfoDroid;
		public static Activity Activity;

		/// <summary>
		/// Checks if NFC Feature is available
		/// </summary>
		public bool IsAvailable
		{
			get
			{

				if (Application.Context.CheckCallingOrSelfPermission(Manifest.Permission.Nfc) != Android.Content.PM.Permission.Granted)
					return false;
				return nfcAdapter != null;
			}
		}

		/// <summary>
		/// Checks if NFC Feature is enabled
		/// </summary>
		public bool IsEnabled => IsAvailable && nfcAdapter.IsEnabled;

		/// <summary>
		/// Starts tags detection
		/// </summary>
		public void StartListening()
		{
			if (nfcAdapter != null)
			{
				if (nfcAdapter.IsEnabled)
				{
					var ndefFilter = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
					ndefFilter.AddDataType("*/*");

					var tagFilter = new IntentFilter(NfcAdapter.ActionTagDiscovered);
					tagFilter.AddCategory(Intent.CategoryDefault);

					var filters = new IntentFilter[] { ndefFilter, tagFilter };

					nfcAdapter.EnableForegroundDispatch(Activity, pendingIntent, filters, null);
				}
				else
                {
					//show enable NFC button
					App.NfcDisabledAction();
				}
			}
			else
            {
				ShowBlackToast("NFC is unavailable.");
			}
		}

		public void StopListening()
        {
			if (nfcAdapter != null)
			{
				try
				{
					nfcAdapter.DisableForegroundDispatch(Activity);					
				}
				catch (IllegalStateException e)
				{
					System.Diagnostics.Debug.WriteLine("Illegal State Exception disabling NFC. Assuming application is terminating.");
					ShowBlackToast("Illegal State Exception disabling NFC. Assuming application is terminating.");
				}
				catch (UnsupportedOperationException e)
				{
					System.Diagnostics.Debug.WriteLine("FEATURE_NFC is unavailable.");
					ShowBlackToast("FEATURE_NFC is unavailable.");					
				}
			}
		}

        public async void ProcessNewIntent(Intent intent)
        {
			if (intent == null)
				return;

			if (intent.Action == NfcAdapter.ActionTagDiscovered || intent.Action == NfcAdapter.ActionNdefDiscovered)
			{
				var androidTag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
				if (androidTag != null)
				{
					var uid = Helper.ReverseByteArray(androidTag.GetId());
					if (uid.SequenceEqual(UNTRACEABLE_UID))
					{
						//leaveUntraceableMode
						System.Diagnostics.Debug.WriteLine("UNTRACEABLE_UID");
					}
					else
					{
						try
						{
							var tagDiscovery = new TagDiscoveryDroid();
							TagInfoDroid = await tagDiscovery.PerformTagDiscoveryAsync(androidTag);

							if (TagInfoDroid.Error == null)//no errors
							{
								var tagInfo = new Models.TagInfo();

								await Task.Run(() =>
								{
									tagInfo.Name = TagInfoDroid.NfcTag.Name;
									tagInfo.Description = TagInfoDroid.NfcTag.Description;
									tagInfo.Type = TagInfoDroid.NfcTag.TypeDescription;
									tagInfo.Manufacturer = TagInfoDroid.NfcTag.ManufacturerName;
									tagInfo.mUID = Helper.ConvertHexByteArrayToString(TagInfoDroid.NfcTag.GetUid()).ToUpper();
									tagInfo.SizeInBytes = TagInfoDroid.NfcTag.MemSizeInBytes;
									tagInfo.TechList = TagInfoDroid.NfcTag.GetTechList().ToList();
								});

								System.Diagnostics.Debug.WriteLine(tagInfo);

								App.OnTagDiscovered(tagInfo);
							}
							else
                            {
								ShowBlackToast(TagInfoDroid.Error.Error.ToString());
							}
						}
						catch(STException e)
                        {
							System.Diagnostics.Debug.WriteLine(e);
							ShowBlackToast(e.Error.ToString());
						}
					}

					// This intent has been processed. Reset it to be sure that we don't process it again
					// if the MainActivity is resumed
					Activity.Intent = null;
				}
			}
		}


		public static void ShowBlackToast(string message)
		{
			var toast = Toast.MakeText(Activity, message, ToastLength.Long);
			var view = toast.View;

			//Gets the actual oval background of the Toast then sets the colour filter
			view.Background.SetColorFilter(Xamarin.Forms.Color.Black.ToAndroid(), PorterDuff.Mode.SrcIn);

			//Gets the TextView from the Toast so it can be editted
			var text = (TextView)view.FindViewById(Android.Resource.Id.Message);
			text.SetTextColor(Xamarin.Forms.Color.White.ToAndroid());

			toast.Show();
		}
	}
}