using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    public class Block : BaseItem, IManufacturedItem
    {
        public IEnumerable<(decimal, Component)> Components { get; internal set; }       
        internal Block() { }
        public IEnumerable<(int, Ingot)> CalculateIngots()
        {
            throw new NotImplementedException();
        }
    }
}
