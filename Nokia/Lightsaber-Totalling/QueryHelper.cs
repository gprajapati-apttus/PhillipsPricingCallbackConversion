using Apttus.Lightsaber.Extensibility.Framework.Library.Common;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    class QueryHelper
    {
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

        //GP: Result of this query can cached to improve callback performance.
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

        //GP: Result of this query can cached to improve callback performance.
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

        public static async Task<List<ProductDiscountQueryModel>> ExecuteProductDiscountQuery(IDBHelper dBHelper, string market, List<string> discountCategories)
        {
            return await dBHelper.FindAsync<ProductDiscountQueryModel>("Product_Discount__c",
                                s => (s.Market__c == market) && (discountCategories.Contains(s.Product_Discount_Category__c) || s.Product_Discount_Category__c == null), 
                                new string[] { "Id", "Name", "Product_Discount_Category__c", "Market__c", "Discount__c" });
        }

        //[select id, name, Product_Discount_Category__c, Market__c, Discount__c from Product_Discount__c where Market__c =: market AND(Product_Discount_Category__c in: discountCatgory OR Product_Discount_Category__c = null)];

        //public static Query GetPLIPriceMultiplierQuery(IEnumerable<string> spooProdIds)
        //{
        //    Query query = new Query();
        //    query.EntityName = "Apttus_Config2__PriceListItem__c";
        //    query.Conditions = new List<FilterCondition>()
        //        {
        //                new FilterCondition() { FieldName = "Apttus_Config2__ProductId__c", Value = spooProdIds, ComparisonOperator = ConditionOperator.In}
        //        };
        //    query.Fields = new string[] { "Id", "Apttus_Config2__ProductId__c", "APTS_List_Price_Multiplier__c", "APTS_Target_Price_Multiplier__c", "APTS_Escalation_Price_Multiplier__c", "APTS_Minimum_Price_Multiplier__c" };

        //    return query;
        //}

        //public static Query GetPLITierQuery(HashSet<string> priceListItemIdSet)
        //{
        //    Query query = new Query();
        //    query.EntityName = "Apttus_Config2__PriceListItem__c";
        //    query.Conditions = new List<FilterCondition>()
        //        {
        //                new FilterCondition() { FieldName = "Id", Value = priceListItemIdSet, ComparisonOperator = ConditionOperator.In}
        //        };
        //    query.Fields = new string[] { "Id", "APTS_Country_Pricelist_List_Price__c", "APTS_Agreement_Group__c", "Apttus_Config2__ProductId__c", "APTS_Tier_1_List_Price__c", "APTS_Tier_2_List_Price__c",
        //                                  "APTS_Tier_3_List_Price__c", "APTS_Tier_4_List_Price__c", "Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c"
        //                                };

        //    return query;
        //}

        //public static async Task<List<AccountContractQueryModel>> ExecuteAgreementTierQuery(IDBHelper dBHelper, HashSet<string> pliRelatedAgreementSet, string soldToAccount)
        //{
        //    //Query query = new Query();
        //    //query.EntityName = "APTS_Account_Contract__c";
        //    //query.Conditions = new List<FilterCondition>()
        //    //    {
        //    //            new FilterCondition() { FieldName = "APTS_Related_Agreement__c", Value = pliRelatedAgreementSet, ComparisonOperator = ConditionOperator.In},
        //    //            new FilterCondition() { FieldName = "APTS_Member__c", Value = soldToAccount, ComparisonOperator = ConditionOperator.EqualTo}
        //    //    };
        //    //query.Fields = new string[] { 
        //    //                                "Id", "APTS_Agreement_Group__c", "APTS_Volume_Tier__c"
        //    //                            };

        //    //return query;

        //    //SELECT Id, APTS_Member__c,
        //    //        APTS_Agreement_Group__c, APTS_Volume_Tier__c, APTS_Related_Agreement__c,
        //    //        APTS_Related_Agreement__r.APTS_Default_Volume_Tier__c
        //    //        FROM APTS_Account_Contract__c
        //    //        WHERE APTS_Member__c = :proposalSO.Account_Sold_to__c
        //    //                                AND APTS_Related_Agreement__c IN: setPliRelatedAgreement
        //    //                                AND(APTS_End_Date__c >= TODAY OR(APTS_End_Date__c = null AND APTS_Related_Agreement__r.Apttus__Contract_End_Date__c >= TODAY))
        //    //                                AND(APTS_Start_Date__c <= TODAY OR(APTS_Start_Date__c = null AND  APTS_Related_Agreement__r.Apttus__Contract_Start_Date__c < = TODAY))

        //    return await dBHelper.FindAsync<AccountContractQueryModel>("APTS_Account_Contract__c",
        //                        s => (s.APTS_Member__c == soldToAccount) && (pliRelatedAgreementSet.Contains(s.APTS_Related_Agreement__c))
        //                                && (s.APTS_End_Date__c >= DateTime.UtcNow || (s.APTS_End_Date__c == null && s.APTS_Related_Agreement__r.Apttus__Contract_End_Date__c >= DateTime.UtcNow))
        //                                && (s.APTS_Start_Date__c <= DateTime.UtcNow || (s.APTS_Start_Date__c == null && s.APTS_Related_Agreement__r.Apttus__Contract_Start_Date__c <= DateTime.UtcNow)),
        //                        new string[] { "Id", "APTS_Agreement_Group__c", "APTS_Volume_Tier__c" });
        //}

        //public static Query GetNAMBundleQuery(HashSet<string> localBundleOptionSet)
        //{
        //    Query query = new Query();
        //    query.EntityName = "APTS_Account_Contract__c";
        //    query.Conditions = new List<FilterCondition>()
        //        {
        //                new FilterCondition() { FieldName = "APTS_Local_Bundle_Header__r.APTS_Active__c", Value = true, ComparisonOperator = ConditionOperator.EqualTo},
        //                new FilterCondition() { FieldName = " APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c", Value = localBundleOptionSet, ComparisonOperator = ConditionOperator.In}
        //        };
        //    query.Fields = new string[] {
        //                                    "Id", "APTS_Local_Bundle_Header__r.APTS_Local_Bundle__c", "APTS_Local_Bundle_Header__r.APTS_Parent_Bundle__c", "APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c","APTS_Local_Bundle_Header__r.Id",
        //                                    "APTS_Component__c"
        //                                };

        //    //SELECT id, APTS_Local_Bundle_Header__r.APTS_Local_Bundle__c, APTS_Local_Bundle_Header__r.APTS_Parent_Bundle__c, APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c, 
        //    //    APTS_Component__c FROM APTS_Local_Bundle_Component__c 
        //    //    where APTS_Active__c = TRUE AND APTS_Local_Bundle_Header__r.APTS_Active__c = TRUE AND APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c IN('01t0W000005GQ7CQAW01t0W0000059WiUQAU')

        //    return query;
        //}
    }
}
