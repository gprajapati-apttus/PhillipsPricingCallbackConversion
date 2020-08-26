using Apttus.Lightsaber.Extensibility.Framework.Library.Common;
using System.Collections.Generic;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class QueryHelper
    {
        public static Query GetPLIQuery(HashSet<string> priceListItemIdSet)
        {
            Query query = new Query();
            query.EntityName = "Apttus_Config2__PriceListItem__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Id", Value = new List<string>(priceListItemIdSet), ComparisonOperator = ConditionOperator.In}
                };
            query.Fields = new string[] { "Id", "APTS_Country_Pricelist_List_Price__c", "Apttus_Config2__PriceListId__r.Apttus_Config2__ContractNumber__c", "Apttus_Config2__PriceListId__r.Apttus_Config2__Type__c","Apttus_Config2__PriceListId__r.APTS_Payment_Term_Credit_Terms__c", "Apttus_Config2__PriceListId__r.APTS_Inco_Terms__c",
                "APTS_Dynamic_Price_Points__c","APTS_Related_Agreement__c","APTS_Tier_1_Target_Price__c", "APTS_Tier_2_Target_Price__c", "APTS_Tier_3_Target_Price__c", "APTS_Tier_4_Target_Price__c",
                "APTS_Country_Target_Price__c", "APTS_Tier_1_Pre_Escalation_Price__c", "APTS_Tier_2_Pre_Escalation_Price__c", "APTS_Tier_3_Pre_Escalation_Price__c", "APTS_Tier_4_Pre_Escalation_Price__c",
                "APTS_Country_Pre_Escalation_Price__c", "APTS_Tier_1_Minimum_Price__c", "APTS_Tier_2_Minimum_Price__c", "APTS_Tier_3_Minimum_Price__c", "APTS_Tier_4_Minimum_Price__c"
                                        };

            return query;
        }

        public static Query GetThresholdApproversQuery(string salesOrganization)
        {
            Query query = new Query();
            query.EntityName = "Threshold_Approvers__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "APTS_Sales_Organization__c", Value = salesOrganization, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "APTS_3rd_Party_Threshold__c", "APTS_PM_Percentage_Threshold__c", "APTS_Installation_Threshold__c", "APTS_Quote_Total_Threshold__c", "APTS_Turnover_Threshold__c",
                                            "APTS_FSE_Threshold__c"
                                        };
            query.Limit = 1;
            return query;
        }

        public static Query GetProcurementApprovalQuery(string salesOrganization)
        {
            Query query = new Query();
            query.EntityName = "APTS_Procurement_Approval__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "APTS_Sales_Organization__c", Value = salesOrganization, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { "Id", "APTS_Threshold_Value__c", "APTS_CLOGS__c"
                                        };
            return query;
        }
    }
}
