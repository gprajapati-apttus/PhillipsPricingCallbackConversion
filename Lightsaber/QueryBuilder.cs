using Apttus.Lightsaber.Extensibility.Framework.Library.Common;
using Apttus.Lightsaber.Extensibility.Framework.Library.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Lightsaber
{
    class QueryHelper
    {
        public static Query GetPLIPriceMultiplierQuery(IEnumerable<string> spooProdIds)
        {
            Query query = new Query();
            query.EntityName = "Apttus_Config2__PriceListItem__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Apttus_Config2__ProductId__c", Value = spooProdIds, ComparisonOperator = ConditionOperator.In}
                };
            query.Fields = new string[] { "Id", "Apttus_Config2__ProductId__c", "APTS_List_Price_Multiplier__c", "APTS_Target_Price_Multiplier__c", "APTS_Escalation_Price_Multiplier__c", "APTS_Minimum_Price_Multiplier__c" };

            return query;
        }

        public static Query GetPLITierQuery(HashSet<string> priceListItemIdSet)
        {
            Query query = new Query();
            query.EntityName = "Apttus_Config2__PriceListItem__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "Id", Value = priceListItemIdSet, ComparisonOperator = ConditionOperator.In}
                };
            query.Fields = new string[] { "Id", "APTS_Country_Pricelist_List_Price__c", "APTS_Agreement_Group__c", "Apttus_Config2__ProductId__c", "APTS_Tier_1_List_Price__c", "APTS_Tier_2_List_Price__c"
                                          "APTS_Tier_3_List_Price__c", "APTS_Tier_4_List_Price__c", "Apttus_Config2__PriceListId__r.APTS_Related_Agreement__c"
                                        };

            return query;
        }

        //GP: Pending
        public static Query GetAgreementTierQuery(HashSet<string> pliRelatedAgreementSet, string soldToAccount)
        {
            Query query = new Query();
            query.EntityName = "APTS_Account_Contract__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "APTS_Related_Agreement__c", Value = pliRelatedAgreementSet, ComparisonOperator = ConditionOperator.In},
                        new FilterCondition() { FieldName = "APTS_Member__c", Value = soldToAccount, ComparisonOperator = ConditionOperator.EqualTo}
                };
            query.Fields = new string[] { 
                                            "Id", "APTS_Agreement_Group__c", "APTS_Volume_Tier__c"
                                        };

            return query;

            //SELECT Id, APTS_Member__c,
            //        APTS_Agreement_Group__c, APTS_Volume_Tier__c, APTS_Related_Agreement__c,
            //        APTS_Related_Agreement__r.APTS_Default_Volume_Tier__c
            //        FROM APTS_Account_Contract__c
            //        WHERE APTS_Member__c = :proposalSO.Account_Sold_to__c
            //                                AND APTS_Related_Agreement__c IN: setPliRelatedAgreement
            //                                AND(APTS_End_Date__c >= TODAY OR(APTS_End_Date__c = null AND APTS_Related_Agreement__r.Apttus__Contract_End_Date__c >= TODAY))
            //                                AND(APTS_Start_Date__c <= TODAY OR(APTS_Start_Date__c = null AND  APTS_Related_Agreement__r.Apttus__Contract_Start_Date__c < = TODAY))

            //DBHelper dBHelper = null;

            //dBHelper.FindAsync<AccountContracttQueryModel>("APTS_Account_Contract__c",
            //                                                (s =>
            //                                                    (s.APTS_Member__c == "")
            //                                                    && (pliRelatedAgreementSet.contains(s.APTS_Related_Agreement__c))
            //                                                    && (s.APTS_End_Date__c >= DateTime.UtcNow || (s.APTS_End_Date__c == null && s.APTS_Related_Agreement__r.Apttus__Contract_End_Date__c >= DateTime.UtcNow))
            //                                                    && (s.APTS_Start_Date__c <= DateTime.UtcNow || (s.APTS_Start_Date__c == null && s.APTS_Related_Agreement__r.Apttus__Contract_Start_Date__c <= DateTime.UtcNow))
            //                                                );
        }

        public static Query GetNAMBundleQuery(HashSet<string> localBundleOptionSet)
        {
            Query query = new Query();
            query.EntityName = "APTS_Account_Contract__c";
            query.Conditions = new List<FilterCondition>()
                {
                        new FilterCondition() { FieldName = "APTS_Local_Bundle_Header__r.APTS_Active__c", Value = true, ComparisonOperator = ConditionOperator.EqualTo},
                        new FilterCondition() { FieldName = " APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c", Value = localBundleOptionSet, ComparisonOperator = ConditionOperator.In}
                };
            query.Fields = new string[] {
                                            "Id", "APTS_Local_Bundle_Header__r.APTS_Local_Bundle__c", "APTS_Local_Bundle_Header__r.APTS_Parent_Bundle__c", "APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c","APTS_Local_Bundle_Header__r.Id",
                                            "APTS_Component__c"
                                        };

            //SELECT id, APTS_Local_Bundle_Header__r.APTS_Local_Bundle__c, APTS_Local_Bundle_Header__r.APTS_Parent_Bundle__c, APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c, 
            //    APTS_Component__c FROM APTS_Local_Bundle_Component__c 
            //    where APTS_Active__c = TRUE AND APTS_Local_Bundle_Header__r.APTS_Active__c = TRUE AND APTS_Local_Bundle_Header__r.APTS_Parent_Local_Bundle__c IN('01t0W000005GQ7CQAW01t0W0000059WiUQAU')

            return query;
        }
    }
}
