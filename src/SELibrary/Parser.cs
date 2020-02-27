using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

namespace SELibrary
{
    public class Parser
    {
        public void GetData()
        {
            var displayNames = GetDisplayNames();
            var doc = new XmlDocument();
            doc.Load("..//..//..//..//..//data//BlueprintClasses.sbc");


        }

        private Dictionary<string, string> GetDisplayNames()
        {
            var output = new Dictionary<string, string>();
            var doc = new XmlDocument();
            doc.Load("..//..//..//..//..//data//Localization//MyTexts.resx");
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
