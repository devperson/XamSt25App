using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.ST.St25sdk;
using Com.ST.St25sdk.Type2;
using Com.ST.St25sdk.Type4a;
using Com.ST.St25sdk.Type5;
using St25App.Droid.Utils;
using St25App.Models;
using St25App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St25App.Droid.Services
{
    public class TagReadWriteMemDroid : ITagReadWriteMemory
    {
        public async Task<List<TagMemoryRow>> GetMemoryRowsAsync(int mStartAddress, int mNumberOfBytes)
        {
            var mBuffer = await ReadRangeAsync(mStartAddress, mNumberOfBytes);
            if (mBuffer == null)
                return null;

            var count = Helper.DivisionRoundedUp(mBuffer.Length, Constants.NBR_OF_BYTES_PER_RAW);

            var list = new List<TagMemoryRow>();

            for (int pos = 0; pos < count; pos++)
            {                
                var row = new TagMemoryRow();

                // The data are now read by Byte but we will still format the display by row of 4 Bytes
                // Get the 4 Bytes to display on this row
                var address = pos * Constants.NBR_OF_BYTES_PER_RAW;
                if (address < mBuffer.Length)
                {
                    row.Bytes[0] = (sbyte)mBuffer[address];
                    row.Byte1Hex = string.Format("{0:x2}", row.Bytes[0]).ToUpper();
                    row.Byte1Char = this.GetChar(row.Bytes[0]);
                }

                address = pos * Constants.NBR_OF_BYTES_PER_RAW + 1;
                if (address < mBuffer.Length)
                {
                    row.Bytes[1] = (sbyte)mBuffer[address];
                    row.Byte2Hex = string.Format("{0:x2}", row.Bytes[1]).ToUpper();
                    row.Byte2Char = this.GetChar(row.Bytes[1]);
                }

                address = pos * Constants.NBR_OF_BYTES_PER_RAW + 2;
                if (address < mBuffer.Length)
                {
                    row.Bytes[2] = (sbyte)mBuffer[address];
                    row.Byte3Hex = string.Format("{0:x2}", row.Bytes[2]).ToUpper();
                    row.Byte3Char = this.GetChar(row.Bytes[2]);
                }

                address = pos * Constants.NBR_OF_BYTES_PER_RAW + 3;
                if (address < mBuffer.Length)
                {
                    row.Bytes[3] = (sbyte)mBuffer[address];
                    row.Byte4Hex = string.Format("{0:x2}", row.Bytes[3]).ToUpper();
                    row.Byte4Char = this.GetChar(row.Bytes[3]);
                }

                var addrRow = mStartAddress + pos * Constants.NBR_OF_BYTES_PER_RAW;

                if (addrRow < 10)                
                    row.AddressStr = $"Addr      {addrRow}: ";                
                else if (addrRow > 10 && addrRow < 100)                
                    row.AddressStr = $"Addr    {addrRow}: ";                
                else if (addrRow >= 100 && addrRow < 1000)                
                    row.AddressStr = $"Addr  {addrRow}: ";                

                row.Position = pos * Constants.NBR_OF_BYTES_PER_RAW;

                list.Add(row);
            }

            return list;
        }

        public async Task UpdateMemoryRowAsync(int mStartAddress, TagMemoryRow row)
        {
            try
            {
                row.Bytes[0] = Convert.ToSByte(row.Byte1Hex, 16);
                row.Byte1Char = this.GetChar(row.Bytes[0]);

                row.Bytes[1] = Convert.ToSByte(row.Byte2Hex, 16);
                row.Byte2Char = this.GetChar(row.Bytes[1]);

                row.Bytes[2] = Convert.ToSByte(row.Byte3Hex, 16);
                row.Byte3Char = this.GetChar(row.Bytes[2]);

                row.Bytes[3] = Convert.ToSByte(row.Byte4Hex, 16);
                row.Byte4Char = this.GetChar(row.Bytes[3]);

                var nfcTag = TagListenerDroid.TagInfoDroid.NfcTag;
                if (nfcTag is Type5Tag)
                {
                    var mAreaId = await Task.Run(() => getAreaIdFromAddressInBytesForType5Tag(nfcTag, mStartAddress));

                    if (mAreaId == -1)
                    {
                        // An issue occured retrieving AreaId from Address
                        // Address is probably invalid                        
                        TagListenerDroid.ShowBlackToast("Invalid AreaId value!");
                    }
                    else
                    {
                        // Type 5                    
                        var bytes = row.Bytes.Select(b => (byte)b).ToArray();
                        await Task.Run(() => nfcTag.WriteBytes(mStartAddress + row.Position, bytes));
                    }
                }
            }
            catch (STException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                TagListenerDroid.ShowBlackToast(e.Error.ToString());                
            }
        }

        public async Task ClearMemoryAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var nfcTag = TagListenerDroid.TagInfoDroid.NfcTag;
                    var emptyByteArray = new byte[nfcTag.MemSizeInBytes];
                    nfcTag.WriteBytes(0, emptyByteArray);

                    InvalidateCache(nfcTag);
                });                
            }
            catch(STException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                TagListenerDroid.ShowBlackToast(e.Error.ToString());
            }
        }

        private async Task<byte[]> ReadRangeAsync(int mStartAddress, int mNumberOfBytes)
        {
            try
            {
                var nfcTag = TagListenerDroid.TagInfoDroid.NfcTag;
                if (nfcTag is Type5Tag)
                {
                    var mAreaId = await Task.Run(() => getAreaIdFromAddressInBytesForType5Tag(nfcTag, mStartAddress));

                    if (mAreaId == -1)
                    {
                        // An issue occured retrieving AreaId from Address
                        // Address is probably invalid
                        TagListenerDroid.ShowBlackToast("Invalid AreaId value!");
                    }
                    else
                    {
                        // Type 5
                        var mBuffer = await Task.Run(() => nfcTag.ReadBytes(mStartAddress, mNumberOfBytes));
                        // Warning: readBytes() may return less bytes than requested                   
                        if (mBuffer != null && mBuffer.Length != mNumberOfBytes)
                        {
                            TagListenerDroid.ShowBlackToast("error_during_read_operation");
                        }

                        return mBuffer;
                    }
                }
            }
            catch (STException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                TagListenerDroid.ShowBlackToast(e.Error.ToString());
            }

            return null;
        }

        private int getAreaIdFromAddressInBytesForType5Tag(NFCTag nfcTag, int mStartAddress)
        {
            int ret = -1;
            if (nfcTag is STType5MultiAreaTag && nfcTag is STType5Tag) {
                var tag = nfcTag as STType5MultiAreaTag;
                try
                {
                    ret = tag.GetAreaFromByteAddress(mStartAddress);
                }
                catch
                {
                    ret = -1;
                }
            }
            return ret;
        }

        private char GetChar(sbyte myByte)
        {
            char myChar = ' ';

            if (myByte > 0x20)
            {
                myChar = (char)(myByte & 0xFF);
            }

            return myChar;
        }

        public void InvalidateCache(NFCTag tag)
        {
            if (tag is Type4Tag)
            {
                var type4Tag = (Type4Tag)tag;
                type4Tag.InvalidateCache();
            }
            else if (tag is Type5Tag)
            {
                var type5Tag = (Type5Tag)tag;
                type5Tag.InvalidateCache();
            }
            else if (tag is Type2Tag)
            {
                var type2Tag = (Type2Tag)tag;
                type2Tag.InvalidateCache();
            }
        }
    }
}