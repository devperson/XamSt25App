using System;
using System.Collections.Generic;
using System.Text;

namespace St25App.Models
{
    public class TagInfo
    {
        public string mUID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Manufacturer { get; set; }
        public int SizeInBytes { get; set; }
        public List<string> TechList { get; set; }
        public string TechListStr
        {
            get
            {
                if (TechList != null)
                {
                    return string.Join("\n", TechList);
                }

                return null;
            }
        }


        public override string ToString()
        {
            var str = $"Name={Name}; Desc:{Description}; Type:{Type}; Manufacture:{Manufacturer}; mUId:{mUID}; TagSize:{SizeInBytes}; TechList:{TechListStr};";
            return str;
        }
    }
}
