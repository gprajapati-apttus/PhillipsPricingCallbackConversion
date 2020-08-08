namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PriceListItemQueryModel
    {
        public string Id { get; set; }

        public decimal? APTS_Country_Pricelist_List_Price__c { get; set; }

        public string APTS_Dynamic_Price_Points__c { get; set; }

        public string APTS_Related_Agreement__c { get; set; }

        public decimal? APTS_Tier_1_Target_Price__c { get; set; }

        public decimal? APTS_Tier_2_Target_Price__c { get; set; }

        public decimal? APTS_Tier_3_Target_Price__c { get; set; }

        public decimal? APTS_Tier_4_Target_Price__c { get; set; }

        public decimal? APTS_Country_Target_Price__c { get; set; }

        public decimal? APTS_Tier_1_Pre_Escalation_Price__c { get; set; }

        public decimal? APTS_Tier_2_Pre_Escalation_Price__c { get; set; }

        public decimal? APTS_Tier_3_Pre_Escalation_Price__c { get; set; }

        public decimal? APTS_Tier_4_Pre_Escalation_Price__c { get; set; }

        public decimal? APTS_Country_Pre_Escalation_Price__c { get; set; }

        public decimal? APTS_Tier_1_Minimum_Price__c { get; set; }

        public decimal? APTS_Tier_2_Minimum_Price__c { get; set; }

        public decimal? APTS_Tier_3_Minimum_Price__c { get; set; }

        public decimal? APTS_Tier_4_Minimum_Price__c { get; set; }

        public PriceListQueryModel Apttus_Config2__PriceListId__r { get; set; }
    }
}
