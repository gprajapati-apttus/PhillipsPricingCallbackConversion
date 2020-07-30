using System.Collections.Generic;

namespace Apttus.Lightsaber.Phillips.Common
{
    class Constants
    {
        public const string PROPOSAL = "Apttus_Proposal__Proposal__c";
        public const string PROPOSAL_CONFIG_RELATIONSHIP_NAME = "Apttus_QPConfig__Proposald__r";

        public const string PRICE_OVERRIDE = "Price Override";
        public const string DISCOUNT_AMOUNT = "Discount Amount";
        public const string ONE_TIME = "One Time";
        public const string DISCOUNT_PERCENTAGE = "Discount %";
        public const string CONFIGURATIONTYPE_BUNDLE = "Bundle";
        public const string CONFIGURATIONTYPE_OPTION = "Option";
        public const string SOLUTION_SUBSCRIPTION = "Solution Subscription";
        public const string BILLING_PLAN_ANNUAL = "Annual";
        public const string BILLING_PLAN_MONTHLY = "Monthly";
        public const string BILLING_PLAN_QUATERLY = "Quarterly";
        public const string SERVICE_PRODUCT_TYPE = "Service";

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
