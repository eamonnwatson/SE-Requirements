using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using SELibrary.ParseData;

namespace SELibrary
{
    class Parser
    {
        internal List<Ore> Ores { get; private set; }
        internal List<Ingot> Ingots { get; private set; }
        internal List<Component> Components { get; private set; }
        internal List<Block> Blocks { get; private set; }

        internal Parser()
        {
            var ser = new XmlSerializer(typeof(SEData));
            var fs = new StreamReader(@"c:\Users\Eamonn\source\repos\SE-Requirements\XML\SEData.xml");
            var data = (SEData)ser.Deserialize(fs);
            fs.Close();

            Ores = data.Ores;
            Ingots = data.Ingots;
            Components = data.Components;
            Blocks = data.Blocks;
        }
    }
}
