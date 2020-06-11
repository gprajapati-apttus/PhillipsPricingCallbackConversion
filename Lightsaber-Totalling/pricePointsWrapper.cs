using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using PhillipsConversion.Lightsaber;
using PhillipsConversion.Totalling;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace PhillipsConversion.Totalling
{
    public class pricePointsWrapper
    {
        public decimal? targetPrice;
        public decimal? minPrice;
        public decimal? listPrice;
        public decimal? preEscalationPrice;
        public decimal? optionPrice;
        public decimal? bundleExtendedPrice;
        public decimal? contractNetPrice;
        public decimal? offeredPrice;
        public decimal? qty;
        public decimal? bundleBaseExtendedPrice;
        public decimal? sellingTerm;
        public decimal? incentiveAdjAmount;
        public decimal? solutionUnitIncentiveAmount;
        public decimal? sapListPrice;
        public decimal? unitStrategicDiscountAmount = 0;
        public decimal? pliCountryPriceListPrice;
        public decimal? philipsListPrice;
        public decimal? contractDiscountAmount;
        public decimal? solutionContractDiscountAmount;

        public pricePointsWrapper(LineItemModel lineItemModel)
        {
            listPrice = 0;
            solutionUnitIncentiveAmount = 0;
            sellingTerm = lineItemModel.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
            preEscalationPrice = 0;
            targetPrice = 0;
            minPrice = 0;

            if (!string.IsNullOrWhiteSpace(lineItemModel.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListId__r_Apttus_Config2__ContractNumber__c)))
            {
                contractDiscountAmount = lineItemModel.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).HasValue 
                    && lineItemModel.Get<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__ListPrice__c).HasValue 
                    && lineItemModel.Get<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__ListPrice__c).Value != 0
                        ? (lineItemModel.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).Value / 100) * lineItemModel.Get<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__ListPrice__c).Value * lineItemModel.GetQuantity() 
                        : 0;

                solutionContractDiscountAmount = !lineItemModel.IsOptional() 
                    && lineItemModel.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).HasValue 
                    && lineItemModel.Get<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__ListPrice__c).HasValue 
                        ? (lineItemModel.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).Value / 100) * lineItemModel.Get<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__ListPrice__c).Value * lineItemModel.GetQuantity() 
                        : 0;
            }

            pliCountryPriceListPrice = lineItemModel.Get<decimal?>(LineItemCustomField.Apttus_Config2__PriceListItemId__r_APTS_Country_Pricelist_List_Price__c).HasValue 
                && lineItemModel.Get<decimal?>(LineItemCustomField.Apttus_Config2__PriceListItemId__r_APTS_Country_Pricelist_List_Price__c).Value > 0 
                ? lineItemModel.Get<decimal?>(LineItemCustomField.Apttus_Config2__PriceListItemId__r_APTS_Country_Pricelist_List_Price__c).Value 
                : 0;
            
            philipsListPrice = lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Item_List_Price__c).HasValue ? PricingHelper.ApplyRounding(lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Item_List_Price__c).Value, 2, RoundingMode.UP) : 0;
            sapListPrice = philipsListPrice;
            qty = lineItemModel.GetQuantity();
            
            if (philipsListPrice != null && (pliCountryPriceListPrice != null && pliCountryPriceListPrice > 0))
                contractNetPrice = lineItemModel.Entity.ListPrice.HasValue ? qty * PricingHelper.ApplyRounding(lineItemModel.Entity.ListPrice.Value, 2, RoundingMode.UP) : 0;
            else
                contractNetPrice = 0;

            optionPrice = 0;
            offeredPrice = 0;
            bundleBaseExtendedPrice = lineItemModel.Entity.BasePrice.HasValue ? PricingHelper.ApplyRounding(lineItemModel.Entity.BasePrice.Value, 2, RoundingMode.UP) * qty * sellingTerm : 0;

            bundleExtendedPrice = bundleBaseExtendedPrice;

            if (solutionContractDiscountAmount == null) solutionContractDiscountAmount = 0;
            incentiveAdjAmount = lineItemModel.Entity.IncentiveAdjustmentAmount.HasValue && !lineItemModel.IsOptional() 
                                            ? PricingHelper.ApplyRounding(lineItemModel.Entity.IncentiveAdjustmentAmount.Value, 2, RoundingMode.UP) * -1 
                                            : 0;

            if (incentiveAdjAmount < 0)
            {
                incentiveAdjAmount = incentiveAdjAmount * -1;
                if (lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c).HasValue && lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c).Value > 0)
                    solutionUnitIncentiveAmount = lineItemModel.Get<decimal?>(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c).Value * -1;
            }
        }
    }
}
