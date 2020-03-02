using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    public interface IManufacturedItem
    {
        IEnumerable<ItemGroup> GetIngots();
        IEnumerable<ItemGroup> GetComponents();
    }
}
