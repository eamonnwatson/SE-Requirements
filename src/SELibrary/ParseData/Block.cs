using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.ParseData
{
    public class Block : BaseItem
    {
        public List<ItemGroup> Components { get; set; }
        public List<string> Categories { get; set; }
        public int? PCU { get; set; }
        public string CubeSize { get; set; }
        public string TopPart { get; set; }
        public string RotorPart { get; set; }
        public decimal? MaterialEfficiency { get; set; }
        public string RefineableOreType { get; set; }
    }
}
