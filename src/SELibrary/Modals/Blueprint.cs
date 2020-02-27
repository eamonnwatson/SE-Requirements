using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    class Blueprint
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }

        public IEnumerable<Item> Prerequisites { get; set; }
        public IEnumerable<Item> Results { get; set; }
        public decimal BaseProductionTimeInSeconds { get; set; }
    }
}
