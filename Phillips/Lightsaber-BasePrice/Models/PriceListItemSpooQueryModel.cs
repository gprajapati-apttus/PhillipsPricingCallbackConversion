namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class PriceListItemSpooQueryModel
    {
        public string Id { get; set; }

        public string Apttus_Config2__ProductId__c { get; set; }

        public decimal? APTS_List_Price_Multiplier__c { get; set; }

        public decimal? APTS_Target_Price_Multiplier__c { get; set; }

        public decimal? APTS_Escalation_Price_Multiplier__c { get; set; }

        public decimal? APTS_Minimum_Price_Multiplier__c { get; set; }

        public decimal? APTS_Country_Pricelist_List_Price__c { get; set; }

        public string APTS_Agreement_Group__c { get; set; }

        public decimal? APTS_Tier_1_List_Price__c { get; set; }

        public decimal? APTS_Tier_2_List_Price__c { get; set; }

        public decimal? APTS_Tier_3_List_Price__c { get; set; }

        public decimal? APTS_Tier_4_List_Price__c { get; set; }

        public decimal? APTS_Tier_1_Discount__c { get; set; }

        public decimal? APTS_Tier_2_Discount__c { get; set; }

        public decimal? APTS_Tier_3_Discount__c { get; set; }

        public decimal? APTS_Tier_4_Discount__c { get; set; }

        public PriceListSpooQueryModel Apttus_Config2__PriceListId__r { get; set; }

        public decimal GetListPriceMultipler()
        {
            return APTS_List_Price_Multiplier__c.HasValue ? APTS_List_Price_Multiplier__c.Value : 1;
        }

        public decimal GetTargetPriceMultipler()
        {
            return APTS_Target_Price_Multiplier__c.HasValue ? APTS_Target_Price_Multiplier__c.Value : 1;
        }

        public decimal GetEscalationPriceMultiplier()
        {
            return APTS_Escalation_Price_Multiplier__c.HasValue ? APTS_Escalation_Price_Multiplier__c.Value : 1;
        }

        public decimal GetMinimumPriceMultiplier()
        {
            return APTS_Minimum_Price_Multiplier__c.HasValue ? APTS_Minimum_Price_Multiplier__c.Value : 1;
        }
    }
}
