using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    public class ItemGroup
    {
        public decimal Quantity { get; internal set; }
        public BaseItem Item { get; internal set; }

        public override string ToString()
        {
            return $"{ Quantity } - { Item }";
        }
    }
}
