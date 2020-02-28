using System;
using System.Collections.Generic;
using System.Text;

namespace SELibrary
{
    public class Resources
    {
        private readonly Parser seParser;
        public Resources()
        {
            seParser = new Parser(@"e:\Games\SteamLibrary\steamapps\common\SpaceEngineers\Content\");
            seParser.ParseSEData();
        }

    }
}
