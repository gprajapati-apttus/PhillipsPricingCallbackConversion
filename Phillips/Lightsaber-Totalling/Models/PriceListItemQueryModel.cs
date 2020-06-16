namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PriceListItemQueryModel
    {
        public string Id { get; set; }

        public decimal? APTS_Country_Pricelist_List_Price__c { get; set; }

        public PriceListQueryModel Apttus_Config2__PriceListId__r { get; set; }
    }
}
