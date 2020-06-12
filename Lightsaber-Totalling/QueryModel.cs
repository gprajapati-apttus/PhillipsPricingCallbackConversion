using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Totalling
{
    public class PriceListItemQueryModel
    {
        public string Id { get; set; }

        public decimal? APTS_Country_Pricelist_List_Price__c { get; set; }

        public PriceListQueryModel Apttus_Config2__PriceListId__r { get; set; }
    }

    public class PriceListQueryModel
    {
        public string APTS_Payment_Term_Credit_Terms__c { get; set; }

        public string APTS_Inco_Terms__c { get; set; }

        public string Apttus_Config2__ContractNumber__c { get; set; }
    }

    public class ThresholdApproverQueryModel
    {
        public string Id { get; set; }

        public decimal? APTS_3rd_Party_Threshold__c { get; set; }

        public decimal? APTS_PM_Percentage_Threshold__c { get; set; }

        public decimal? APTS_Installation_Threshold__c { get; set; }

        public decimal? APTS_Quote_Total_Threshold__c { get; set; }

        public decimal? APTS_Turnover_Threshold__c { get; set; }

        public decimal? APTS_FSE_Threshold__c { get; set; }
    }

    public class ProcurementApprovalQueryModel
    {
        public string Id { get; set; }

        public decimal? APTS_Threshold_Value__c { get; set; }

        public string APTS_CLOGS__c { get; set; }
    }
}
