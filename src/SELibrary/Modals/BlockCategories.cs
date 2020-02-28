using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    class BlockCategories
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IList<string> Items { get; } = new List<string>();
    }
}
