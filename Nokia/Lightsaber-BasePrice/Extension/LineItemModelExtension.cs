using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public static class LineItemModelExtension
    {
        public static bool? is_Custom_Product__c(this LineItemModel lineItem)
        {
            return lineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c);
        }

        public static bool? Advanced_pricing_done__c(this LineItemModel lineItem)
        {
            return lineItem.Get<bool?>(LineItemCustomField.Advanced_pricing_done__c);
        }

        public static string Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c(this LineItemModel lineItem)
        {
            return lineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c);
        }
    }
}
