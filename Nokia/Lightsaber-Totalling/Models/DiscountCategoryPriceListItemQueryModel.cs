namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class DiscountCategoryPriceListItemQueryModel
    {
        public string Id { get; set; }

        public DiscountCategoryPriceListItemProductQueryModel Apttus_Config2__ProductId__r { get; set; }
    }

    public class DiscountCategoryPriceListItemProductQueryModel
    {
        public string NokiaCPQ_Product_Discount_Category__c { get; set; }
    }
}
