using Apttus.Lightsaber.Extensibility.Library.Model;
using Apttus.Lightsaber.Nokia.Common;
using System.Collections.Generic;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public static class QueryHelper
    {
        public static Query GetDefaultExchangeRateQuery(string currencyIsoCode)
        {
            Query query = new Query();
            query.EntityName = "CurrencyType";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "IsoCode", Value = currencyIsoCode, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "ConversionRate" };
            query.Limit = 1;

            return query;
        }

        public static Query GetCountryPriceListItemQuery(List<string> productList, string currencyIsoCode)
        {
            Query query = new Query();
            query.EntityName = "Apttus_Config2__PriceListItem__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Apttus_Config2__PriceListId__r.PriceList_Type__c", Value = "CPQ", ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Apttus_Config2__ProductId__c", Value = productList, ComparisonOperator = ConditionOperator.In},
                        new FilterCondition() { FieldName = "Apttus_Config2__PriceListId__r.Apttus_Config2__BasedOnPriceListId__c", Value = null, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "CurrencyIsoCode", Value = currencyIsoCode, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Apttus_Config2__ListPrice__c", "Apttus_Config2__ProductId__c", "Apttus_Config2__ProductActive__c",
                "Apttus_Config2__ProductId__r.Portfolio__c", "Apttus_Config2__Cost__c" };

            return query;
        }

        public static Query GetProductExtensionsQuery(HashSet<string> productList, string currencycode)
        {
            Query query = new Query();
            query.EntityName = "Product_Extension__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "CurrencyIsoCode", Value = currencycode, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Product__c", Value = new List<string>(productList), ComparisonOperator = ConditionOperator.In},
                };
            query.Fields = new string[] { "Id", "Product__c", "Market_Price__c", "Floor_Price__c", "Custom_Bid__c" };

            return query;
        }

        public static Query GetMNDirectProductMapQuery()
        {
            Query query = new Query();
            query.EntityName = "NokiaCPQ_MN_Direct_Product_Map__mdt";
            query.Fields = new string[] { "Id", "NokiaCPQ_Product_Code__c", "NokiaCPQ_Product_Type__c" };

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

        public static Query GetMaintenanceAndSSPRuleQuery(string region, string maintenanceType)
        {
            Query query = new Query();
            query.EntityName = "CPQ_Maintenance_and_SSP_Rule__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Region__c", Value = region, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Maintenance_Type__c", Value = maintenanceType, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Region__c", "Maintenance_Level__c", "Maintenance_Type__c", "Maintenance_Category__c", "Service_Rate_Y1__c",
                                            "Service_Rate_Y2__c", "Biennial_SSP_Discount__c", "Unlimited_SSP_Discount__c", "Biennial_SRS_Discount__c", "Unlimited_SRS_Discount__c" };

            return query;
        }

        public static Query GetNokiaMaintenanceAndSSPRulesQuery(Proposal proposal)
        {
            Query query = new Query();
            query.EntityName = "NokiaCPQ_Maintenance_and_SSP_Rules__c";
            string maintenanceType = string.Empty;
            string maintPricinglevel = string.Empty;
            string pricingCluster = string.Empty;
            var whereCondition = new List<FilterCondition>();

            pricingCluster = proposal.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c ?? proposal.NokiaProductAccreditation__r_Pricing_Cluster__c;

            if (proposal.NokiaCPQ_LEO_Discount__c == true && proposal.NokiaCPQ_Portfolio__c != Constants.NOKIA_NUAGE)
            {
                maintenanceType = proposal.NokiaCPQ_Maintenance_Type__c;
            }
            else
            {
                maintenanceType = Constants.MAINT_TYPE_DEFAULT_VALUE;
            }

            if (proposal.NokiaCPQ_Maintenance_Level__c != Constants.NOKIA_YES)
            {
                maintPricinglevel = proposal.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c;
            }
            else
            {
                maintPricinglevel = Constants.Nokia_Brand;
            }

            if (proposal.NokiaCPQ_IsPMA__c == false && proposal.NokiaCPQ_LEO_Discount__c == false)
            {
                whereCondition = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "NokiaCPQ_Pricing_Cluster__c", Value = pricingCluster, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_Maintenance_Type__c", Value = maintenanceType, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Partner_Program__c", Value = proposal.Apttus_Proposal__Account__r_Partner_Program__c, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_withPMA__c", Value = false, ComparisonOperator = ConditionOperator.EqualTo}
                };
            }
            else if (proposal.NokiaCPQ_LEO_Discount__c == true)
            {
                whereCondition = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "NokiaCPQ_Pricing_Cluster__c", Value = pricingCluster, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_Maintenance_Type__c", Value = maintenanceType, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_Maintenance_Level__c", Value = string.Empty, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Partner_Program__c", Value = proposal.Apttus_Proposal__Account__r_Partner_Program__c, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_withPMA__c", Value = false, ComparisonOperator = ConditionOperator.EqualTo}

                };
            }
            else if (proposal.NokiaCPQ_IsPMA__c == true)
            {
                whereCondition = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "NokiaCPQ_Pricing_Cluster__c", Value = pricingCluster, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_Maintenance_Type__c", Value = proposal.NokiaCPQ_Maintenance_Type__c, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_Maintenance_Level__c", Value = maintPricinglevel, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "Partner_Program__c", Value = proposal.Apttus_Proposal__Account__r_Partner_Program__c, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_withPMA__c", Value = true, ComparisonOperator = ConditionOperator.EqualTo}
                };
            }

            query.Conditions = whereCondition;
            query.Fields = new string[] { "Id", "NokiaCPQ_Product_Discount_Category__c", "NokiaCPQ_Product_Discount_Category_per__c", "NokiaCPQ_Unlimited_SSP_Discount__c",
                "NokiaCPQ_Biennial_SSP_Discount__c", "NokiaCPQ_Service_Rate_Y1__c", "NokiaCPQ_Service_Rate_Y2__c" };

            return query;
        }

        public static Query GetTierDiscountDetailQuery(string partnerProgram, string partnerType)
        {
            Query query = new Query();
            query.EntityName = "Tier_Discount_Detail__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Nokia_CPQ_Partner_Program__c", Value = partnerProgram, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = "NokiaCPQ_Partner_Type__c", Value = partnerType, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "NokiaCPQ_Tier_Type__c", "NokiaCPQ_Partner_Type__c", "NokiaCPQ_Pricing_Tier__c", "NokiaCPQ_Tier_Discount__c", "Nokia_CPQ_Partner_Program__c" };

            return query;
        }

        public static Query GetSSPSRSDefaultValuesQuery(string portfolio)
        {
            Query query = new Query();
            query.EntityName = "Nokia_CPQ_SSP_SRS_Default_Values__mdt";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Portfolio__c", Value = portfolio, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "Portfolio__c", "SSP_Visible__c", "SRS_Visible__c", "SRS_Percentage__c", "Tier_Discount_Applicable__c", "AccountLevel_Discount_Applicable__c",
                "Multi_Year_Discount_Applicable__c" };
            query.Limit = 1;

            return query;
        }

        public static Query GetIndirectMarketPriceListQuery()
        {
            Query query = new Query();
            query.EntityName = "Apttus_Config2__PriceList__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "PriceList_Type__c", Value = "Indirect Market", ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "PriceList_Type__c" };

            return query;
        }
    }
}
