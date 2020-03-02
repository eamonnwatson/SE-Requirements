using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SELibrary.Modals
{
    public class Block : BaseItem, IManufacturedItem
    {
        public List<ItemGroup> Components { get; set; }
        public int? PCU { get; set; }
        public CubeSize CubeSize { get; set; }
        public Block TopPart { get; set; }
        public Block RotorPart { get; set; }
        public decimal? MaterialEfficiency { get; set; }
        public List<Ore> RefineableOreTypes { get; set; }

        public IEnumerable<ItemGroup> GetComponents()
        {
            return Components;
        }

        public IEnumerable<ItemGroup> GetIngots()
        {
            var output = new List<ItemGroup>();
            foreach (var item in Components)
            {
                var comp = item.Item as IManufacturedItem;
                var ingots = comp.GetIngots();
                foreach (var ingot in ingots)
                {
                    output.Add(new ItemGroup() { Item = ingot.Item, Quantity = ingot.Quantity * item.Quantity });
                }
            }
            return output;
        }
    }
}
