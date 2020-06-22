using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class NokiaMaintenanceAndSSPRulesQueryModel
    {
        public string Id { get; set; }

        public bool? NokiaCPQ_withPMA__c { get; set; }

        public string NokiaCPQ_Pricing_Cluster__c { get; set; }

        public string NokiaCPQ_Product_Discount_Category__c { get; set; }

        public decimal? NokiaCPQ_Product_Discount_Category_per__c { get; set; }

        public decimal? NokiaCPQ_Unlimited_SSP_Discount__c { get; set; }

        public decimal? NokiaCPQ_Biennial_SSP_Discount__c { get; set; }

        public string NokiaCPQ_Maintenance_Level__c { get; set; }

        public string NokiaCPQ_Maintenance_Type__c { get; set; }

        public decimal? NokiaCPQ_Service_Rate_Y1__c { get; set; }

        public decimal? NokiaCPQ_Service_Rate_Y2__c { get; set; }
    }
}
