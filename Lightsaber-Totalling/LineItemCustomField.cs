using Apttus.Lightsaber.Pricing.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Totalling
{
    public class LineItemCustomField
    {
        public const string APTS_Unit_Incentive_Adj_Amount__c = "APTS_Unit_Incentive_Adj_Amount__c";
        public const string APTS_Promotion_Discount_Amount_c__c = "APTS_Promotion_Discount_Amount_c__c";
        public const string APTS_Solution_Offered_Price__c = "APTS_Solution_Offered_Price__c";
        public const string APTS_Offered_Price_c__c = "APTS_Offered_Price_c__c";
        public const string APTS_ContractDiscount__c = "APTS_ContractDiscount__c";
        public const string APTS_Item_List_Price__c = "APTS_Item_List_Price__c";
        public const string APTS_Solution_list_Price_c__c = "APTS_Solution_list_Price_c__c";
        public const string APTS_Philips_List_Price__c = "APTS_Philips_List_Price__c";
        public const string APTS_Net_Adjustment_Percent_c__c = "APTS_Net_Adjustment_Percent_c__c";
        public const string APTS_Extended_Quantity__c = "APTS_Extended_Quantity__c";
        public const string APTS_Contract_Discount_Amount__c = "APTS_Contract_Discount_Amount__c";
        public const string APTS_Extended_List_Price__c = "APTS_Extended_List_Price__c";
        public const string APTS_Billing_Plan__c = "APTS_Billing_Plan__c";
        public const string APTS_Unit_Strategic_Discount_Amount__c = "APTS_Unit_Strategic_Discount_Amount__c";
        public const string APTS_Strategic_Discount_Amount_c__c = "APTS_Strategic_Discount_Amount_c__c";
        public const string APTS_SAP_List_Price__c = "APTS_SAP_List_Price__c";
        public const string APTS_Target_Price__c = "APTS_Target_Price__c";
        public const string APTS_Minimum_Price__c = "APTS_Minimum_Price__c";
        public const string APTS_Escalation_Price__c = "APTS_Escalation_Price__c";
        public const string APTS_Contract_Net_Price__c = "APTS_Contract_Net_Price__c";
        public const string APTS_Escalation_Price_Attainment_c__c = "APTS_Escalation_Price_Attainment_c__c";
        public const string APTS_Target_Price_Attainment__c = "APTS_Target_Price_Attainment__c";
        public const string APTS_Minimum_Price_Attainment_c__c = "APTS_Minimum_Price_Attainment_c__c";
        public const string APTS_Price_Attainment_Color__c = "APTS_Price_Attainment_Color__c";
        public const string APTS_Local_Bundle_Component_Flag__c = "APTS_Local_Bundle_Component_Flag__c";
        public const string APTS_Minimum_Price_Bundle__c = "APTS_Minimum_Price_Bundle__c";
        public const string APTS_Escalation_Price_Bundle__c = "APTS_Escalation_Price_Bundle__c";
        public const string APTS_Country_Target_Price__c = "APTS_Country_Target_Price__c";
        public const string APTS_Target_Price_Bundle__c = "APTS_Target_Price_Bundle__c";
        public const string APTS_Option_List_Price__c = "APTS_Option_List_Price__c";
        public const string APTS_Solution_Contract_Discount_Amount__c = "APTS_Solution_Contract_Discount_Amount__c";
        public const string APTS_Incentive_Adjustment_Amount_Bundle__c = "APTS_Incentive_Adjustment_Amount_Bundle__c";
        public const string APTS_Solution_Unit_Incentive_Adj_Amount__c = "APTS_Solution_Unit_Incentive_Adj_Amount__c";
        public const string APTS_Solution_Offered_Price_c__c = "APTS_Solution_Offered_Price_c__c";
        public const string APTS_Solution_List_Price_c__c = "APTS_Solution_List_Price_c__c";
        public const string APTS_Total_Resulting_Discount_Amount__c = "APTS_Total_Resulting_Discount_Amount__c";
        public const string APTS_Solution_Price_Attainment_Color__c = "APTS_Solution_Price_Attainment_Color__c";
        public const string APTS_Payment_Term__c = "APTS_Payment_Term__c";
        public const string APTS_Inco_Term__c = "APTS_Inco_Term__c";
        public const string APTS_Escalation_Price_Attainment__c = "APTS_Escalation_Price_Attainment__c";
        public const string APTS_Minimum_Price_Attainment__c = "APTS_Minimum_Price_Attainment__c";
        public const string APTS_MAG__c = "APTS_MAG__c";
        public const string APTS_Business_Unit__c = "APTS_Business_Unit__c";
        public const string APTS_Is_Escalation_Price_Attained__c = "APTS_Is_Escalation_Price_Attained__c";
        public const string APTS_Is_Minimum_Price_Attained__c = "APTS_Is_Minimum_Price_Attained__c";
        public const string APTS_Procurement_approval_needed__c = "APTS_Procurement_approval_needed__c";
    }

    public class LineItemStandardRelationshipField
    {
        public const string Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c = "Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c";
        public const string Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c = "Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c";
        public const string Apttus_Config2__OptionId__r_Main_Article_Group_ID__c = "Apttus_Config2__OptionId__r.Main_Article_Group_ID__c";
        public const string Apttus_Config2__OptionId__r_Business_Unit_ID__c = "Apttus_Config2__OptionId__r.Business_Unit_ID__c";
        public const string Apttus_Config2__ProductId__r_Main_Article_Group_ID__c = "Apttus_Config2__ProductId__r.Main_Article_Group_ID__c";
        public const string Apttus_Config2__ProductId__r_Business_Unit_ID__c = "Apttus_Config2__ProductId__r.Business_Unit_ID__c";
        public const string Apttus_Config2__ProductId__r_APTS_SPOO_Type__c = "Apttus_Config2__ProductId__r.APTS_SPOO_Type__c";
        public const string Apttus_Config2__ProductId__r_APTS_Type__c = "Apttus_Config2__ProductId__r.APTS_Type__c";
        public const string Apttus_Config2__ProductId__r_APTS_CLOGS__c = "Apttus_Config2__ProductId__r.APTS_CLOGS__c";
    }
}
