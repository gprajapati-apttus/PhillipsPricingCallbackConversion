namespace PhillipsConversion
{
    public class PriceListItemQueryModel
    {
        public string Id { get; set; }

        public string Apttus_Config2__ProductId__c { get; set; }

        public decimal? APTS_List_Price_Multiplier__c { get; set; }

        public decimal? APTS_Target_Price_Multiplier__c { get; set; }

        public decimal? APTS_Escalation_Price_Multiplier__c { get; set; }

        public decimal? APTS_Minimum_Price_Multiplier__c { get; set; }

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
