﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    public class Component : BaseItem, IManufacturedItem
    {
        public IEnumerable<(decimal, Ingot)> Ingots { get; internal set; }
        internal Component() { }
        public IEnumerable<(int, Ingot)> CalculateIngots()
        {
            throw new NotImplementedException();
        }
    }
}
