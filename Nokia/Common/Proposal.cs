using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Common
{
    public class Proposal
    {
        public string NokiaCPQ_Portfolio__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_Portfolio__c);
            }
        }

        public bool? NokiaCPQ_LEO_Discount__c
        {
            get
            {
                return Get<bool?>(ProposalField.NokiaCPQ_LEO_Discount__c);
            }
        }

        public string NokiaCPQ_No_Of_Years__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_No_Of_Years__c);
            }
        }

        public string Apttus_Proposal__Account__r_L7Name__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_L7Name__c);
            }
        }

        public bool? Is_List_Price_Only__c
        {
            get
            {
                return Get<bool?>(ProposalField.Is_List_Price_Only__c);
            }
        }

        public string NokiaCPQ_Maintenance_Type__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c);
            }
        }

        public string NokiaCPQ_SSP_Level__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_SSP_Level__c);
            }
        }

        public string NokiaCPQ_SRS_Level__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_SRS_Level__c);
            }
        }

        public string CurrencyIsoCode
        {
            get
            {
                return Get<string>(ProposalField.CurrencyIsoCode);
            }
        }

        public string Quote_Type__c
        {
            get
            {
                return Get<string>(ProposalField.Quote_Type__c);
            }
        }

        public decimal? Exchange_Rate__c
        {
            get
            {
                return Get<decimal?>(ProposalField.Exchange_Rate__c);
            }
        }

        public string NokiaProductAccreditation__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaProductAccreditation__c);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_Maintenance_Accreditation__c);
            }
        }

        public string NokiaCPQ_Maintenance_Level__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_Maintenance_Level__c);
            }
        }

        public bool? NokiaCPQ_IsPMA__c
        {
            get
            {
                return Get<bool?>(ProposalField.NokiaCPQ_IsPMA__c);
            }
        }

        public string Apttus_Proposal__Account__r_GEOLevel1ID__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_GEOLevel1ID__c);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c);
            }
        }

        public string NokiaProductAccreditation__r_Pricing_Accreditation__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Accreditation__c);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c);
            }
        }

        public string NokiaProductAccreditation__r_Pricing_Cluster__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Cluster__c);
            }
        }

        public string Apttus_Proposal__Account__r_Partner_Program__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Program__c);
            }
        }

        public string Apttus_Proposal__Account__r_Partner_Type__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Type__c);
            }
        }

        public string NokiaCPQ_Existing_IONMaint_Contract__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c);
            }
        }

        public string Account_Market__c
        {
            get
            {
                return Get<string>(ProposalField.Account_Market__c);
            }
        }

        public decimal? Maintenance_Y1__c
        {
            get
            {
                return Get<decimal?>(ProposalField.Maintenance_Y1__c);
            }
        }

        public decimal? Maintenance_Y2__c
        {
            get
            {
                return Get<decimal?>(ProposalField.Maintenance_Y2__c);
            }
        }

        public decimal? SSP__c
        {
            get
            {
                return Get<decimal?>(ProposalField.SSP__c);
            }
        }

        public decimal? SRS__c
        {
            get
            {
                return Get<decimal?>(ProposalField.SRS__c);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c);
            }
        }

        public string Apttus_Proposal__Account__r_CountryCode__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_CountryCode__c);
            }
        }

        public decimal? NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c
        {
            get
            {
                return Get<decimal?>(ProposalRelationshipField.NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c);
            }
        }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c
        {
            get
            {
                return Get<bool?>(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c);
            }
        }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c
        {
            get
            {
                return Get<bool?>(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c);
            }
        }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Performance__c
        {
            get
            {
                return Get<bool?>(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Performance__c);
            }
        }

        public bool? NokiaCPQ_Is_Maintenance_Quote__c
        {
            get
            {
                return Get<bool?>(ProposalField.NokiaCPQ_Is_Maintenance_Quote__c);
            }
        }

        public string Warranty_Credit__c
        {
            get
            {
                return Get<string>(ProposalField.Warranty_Credit__c);
            }
        }


        private readonly Dictionary<string, object> proposal;

        public Proposal(Dictionary<string, object> proposal)
        {
            this.proposal = proposal;
        }

        public T Get<T>(string fieldName)
        {
            return (T)proposal[fieldName];
        }
    }
}