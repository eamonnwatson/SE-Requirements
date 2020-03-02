using SELibrary.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SELibrary
{
    public class Resources
    {
        private readonly Parser parser;
        private readonly Mapper mapper;

        public IReadOnlyDictionary<string, Ore> Ores { get; private set; }
        public IReadOnlyDictionary<string, Ingot> Ingots { get; private set; }
        public IReadOnlyDictionary<string, Component> Components { get; private set; }
        public IReadOnlyDictionary<string, Block> Blocks { get; private set; }
        public Resources()
        {
            parser = new Parser();
            mapper = new Mapper();

            Ingots = mapper.MapIngots(parser.Ingots);
            Ores = mapper.MapOres(parser.Ores, Ingots);
            Components = mapper.MapComponents(parser.Components, Ingots);
            Blocks = mapper.MapBlocks(parser.Blocks, Components, Ores);
        }

        public IEnumerable<ItemGroup> CalculateIngotRequirements(IEnumerable<IManufacturedItem> items)
        {
            var output = Ingots.Values.Select(a => new ItemGroup() { Item = a, Quantity = 0 }).ToList();

            foreach (var item in items)
            {
                var result = item.GetIngots();
                CombineItemGroup(output, result);
            }

            return output.Where(a => a.Quantity > 0);
        }

        public IEnumerable<ItemGroup> CalculateComponentRequirements(IEnumerable<IManufacturedItem> items)
        {
            var output = Components.Values.Select(a => new ItemGroup() { Item = a, Quantity = 0 }).ToList();

            foreach (var item in items)
            {
                var result = item.GetComponents();
                CombineItemGroup(output, result);
            }

            return output.Where(a => a.Quantity > 0);
        }

        public IEnumerable<ItemGroup> CalculateOreRequirements(IEnumerable<ItemGroup> ingots, Block refinery, bool fullYield = true)
        {
            if (refinery.RefineableOreTypes.Count == 0)
                throw new ApplicationException("Please pass a refinery in");

            var ores = refinery.RefineableOreTypes.Where(a => a.ID != "Stone");
            var efficiency = refinery.MaterialEfficiency ?? 1;
            var yieldEfficiency = fullYield ? 2 : 1;

            foreach (var item in ingots)
            {
                if (ores.Count(a => a.IngotResults.Count(b => b.Item == item.Item) > 0) == 0)
                    throw new ApplicationException("Refinery is unable to create :" + item.Item.Name);
            }

            var output = ores.Select(a => new ItemGroup() { Item = a, Quantity = 0 }).ToList();

            foreach (var item in ingots)
            {
                var ore = output.FirstOrDefault(a => OreMakesIngot(a, item.Item));
                if (ore == null)
                    continue;

                var orePrereq = ((Ore)ore.Item).PrerequisiteAmount;
                var ingot = ((Ore)ore.Item).IngotResults.FirstOrDefault(a => a.Item == item.Item);

                var oreToMake = item.Quantity / efficiency / yieldEfficiency / ingot.Quantity * orePrereq;
                ore.Quantity = decimal.Round(oreToMake, 0, MidpointRounding.AwayFromZero);
            }

            return output.Where(a => a.Quantity > 0);
        }

        public ItemGroup CalculateStoneRequirements(IEnumerable<ItemGroup> ingots, Block refinery)
        {
            if (refinery.RefineableOreTypes.Count == 0)
                throw new ApplicationException("Please pass a refinery in");

            var ore = refinery.RefineableOreTypes.FirstOrDefault(a => a.Name.Contains("Stone"));
            if (ore == null)
                throw new ApplicationException("Refinery does not make stone");

            var efficiency = refinery.MaterialEfficiency ?? 1;

            var output = new ItemGroup() { Quantity = 0, Item = ore };

            var orePrereq = ore.PrerequisiteAmount;
            var ir = ore.IngotResults;

            foreach (var item in ingots)
            {
                var result = ir.FirstOrDefault(a => a.Item == item.Item);
                if (result == null)
                    throw new ApplicationException($"Unable to make { item.Item.Name } with Stone");


                var oreToMake = item.Quantity / efficiency / result.Quantity * orePrereq;

                output.Quantity = Math.Max(output.Quantity, decimal.Round(oreToMake, 0, MidpointRounding.AwayFromZero));

            }

            return output;
        }

        private bool OreMakesIngot(ItemGroup ore, BaseItem ingot)
        {
            var o = (Ore)ore.Item;

            return o.IngotResults.Select(a => a.Item).Contains(ingot);
        }

        private void CombineItemGroup(List<ItemGroup> output, IEnumerable<ItemGroup> toAdd)
        {
            foreach (var item in toAdd)
            {
                var orig = output.FirstOrDefault(a => a.Item == item.Item);
                if (orig == null)
                {
                    output.Add(item);
                }
                else
                {
                    orig.Quantity += item.Quantity;
                }
            }

        }


    }
}
