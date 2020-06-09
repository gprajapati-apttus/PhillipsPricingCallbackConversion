using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Lightsaber
{
    class Constants
    {
        public const string SYSTEM_TYPE_DEMO = "Demo";
        public const string SYSTEM_TYPE_3RD_PARTY = "3rd Party";
        public const string SYSTEM_TYPE_PHILIPS = "Philips";
        public const string SYSTEM_TYPE_SERVICE = "Service";

        public const string TIER_1 = "Tier 1";
        public const string TIER_2 = "Tier 2";
        public const string TIER_3 = "Tier 3";
        public const string TIER_4 = "Tier 4";

        public static readonly List<string> listTradeSpoo = new List<string> { "Trade-In", "Trade-In PO", "Trade-In Return" };
    }
}
