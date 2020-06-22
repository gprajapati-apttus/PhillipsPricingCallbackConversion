using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class TierDiscountDetailQueryModel
    {
        public string Id { get; set; }

        public string NokiaCPQ_Tier_Type__c { get; set; }

        public string NokiaCPQ_Partner_Type__c { get; set; }

        public string NokiaCPQ_Pricing_Tier__c { get; set; }

        public decimal? NokiaCPQ_Tier_Discount__c { get; set; }

        public string Nokia_CPQ_Partner_Program__c { get; set; }
    }
}
