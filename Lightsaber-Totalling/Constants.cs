using System.Collections.Generic;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    class Constants
    {
        public const string PROPOSAL = "Apttus_Proposal__Proposal__c";

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

        public static readonly List<string> listTradeSpoo = new List<string> { "Trade-In", "Trade-In PO", "Trade-In Return" };
    }
}
