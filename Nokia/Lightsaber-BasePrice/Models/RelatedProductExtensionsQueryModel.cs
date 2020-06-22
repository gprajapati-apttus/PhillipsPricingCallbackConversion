using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class ProductExtensionsQueryModel
    {
        public string Id { get; set; }

        public string Product__c { get; set; }

        public decimal? Market_Price__c { get; set; }

        public decimal? Floor_Price__c { get; set; }

        public bool? Custom_Bid__c { get; set; }
    }
}
