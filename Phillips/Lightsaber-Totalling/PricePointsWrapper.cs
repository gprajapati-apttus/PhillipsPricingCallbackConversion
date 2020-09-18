using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Constants;
using LineItemPropertyNames = Apttus.Lightsaber.Pricing.Common.Entities.LineItem.PropertyNames;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PricePointsWrapper
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

        public PricePointsWrapper(LineItem lineItem, PriceListItemQueryModel pliQueryModel, IPricingHelper pricingHelper)
        {
            var priceListItem = lineItem.GetPriceListItem();

            listPrice = 0;
            solutionUnitIncentiveAmount = 0;
            sellingTerm = lineItem.GetValuetOrDefault(LineItemPropertyNames.SellingTerm, 1);
            preEscalationPrice = 0;
            targetPrice = 0;
            minPrice = 0;

            if (!string.IsNullOrWhiteSpace(pliQueryModel.Apttus_Config2__PriceListId__r.Apttus_Config2__ContractNumber__c))
            {
                contractDiscountAmount = lineItem.APTS_ContractDiscount__c.HasValue 
                    && priceListItem.Entity.ListPrice.HasValue 
                    && priceListItem.Entity.ListPrice.Value != 0
                        ? (lineItem.APTS_ContractDiscount__c.Value / 100) * priceListItem.Entity.ListPrice.Value * lineItem.GetQuantity() 
                        : 0;

                solutionContractDiscountAmount = lineItem.IsOptional == false 
                    && lineItem.APTS_ContractDiscount__c.HasValue 
                    && priceListItem.Entity.ListPrice.HasValue 
                        ? (lineItem.APTS_ContractDiscount__c.Value / 100) * priceListItem.Entity.ListPrice.Value * lineItem.GetQuantity() 
                        : 0;
            }

            pliCountryPriceListPrice = pliQueryModel.APTS_Country_Pricelist_List_Price__c.HasValue
                && pliQueryModel.APTS_Country_Pricelist_List_Price__c.Value > 0 
                ? pliQueryModel.APTS_Country_Pricelist_List_Price__c.Value 
                : 0;
            
            philipsListPrice = lineItem.APTS_Item_List_Price__c.HasValue ? pricingHelper.ApplyRounding(lineItem.APTS_Item_List_Price__c.Value, 2, RoundingMode.UP) : 0;
            sapListPrice = philipsListPrice;
            qty = lineItem.GetQuantity();
            
            if (philipsListPrice != null && (pliCountryPriceListPrice != null && pliCountryPriceListPrice > 0))
                contractNetPrice = lineItem.ListPrice.HasValue ? qty * pricingHelper.ApplyRounding(lineItem.ListPrice.Value, 2, RoundingMode.UP) : 0;
            else
                contractNetPrice = 0;

            optionPrice = 0;
            offeredPrice = 0;
            bundleBaseExtendedPrice = lineItem.BasePrice.HasValue ? pricingHelper.ApplyRounding(lineItem.BasePrice.Value, 2, RoundingMode.UP) * qty * sellingTerm : 0;

            bundleExtendedPrice = bundleBaseExtendedPrice;

            if (solutionContractDiscountAmount == null) solutionContractDiscountAmount = 0;
            incentiveAdjAmount = lineItem.IncentiveAdjustmentAmount.HasValue && lineItem.IsOptional == false 
                                            ? pricingHelper.ApplyRounding(lineItem.IncentiveAdjustmentAmount.Value, 2, RoundingMode.UP) * -1 
                                            : 0;

            if (incentiveAdjAmount < 0)
            {
                incentiveAdjAmount = incentiveAdjAmount * -1;
                if (lineItem.APTS_Unit_Incentive_Adj_Amount__c.HasValue && lineItem.APTS_Unit_Incentive_Adj_Amount__c.Value > 0)
                    solutionUnitIncentiveAmount = lineItem.APTS_Unit_Incentive_Adj_Amount__c.Value * -1;
            }
        }
    }
}
