using SELibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using SELibrary.Modals;
using System.Xml;

namespace SEConsole
{
    class Program
    {
        private static Resources resources;
        static void Main(string[] args)
        {
            resources = new Resources();
            var order = new List<IManufacturedItem>();

            var refinery = resources.Blocks["Refinery/LargeRefinery"];
            var basicRefinery = resources.Blocks["Refinery/Blast Furnace"];
            var survivalKit = resources.Blocks["SurvivalKit/SurvivalKit"];

            //Refinery/LargeRefinery
            //Refinery/Blast Furnace
            //SurvivalKit/SurvivalKit

            //order.AddRange(ProcessSBC(@"c:\Users\Eamonn\Dropbox\New Moon Driller.sbc"));

            //order.Add(resources.Blocks["ShipConnector/Connector"]);
            //order.Add(resources.Blocks["Assembler/LargeAssembler"]);

            OutputComponents(order);
            OutputIngots(order);
            OutputOres(order, basicRefinery, false);
            //OutputStone(order, survivalKit);






        }

        static void OutputStone(List<IManufacturedItem> order, Block refinery)
        {
            var ingots = resources.CalculateIngotRequirements(order);
            var stone = resources.CalculateStoneRequirements(ingots, refinery);
            Console.WriteLine($"{stone.Item.Name,30}:{stone.Quantity}");
        }

        static void OutputOres(List<IManufacturedItem> order, Block refinery, bool yield = true)
        {
            var ingots = resources.CalculateIngotRequirements(order);
            var ores = resources.CalculateOreRequirements(ingots, refinery, yield);
            Console.WriteLine();
            foreach (var item in ores)
            {
                Console.WriteLine($"{item.Item.Name,30}:{item.Quantity}");
            }

            Console.WriteLine();
        }
        static void OutputComponents(List<IManufacturedItem> order)
        {
            var results = resources.CalculateComponentRequirements(order);

            foreach (var item in results)
            {
                Console.WriteLine($"{item.Item.Name,30}:{item.Quantity}");
            }
            Console.WriteLine();
        }
        static void OutputIngots(List<IManufacturedItem> order)
        {
            var results = resources.CalculateIngotRequirements(order);

            foreach (var item in results)
            {
                Console.WriteLine($"{item.Item.Name,30}:{item.Quantity}");
            }
            Console.WriteLine();
        }
        private static IEnumerable<Block> ProcessSBC(string filename)
        {
            var output = new List<Block>();
            var items = GetSBC(filename);
            foreach (var item in items)
            {
                var block = resources.Blocks.Values.Where(a => a.ID.Contains(item)).FirstOrDefault();
                if (block != null)
                    output.Add(block);
            }

            return output;

        }

        static IEnumerable<string> GetSBC(string filename)
        {
            var doc = new XmlDocument();
            doc.Load(filename);

            var items = doc.GetElementsByTagName("SubtypeName");

            return items.Cast<XmlNode>().Select(a => a.InnerText).Where(a => !string.IsNullOrWhiteSpace(a));
        }
    }
}
