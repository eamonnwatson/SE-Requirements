using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.ParseData
{
    public class SEData
    {
        public List<Ore> Ores { get; set; }
        public List<Ingot> Ingots { get; set; }
        public List<Component> Components { get; set; }
        public List<Block> Blocks { get; set; }
    }
}
