using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace St25App.Models
{
    public class TagMemoryRow : BindableBase
    {
        public string AddressStr { get; set; }

        public string Byte1Hex { get; set; }
        public string Byte2Hex { get; set; }
        public string Byte3Hex { get; set; }
        public string Byte4Hex { get; set; }

        public char Byte1Char { get; set; }
        public char Byte2Char { get; set; }
        public char Byte3Char { get; set; }
        public char Byte4Char { get; set; }


        public int Position { get; set; }
        public sbyte[] Bytes { get; set; } = new sbyte[Constants.NBR_OF_BYTES_PER_RAW];
    }
}
