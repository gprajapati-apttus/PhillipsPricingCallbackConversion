using Apttus.Lightsaber.Extensibility.Library.Interface;
using Apttus.Lightsaber.Extensibility.Library.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public static class QueryHelper
    {
        public static Query GetContractedPriceListItemQuery(HashSet<string> priceListItemIds)
        {
            Query query = new Query();
            query.EntityName = "Apttus_Config2__PriceListItem__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Id", Value = new List<string>(priceListItemIds), ComparisonOperator = ConditionOperator.In},
                };
            query.Fields = new string[] { "Id", "Apttus_Config2__PriceListId__c", "Contracted__c", "Partner_Price__c" };
            return query;
        }

        public static Query GetDefaultExchangeRateQuery(string CurrencyIsoCode)
        {
            Query query = new Query();
            query.EntityName = "CurrencyType";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "IsoCode", Value = CurrencyIsoCode, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "ConversionRate" };
            query.Limit = 1;

            return query;
        }

        public static Query GetPricingGuidanceSettingQuery(string portfolio)
        {
            Query query = new Query();
            query.EntityName = "Pricing_Guidance_Setting__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Name", Value = portfolio, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Name", "Threshold__c" };
            query.Limit = 1;

            return query;
        }

        public static Query GetDirectPortfolioGeneralSettingQuery(string portfolio)
        {
            Query query = new Query();
            query.EntityName = "Direct_Portfolio_General_Setting__mdt";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Portfolio__c", Value = portfolio, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Portfolio__c", "Cost_Calculation_In_PCB__c" };
            query.Limit = 1;

            return query;
        }

        public static Query GetDirectCareCostPercentageQuery(string accountMarket)
        {
            Query query = new Query();
            query.EntityName = "Direct_Care_Cost_Percentage__mdt";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Market__c", Value = accountMarket, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Market__c", "Care_Cost__c" };

            return query;
        }

        public static Query GetShippingLocationForDirectQuoteQuery(string portfolio, string maintainanceType)
        {
            Query query = new Query();
            query.EntityName = "Shipping_Location__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Maintenance_Type__c", Value = maintainanceType, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Portfolio__c", Value = portfolio, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Min_Maint_EUR__c", "Min_Maint_USD__c", "Quote_Type__c", "Maintenance_Type__c", "Portfolio__c" };
            query.Limit = 1;


            return query;
        }

        public static Query GetShippingLocationForIndirectQuoteQuery(string portfolio, string pricingCluster)
        {
            Query query = new Query();
            query.EntityName = "Shipping_Location__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Pricing_Cluster__c", Value = pricingCluster, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Portfolio__c", Value = portfolio, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Min_Maint_EUR__c", "LEO_Mini_Maint_EUR__c", "LEO_Mini_Maint_USD__c", "Min_Maint_USD__c", "Portfolio__c", "Pricing_Cluster__c" };
            query.Limit = 1;

            return query;
        }

        public static Query GetDiscountCategoryPriceListItemQuery(HashSet<string> discountCategories, string priceListName)
        {
            Query query = new Query();
            query.EntityName = "Apttus_Config2__PriceListItem__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Apttus_Config2__ProductId__r.NokiaCPQ_Product_Discount_Category__c", Value = new List<string>(discountCategories), ComparisonOperator = ConditionOperator.In},
                        new FilterCondition() { FieldName = "Apttus_Config2__PriceListId__r.Name", Value = priceListName, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Contracted__c", Value = "Yes", ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Apttus_Config2__ProductId__r.NokiaCPQ_Product_Discount_Category__c" };

            return query;
        }

        public static async Task<List<ProductDiscountQueryModel>> ExecuteProductDiscountQuery(IDBHelper dBHelper, string market, List<string> discountCategories)
        {
            return await dBHelper.FindAsync<ProductDiscountQueryModel>("Product_Discount__c",
                                s => (s.Market__c == market) && (discountCategories.Contains(s.Product_Discount_Category__c) || s.Product_Discount_Category__c == null),
                                new string[] { "Id", "Name", "Product_Discount_Category__c", "Market__c", "Discount__c" });
        }
    }
}
