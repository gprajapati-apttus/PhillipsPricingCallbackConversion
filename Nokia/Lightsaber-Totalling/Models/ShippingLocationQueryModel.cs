using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class ShippingLocationQueryModel
    {
        public string Id { get; set; }
        public decimal? Min_Maint_EUR__c { get; set; }
        public decimal? Min_Maint_USD__c { get; set; }
        public string Quote_Type__c { get; set; }
        public string Maintenance_Type__c { get; set; }
        public string Portfolio__c { get; set; }
        public decimal? LEO_Mini_Maint_EUR__c { get; set; }
        public decimal? LEO_Mini_Maint_USD__c { get; set; }
        public string Pricing_Cluster__c { get; set; }
    }
}
