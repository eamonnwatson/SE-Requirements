using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary.ParseData
{
    public abstract class BaseItem
    {
        public string Name { get; set; }
        public string TypeID { get; set; }
        public string SubTypeID { get; set; }
    }
}
