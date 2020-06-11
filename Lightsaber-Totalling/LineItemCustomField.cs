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
        public const string Apttus_Config2__PriceListItemId__r_APTS_Country_Pricelist_List_Price__c = "Apttus_Config2__PriceListItemId__r.APTS_Country_Pricelist_List_Price__c";
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
        //public const string APTS_Minimum_Price_Service__c = "APTS_Minimum_Price_Service__c";
        //public const string APTS_Cost_Service__c = "APTS_Cost_Service__c";
        //public const string APTS_VolumeTier__c = "APTS_VolumeTier__c";
        //public const string APTS_ContractDiscount__c = "APTS_ContractDiscount__c";
        //public const string Apttus_Config2__OptionId__r_APTS_Local_Bundle__c = "Apttus_Config2__OptionId__r.APTS_Local_Bundle__c";
        public const string APTS_Local_Bundle_Component_Flag__c = "APTS_Local_Bundle_Component_Flag__c";
    }

    public class LineItemStandardRelationshipField
    {
        public const string Apttus_Config2__PriceListId__r_Apttus_Config2__ContractNumber__c = "Apttus_Config2__PriceListId__r.Apttus_Config2__ContractNumber__c";
        public const string Apttus_Config2__PriceListItemId__r_Apttus_Config2__ListPrice__c = "Apttus_Config2__PriceListItemId__r.Apttus_Config2__ListPrice__c";
        public const string Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c = "Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c";
        public const string Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c = "Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c";
    }
}
