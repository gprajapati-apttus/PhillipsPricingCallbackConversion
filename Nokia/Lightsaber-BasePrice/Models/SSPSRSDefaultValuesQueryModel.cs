using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class SSPSRSDefaultValuesQueryModel
    {
        public string Id { get; set; }

        public string Portfolio__c { get; set; }

        public bool? SSP_Visible__c { get; set; }

        public bool? SRS_Visible__c { get; set; }

        public decimal? SRS_Percentage__c { get; set; }

        public bool? Tier_Discount_Applicable__c { get; set; }

        public bool? AccountLevel_Discount_Applicable__c { get; set; }

        public bool? Multi_Year_Discount_Applicable__c { get; set; }
    }
}
