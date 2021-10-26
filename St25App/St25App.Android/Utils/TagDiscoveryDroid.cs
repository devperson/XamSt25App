using Android.Nfc;
using Com.ST.St25android;
using Com.ST.St25sdk;
using Com.ST.St25sdk.Iso14443b;
using Com.ST.St25sdk.Type2;
using Com.ST.St25sdk.Type2.St25tn;
using Com.ST.St25sdk.Type4a;
using Com.ST.St25sdk.Type4a.M24srtahighdensity;
using Com.ST.St25sdk.Type4a.St25ta;
using Com.ST.St25sdk.Type4b;
using Com.ST.St25sdk.Type5;
using Com.ST.St25sdk.Type5.Lri;
using Com.ST.St25sdk.Type5.M24lr;
using Com.ST.St25sdk.Type5.St25dv;
using Com.ST.St25sdk.Type5.St25dvpwm;
using Com.ST.St25sdk.Type5.St25tv;
using Com.ST.St25sdk.Type5.St25tvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace St25App.Droid.Utils
{
    public class TagDiscoveryDroid
    {
		public Task<TagInfoDroid> PerformTagDiscoveryAsync(Tag androidTag)
		{
			return Task.Run(() =>
			{
				return PerformTagDiscovery(androidTag);
			});
		}

		private TagInfoDroid PerformTagDiscovery(Tag androidTag)
		{
			var tagInfo = new TagInfoDroid();
			tagInfo.NfcTag = null;
			tagInfo.ProductID = TagHelper.ProductID.ProductUnknown;

			try
			{				
				var readerInterface = AndroidReaderInterface.NewInstance(androidTag);

				if (readerInterface == null)
				{
					return tagInfo;
				}

				var uid = androidTag.GetId();

				if (readerInterface.MTagType == NFCTag.NfcTagTypes.NfcTagTypeV)
				{
					uid = Helper.ReverseByteArray(uid);
					tagInfo.ProductID = TagHelper.IdentifyTypeVProduct(readerInterface, uid);
				}
				else if (readerInterface.MTagType == NFCTag.NfcTagTypes.NfcTagType4a)
				{
					tagInfo.ProductID = TagHelper.IdentifyType4Product(readerInterface, uid);
				}
				else if (readerInterface.MTagType == NFCTag.NfcTagTypes.NfcTagType2)
				{
					tagInfo.ProductID = TagHelper.IdentifyIso14443aType2Type4aProduct(readerInterface, uid);
				}
				else if (readerInterface.MTagType == NFCTag.NfcTagTypes.NfcTagType4b)
				{
					tagInfo.ProductID = TagHelper.IdentifyIso14443BProduct(readerInterface, uid);
				}
				else if (readerInterface.MTagType == NFCTag.NfcTagTypes.NfcTagTypeA || readerInterface.MTagType == NFCTag.NfcTagTypes.NfcTagTypeB)
				{

				}
				
				if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv64kI
			|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv64kJ
			|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv16kI
			|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv16kJ
			|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv04kI
			|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv04kJ)
				{
					tagInfo.NfcTag = new ST25DVTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv04kcI
				|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv04kcJ
				|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv16kcI
				|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv16kcJ
				|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv64kcI
				|| tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv64kcJ)
				{
					tagInfo.NfcTag = new ST25DVCTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.PRODUCTSTLRi512)
				{
					tagInfo.NfcTag = new LRi512Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.PRODUCTSTLRi1K)
				{
					tagInfo.NfcTag = new LRi1KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.PRODUCTSTLRi2K)
				{
					tagInfo.NfcTag = new LRi2KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.PRODUCTSTLRiS2K)
				{
					tagInfo.NfcTag = new LRiS2KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.PRODUCTSTLRiS64K)
				{
					tagInfo.NfcTag = new LRiS64KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStM24sr02Y)
				{
					tagInfo.NfcTag = new M24SR02KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStM24sr04Y
					  || tagInfo.ProductID == TagHelper.ProductID.ProductStM24sr04G)
				{
					tagInfo.NfcTag = new M24SR04KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStM24sr16Y)
				{
					tagInfo.NfcTag = new M24SR16KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStM24sr64Y)
				{
					tagInfo.NfcTag = new M24SR64KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tv512
					  || tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tv02k)
				{
					tagInfo.NfcTag = new ST25TVTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tv04kP)
				{
					tagInfo.NfcTag = new ST25TV04KPTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tv02kc || tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tv512c)
				{
					tagInfo.NfcTag = new ST25TVCTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tv16k)
				{
					tagInfo.NfcTag = new ST25TV16KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tv64k)
				{
					tagInfo.NfcTag = new ST25TV64KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv02kW1)
				{
					tagInfo.NfcTag = new ST25DV02KW1Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25dv02kW2)
				{
					tagInfo.NfcTag = new ST25DV02KW2Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStM24lr16eR)
				{
					tagInfo.NfcTag = new M24LR16KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStM24lr64R || tagInfo.ProductID == TagHelper.ProductID.ProductStM24lr64eR)
				{
					tagInfo.NfcTag = new M24LR64KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStM24lr04eR)
				{
					tagInfo.NfcTag = new M24LR04KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta02k)
				{
					tagInfo.NfcTag = new ST25TA02KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta02kb)
				{
					tagInfo.NfcTag = new ST25TA02KBTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta02kP)
				{
					tagInfo.NfcTag = new ST25TA02KPTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta02kbP)
				{
					tagInfo.NfcTag = new ST25TA02KBPTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta02kD)
				{
					tagInfo.NfcTag = new ST25TA02KDTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta02kbD)
				{
					tagInfo.NfcTag = new ST25TA02KBDTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta16k)
				{
					tagInfo.NfcTag = new ST25TA16KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta512 || tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta512K)
				{
					tagInfo.NfcTag = new ST25TA512Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta512b)
				{
					tagInfo.NfcTag = new ST25TA512BTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25ta64k)
				{
					tagInfo.NfcTag = new ST25TA64KTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductGenericType4 || tagInfo.ProductID == TagHelper.ProductID.ProductGenericType4a)
				{
					tagInfo.NfcTag = new Type4Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductGenericType4b)
				{
					tagInfo.NfcTag = new Type4bTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductGenericIso14443b)
				{
					tagInfo.NfcTag = new Iso14443bTag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductGenericType5AndIso15693)
				{
					tagInfo.NfcTag = new STType5Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductGenericType5)
				{
					tagInfo.NfcTag = new Type5Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductGenericType2)
				{
					tagInfo.NfcTag = new Type2Tag(readerInterface, uid);
				}
				else if (tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tn01k || tagInfo.ProductID == TagHelper.ProductID.ProductStSt25tn512)
				{
					tagInfo.NfcTag = new ST25TNTag(readerInterface, uid);
				}


				if (tagInfo.NfcTag != null)
				{
					var manufacturerName = tagInfo.NfcTag.ManufacturerName;

					if (manufacturerName.Equals("STMicroelectronics"))
					{
						tagInfo.NfcTag.Name = tagInfo.ProductID.ToString();
					}
				}
			}
			catch (STException e)
			{
				tagInfo.Error = e;
				// An STException has occured while instantiating the tag
				Debug.WriteLine(e);
			}

			return tagInfo;
		}
	}


	public class TagInfoDroid
    {
		public NFCTag NfcTag { get; set; }
		public TagHelper.ProductID ProductID { get; set; }
        public STException Error { get; set; }
    }
}