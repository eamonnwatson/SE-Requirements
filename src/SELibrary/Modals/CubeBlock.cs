using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    class CubeBlock
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string CubeSize { get; set; }
        public string Public { get; set; }
        public string TopPart { get; set; }
        public decimal MaterialEfficiency { get; set; }
        public IList<ParseComponent> Components { get; } = new List<ParseComponent>();
        public IList<BlueprintClass> BlueprintClasses { get; } = new List<BlueprintClass>();
        public int PCU { get; set; }
    }
}
