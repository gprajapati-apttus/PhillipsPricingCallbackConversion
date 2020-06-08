using Apttus.Lightsaber.Extensibility.Framework.Library.Common;
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
    }
}
