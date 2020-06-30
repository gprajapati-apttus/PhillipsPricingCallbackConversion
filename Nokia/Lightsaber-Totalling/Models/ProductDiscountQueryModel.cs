using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class ProductDiscountQueryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Product_Discount_Category__c { get; set; }
        public string Market__c { get; set; }
        public decimal? Discount__c { get; set; }
    }
}
