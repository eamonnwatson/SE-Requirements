using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SELibrary
{
    class Mapper
    {
        internal Dictionary<string, Modals.Ingot> MapIngots(List<ParseData.Ingot> ingots)
        {
            return ingots.Select(i => new Modals.Ingot() { ID = i.SubTypeID, Name = i.Name }).ToDictionary(a => a.ID);
        }

        internal Dictionary<string, Modals.Ore> MapOres(List<ParseData.Ore> ores, IReadOnlyDictionary<string, Modals.Ingot> ingots)
        {
            return ores.Select(o => new Modals.Ore()
                {
                    ID = o.SubTypeID,
                    Categories = o.Categories,
                    PrerequisiteAmount = o.PrerequisiteAmount,
                    Name = o.Name,
                    IngotResults = o.IngotResults
                        .Select(ir => new Modals.ItemGroup() { Quantity = ir.Amount, Item = ingots[ir.SubTypeID] }).ToList()
                }).ToDictionary(a => a.ID);
        }

        internal Dictionary<string, Modals.Block> MapBlocks(List<ParseData.Block> blocks, 
            IReadOnlyDictionary<string, Modals.Component> components, 
            IReadOnlyDictionary<string, Modals.Ore> ores)
        {

            var output = blocks.Select(b => new Modals.Block()
            {
                ID = b.TypeID + @"/" + b.SubTypeID,
                CubeSize = b.CubeSize == "Large" ? Modals.CubeSize.Large : Modals.CubeSize.Small,
                MaterialEfficiency = b.MaterialEfficiency,
                Name = b.Name,
                PCU = b.PCU,
                Components = b.Components
                    .Select(c => new Modals.ItemGroup() { Quantity = c.Amount, Item = components[c.SubTypeID] }).ToList(),
                RefineableOreTypes = ores.Values.Where(a => a.Categories.Contains(b.RefineableOreType)).ToList(),
            }).ToDictionary(a => a.ID);

            return output;
        }

        internal Dictionary<string, Modals.Component> MapComponents(List<ParseData.Component> components, IReadOnlyDictionary<string, Modals.Ingot> ingots)
        {
            return components.Select(c => new Modals.Component()
            { 
                ID = c.SubTypeID,
                Name = c.Name,
                Requirements = c.IngotRequirements
                    .Select(ir => new Modals.ItemGroup() { Quantity = ir.Amount, Item = ingots[ir.SubTypeID] }).ToList()
            })
                .ToDictionary(a => a.ID);
        }
    }
}
