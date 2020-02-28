using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;
using SELibrary.Modals;

namespace SELibrary
{
    class Parser
    {
        private string se_folder;
        private string xmlBlockCategories;
        private string xmlBlueprintClasses;
        private string xmlBlueprints;
        private IList<string> xmlBlocks = new List<string>();
        private IDictionary<string, string> displayNames;
        private IDictionary<string, Blueprint> bps;
        private IDictionary<string, CubeBlock> cbs;
        private IDictionary<string, BlueprintClass> bpc;
        private IDictionary<string, BlockCategories> cat;

        private const string BLOCK_CATEGORIES_FILE = @"\data\BlockCategories.sbc";
        private const string BLUEPRINT_CLASSES_FILE = @"\data\BlueprintClasses.sbc";
        private const string BLUEPRINTS_FILE = @"\data\Blueprints.sbc";
        private const string CUBEBLOCKS_FOLDER = @"\data\CubeBlocks";
        private const string LOCALIZATION_FILE = @"\data\Localization\MyTexts.resx";

        public Parser(string folder)
        {
            se_folder = folder;
        }
        internal void ParseSEData()
        {
            LoadFiles();
            displayNames = GetDisplayNames();
            cat = ParseBlockCategories();
            bpc = ParseBluePrintClasses();
            bps = ParseBlueprints();
            cbs = ParseCubeBlocks();
        }
        private IDictionary<string, BlockCategories> ParseBlockCategories()
        {
            var cat = new List<BlockCategories>();
            var doc = GetDocument(xmlBlockCategories);
            var nodes = doc.GetElementsByTagName("Category");

            foreach (XmlNode node in nodes)
            {
                var newCat = new BlockCategories()
                {
                    Name = node.SelectSingleNode("Name").InnerText,
                    DisplayName = SafeGetValue(displayNames, node.SelectSingleNode("DisplayName")?.InnerText),
                };

                foreach (XmlNode item in node.SelectNodes("ItemIds/string"))
                {
                    newCat.Items.Add(item.InnerText);
                }
                cat.Add(newCat);
            }

            return cat.ToDictionary(a => a.Name);
        }
        private IDictionary<string, BlueprintClass> ParseBluePrintClasses()
        {
            var bpc = new List<BlueprintClass>();
            var doc = GetDocument(xmlBlueprintClasses);
            var bpClasses = doc.GetElementsByTagName("Class");

            foreach (XmlNode node in bpClasses)
            {
                var cls = new BlueprintClass()
                {
                    ID = node.SelectSingleNode("Id/SubtypeId").InnerText,
                    DisplayName = SafeGetValue(displayNames, node.SelectSingleNode("DisplayName")?.InnerText),
                    Icon = node.SelectSingleNode("Icon")?.InnerText,
                };
                bpc.Add(cls);
            }

            var output = bpc.ToDictionary(a => a.ID);

            var bpEntries = doc.GetElementsByTagName("Entry");
            foreach (XmlNode node in bpEntries)
            {
                var entry = new BlueprintClassEntry()
                {
                    ClassName = node.Attributes["Class"].Value,
                    BlueprintSubtypeId = node.Attributes["BlueprintSubtypeId"].Value,
                };

                output[entry.ClassName].Entries.Add(entry);
            }

            return output;
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
                        TopPart = node.SelectSingleNode("TopPart")?.InnerText,
                        MaterialEfficiency = Convert.ToDecimal(node.SelectSingleNode("MaterialEfficiency")?.InnerText),
                    };

                    foreach (XmlNode item in node.SelectNodes("Components/Component"))
                    {
                        var comp = new ParseComponent()
                        {
                            Count = Convert.ToInt32(item.Attributes["Count"].Value),
                            SubType = item.Attributes["Subtype"].Value,
                        };
                        cb.Components.Add(comp);
                    }

                    foreach (XmlNode item in node.SelectNodes("BlueprintClasses/Class"))
                    {
                        cb.BlueprintClasses.Add(bpc[item.InnerText]);
                    }

                    cbs.Add(cb);
                }
            }

            return cbs.ToDictionary(a => a.ID);
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
        private IEnumerable<Item> GetItems(XmlNodeList xmlNodes)
        {
            var output = new List<Item>();
            foreach (XmlNode xmlItem in xmlNodes)
            {
                output.Add(GetItem(xmlItem));
            }
            return output;
        }
        private Item GetItem(XmlNode xmlItem)
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
            xmlBlockCategories = File.ReadAllText(se_folder + BLOCK_CATEGORIES_FILE);
            xmlBlueprintClasses = File.ReadAllText(se_folder + BLUEPRINT_CLASSES_FILE);
            xmlBlueprints = File.ReadAllText(se_folder + BLUEPRINTS_FILE);
            var files = Directory.GetFiles(se_folder + CUBEBLOCKS_FOLDER, "*.*");

            foreach (var file in files)
            {
                xmlBlocks.Add(File.ReadAllText(file));
            }
        }
        private IDictionary<string, string> GetDisplayNames()
        {
            var output = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var doc = new XmlDocument();
            doc.Load(se_folder + LOCALIZATION_FILE);
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
