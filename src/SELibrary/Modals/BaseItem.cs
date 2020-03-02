using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.Modals
{
    public class BaseItem
    {
        public string ID { get; internal set; }
        public string Name { get; internal set; }
        public decimal Amount { get; internal set; } = 1;

        public override string ToString()
        {
            return $"{ ID } - { Name }";
        }
    }
}
