using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class CountryPriceListItemQueryModel
    {
        public string Id { get; set; }

        public decimal? Apttus_Config2__ListPrice__c { get; set; }

        public string Apttus_Config2__ProductId__c { get; set; }

        public bool? Apttus_Config2__ProductActive__c { get; set; }

        public decimal? Apttus_Config2__Cost__c { get; set; }

        public CountryPriceListItemProductQueryModel Apttus_Config2__ProductId__r { get; set; }
    }

    public class CountryPriceListItemProductQueryModel
    {
        public string Portfolio__c { get; set; }
    }
}
