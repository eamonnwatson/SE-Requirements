using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.ParseData
{
    public class Ore : BaseItem
    {
        public List<string> Categories { get; set; }
        public decimal PrerequisiteAmount { get; set; }
        public List<ItemGroup> IngotResults { get; set; }
    }
}
