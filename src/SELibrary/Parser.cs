using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;
using SELibrary.Modals;

namespace SELibrary
{
    public class Parser
    {
        private const string SE_FOLDER = @"e:\Games\SteamLibrary\steamapps\common\SpaceEngineers\Content\";
        private string xmlBlockCategories;
        private string xmlBlueprintClasses;
        private string xmlBlueprints;
        private IList<string> xmlBlocks = new List<string>();
        private IDictionary<string, string> displayNames;
        private IDictionary<string, Blueprint> bps;
        private IDictionary<string, CubeBlock> cbs;
        public Parser()
        {
            LoadFiles();
            displayNames = GetDisplayNames();
            bps = ParseBlueprints();
            cbs = ParseCubeBlocks();
        }

        private IDictionary<string, CubeBlock> ParseCubeBlocks()
        {
            var cbs = new List<CubeBlock>();
            foreach (var xmlFile in xmlBlocks)
            {
                var doc = GetDocument(xmlFile);
                var nodes = doc.GetElementsByTagName("Definition");

                foreach (XmlNode node in nodes)
                {
                    var cb = new CubeBlock()
                    {
                        ID = node.SelectSingleNode("Id/TypeId").InnerText + "/" + node.SelectSingleNode("Id /SubtypeId").InnerText,
                        DisplayName = SafeGetValue(displayNames, node.SelectSingleNode("DisplayName")?.InnerText),
                        Icon = node.SelectSingleNode("Icon")?.InnerText,
                        Description = SafeGetValue(displayNames, node.SelectSingleNode("Description")?.InnerText),
                        CubeSize = node.SelectSingleNode("CubeSize")?.InnerText,
                        Public = node.SelectSingleNode("Public")?.InnerText,
                        PCU = Convert.ToInt32(node.SelectSingleNode("PCU")?.InnerText),
                    };

                    var xmlComponents = node.SelectSingleNode("Components").ChildNodes;
                    cb.Components = GetComponents(xmlComponents);

                    cbs.Add(cb);
                }
            }

            cbs = cbs.Where(a => a.Public != "false").ToList();

            return cbs.ToDictionary(a => a.ID);
        }

        private IEnumerable<Component> GetComponents(XmlNodeList xmlNodes)
        {
            var output = new List<Component>();
            foreach (XmlNode xmlItem in xmlNodes)
            {
                if (xmlItem.Name != "Component")
                    continue;

                var comp = new Component()
                {
                    Count = Convert.ToInt32(xmlItem.Attributes["Count"].Value),
                    SubType = xmlItem.Attributes["Subtype"].Value,
                };
                output.Add(comp);
            }
            return output;
        }

        private IDictionary<string, Blueprint> ParseBlueprints()
        {
            var bps = new List<Blueprint>();
            var doc = GetDocument(xmlBlueprints);
            var nodes = doc.GetElementsByTagName("Blueprint");

            foreach (XmlNode item in nodes)
            {
                var bp = new Blueprint()
                {
                    ID = item.SelectSingleNode("Id/SubtypeId").InnerText,
                    BaseProductionTimeInSeconds = Convert.ToDecimal(item.SelectSingleNode("BaseProductionTimeInSeconds").InnerText),
                    DisplayName = SafeGetValue(displayNames, item.SelectSingleNode("DisplayName").InnerText),
                    Icon = item.SelectSingleNode("Icon").InnerText,
                };
                var xmlResults = item.SelectSingleNode("Results")?.ChildNodes;
                if (xmlResults == null)
                {
                    var result = item.SelectSingleNode("Result");
                    bp.Results = new List<Item>() { GetItem(result) };
                }
                else
                {
                    bp.Results = GetItems(xmlResults);
                }
               
                var xmlPrerequisites = item.SelectSingleNode("Prerequisites").ChildNodes;
                bp.Prerequisites = GetItems(xmlPrerequisites);

                bps.Add(bp);
            }

            return bps.ToDictionary(a => a.ID);
        }

        private static IEnumerable<Item> GetItems(XmlNodeList xmlNodes)
        {
            var output = new List<Item>();
            foreach (XmlNode xmlItem in xmlNodes)
            {
                output.Add(GetItem(xmlItem));
            }
            return output;
        }

        private static Item GetItem(XmlNode xmlItem)
        {
            return new Item()
            {
                Amount = Convert.ToDecimal(xmlItem.Attributes["Amount"].Value),
                ID = xmlItem.Attributes["TypeId"].Value,
                SubTypeID = xmlItem.Attributes["SubtypeId"].Value,
            };
        }

        private string SafeGetValue(IDictionary<string, string> displayNames, string key)
        {
            if (key == null)
                return string.Empty;

            if (displayNames.TryGetValue(key, out string value))
                return value;

            return string.Empty;
        }

        private XmlDocument GetDocument(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        private void LoadFiles()
        {
            xmlBlockCategories = File.ReadAllText(SE_FOLDER + @"\data\BlockCategories.sbc");
            xmlBlueprintClasses = File.ReadAllText(SE_FOLDER + @"\data\BlueprintClasses.sbc");
            xmlBlueprints = File.ReadAllText(SE_FOLDER + @"\data\Blueprints.sbc");
            var files = Directory.GetFiles(SE_FOLDER + @"\data\CubeBlocks", "*.*");

            foreach (var file in files)
            {
                xmlBlocks.Add(File.ReadAllText(file));
            }
        }



        private IDictionary<string, string> GetDisplayNames()
        {
            var output = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var doc = new XmlDocument();
            doc.Load(SE_FOLDER + @"/data/Localization/MyTexts.resx");
            var nodes = doc.GetElementsByTagName("data");
            foreach (XmlNode item in nodes)
            {
                var key = item.Attributes["name"].Value;
                var value = item.ChildNodes.Cast<XmlNode>().Where(n => n.Name.Equals("value")).FirstOrDefault()?.InnerText;

                output.Add(key, value);
            }

            return output;
        }
    }
}
