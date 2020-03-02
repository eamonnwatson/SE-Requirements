using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    public class Component : BaseItem, IManufacturedItem
    {
        public List<ItemGroup> Requirements { get; set; }

        public IEnumerable<ItemGroup> GetComponents()
        {
            return new List<ItemGroup>();
        }

        public IEnumerable<ItemGroup> GetIngots()
        {
            return Requirements;
        }
    }
}
