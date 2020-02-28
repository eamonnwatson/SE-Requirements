using System.Collections.Generic;

namespace SELibrary.Modals
{
    public interface IManufacturedItem
    {
        IEnumerable<(int, Ingot)> CalculateIngots();
    }
}