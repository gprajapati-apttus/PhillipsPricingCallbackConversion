using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class MaintenanceAndSSPRuleQueryModel
    {
        public string Id { get; set; }

        public string Region__c { get; set; }

        public string Maintenance_Level__c { get; set; }

        public string Maintenance_Type__c { get; set; }

        public string Maintenance_Category__c { get; set; }

        public decimal? Service_Rate_Y1__c { get; set; }

        public decimal? Service_Rate_Y2__c { get; set; }

        public decimal? Biennial_SSP_Discount__c { get; set; }

        public decimal? Unlimited_SSP_Discount__c { get; set; }

        public decimal? Biennial_SRS_Discount__c { get; set; }

        public decimal? Unlimited_SRS_Discount__c { get; set; }
    }
}
