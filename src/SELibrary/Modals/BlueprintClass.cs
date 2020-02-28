using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    class BlueprintClass
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }

        public IList<BlueprintClassEntry> Entries { get; } = new List<BlueprintClassEntry>();
    }
}
