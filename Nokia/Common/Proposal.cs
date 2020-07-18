using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Common
{
    [JsonConverter(typeof(BaseEntitySerializer))]
    public class Proposal : BaseEntity
    {
        public Proposal() : base(Constants.PROPOSAL) { }
        public Proposal(Dictionary<string, object> dbObject) : base(Constants.PROPOSAL, dbObject) { }

        public string NokiaCPQ_Portfolio__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_Portfolio__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_Portfolio__c, value);
            }
        }

        public bool? NokiaCPQ_LEO_Discount__c
        {
            get
            {
                return GetValue<bool?>(ProposalField.NokiaCPQ_LEO_Discount__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_LEO_Discount__c, value);
            }
        }

        public string NokiaCPQ_No_Of_Years__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_No_Of_Years__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_No_Of_Years__c, value);
            }
        }

        public string Apttus_Proposal__Account__r_L7Name__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_L7Name__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_L7Name__c, value);
            }
        }

        public bool? Is_List_Price_Only__c
        {
            get
            {
                return GetValue<bool?>(ProposalField.Is_List_Price_Only__c);
            }
            set
            {
                SetValue(ProposalField.Is_List_Price_Only__c, value);
            }
        }

        public string NokiaCPQ_Maintenance_Type__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_Maintenance_Type__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_Maintenance_Type__c, value);
            }
        }

        public string NokiaCPQ_SSP_Level__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_SSP_Level__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_SSP_Level__c, value);
            }
        }

        public string NokiaCPQ_SRS_Level__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_SRS_Level__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_SRS_Level__c, value);
            }
        }

        public string Quote_Type__c
        {
            get
            {
                return GetValue<string>(ProposalField.Quote_Type__c);
            }
            set
            {
                SetValue(ProposalField.Quote_Type__c, value);
            }
        }

        public decimal? Exchange_Rate__c
        {
            get
            {
                return GetValue<decimal?>(ProposalField.Exchange_Rate__c);
            }
            set
            {
                SetValue(ProposalField.Exchange_Rate__c, value);
            }
        }

        public string NokiaProductAccreditation__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaProductAccreditation__c);
            }
            set
            {
                SetValue(ProposalField.NokiaProductAccreditation__c, value);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_Maintenance_Accreditation__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_Maintenance_Accreditation__c, value);
            }
        }

        public string NokiaCPQ_Maintenance_Level__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_Maintenance_Level__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_Maintenance_Level__c, value);
            }
        }

        public bool? NokiaCPQ_IsPMA__c
        {
            get
            {
                return GetValue<bool?>(ProposalField.NokiaCPQ_IsPMA__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_IsPMA__c, value);
            }
        }

        public string Apttus_Proposal__Account__r_GEOLevel1ID__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_GEOLevel1ID__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_GEOLevel1ID__c, value);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c, value);
            }
        }

        public string NokiaProductAccreditation__r_Pricing_Accreditation__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Accreditation__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Accreditation__c, value);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c, value);
            }
        }

        public string NokiaProductAccreditation__r_Pricing_Cluster__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Cluster__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Cluster__c, value);
            }
        }

        public string Apttus_Proposal__Account__r_Partner_Program__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Program__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Program__c, value);
            }
        }

        public string Apttus_Proposal__Account__r_Partner_Type__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Type__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Type__c, value);
            }
        }

        public string NokiaCPQ_Existing_IONMaint_Contract__c
        {
            get
            {
                return GetValue<string>(ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c, value);
            }
        }

        public string Account_Market__c
        {
            get
            {
                return GetValue<string>(ProposalField.Account_Market__c);
            }
            set
            {
                SetValue(ProposalField.Account_Market__c, value);
            }
        }

        public decimal? Maintenance_Y1__c
        {
            get
            {
                return GetValue<decimal?>(ProposalField.Maintenance_Y1__c);
            }
            set
            {
                SetValue(ProposalField.Maintenance_Y1__c, value);
            }
        }

        public decimal? Maintenance_Y2__c
        {
            get
            {
                return GetValue<decimal?>(ProposalField.Maintenance_Y2__c);
            }
            set
            {
                SetValue(ProposalField.Maintenance_Y2__c, value);
            }
        }

        public decimal? SSP__c
        {
            get
            {
                return GetValue<decimal?>(ProposalField.SSP__c);
            }
            set
            {
                SetValue(ProposalField.SSP__c, value);
            }
        }

        public decimal? SRS__c
        {
            get
            {
                return GetValue<decimal?>(ProposalField.SRS__c);
            }
            set
            {
                SetValue(ProposalField.SRS__c, value);
            }
        }

        public string NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c
        {
            get
            {
                return GetLookupValue<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c, value);
            }
        }

        public string Apttus_Proposal__Account__r_CountryCode__c
        {
            get
            {
                return GetValue<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_CountryCode__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_CountryCode__c, value);
            }
        }

        public decimal? NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c
        {
            get
            {
                return GetLookupValue<decimal?>(ProposalRelationshipField.NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c, value);
            }
        }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c
        {
            get
            {
                return GetLookupValue<bool?>(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c, value);
            }
        }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c
        {
            get
            {
                return GetLookupValue<bool?>(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c, value);
            }
        }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Performance__c
        {
            get
            {
                return GetLookupValue<bool?>(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Performance__c);
            }
            set
            {
                SetValue(ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Performance__c, value);
            }
        }

        public bool? NokiaCPQ_Is_Maintenance_Quote__c
        {
            get
            {
                return GetValue<bool?>(ProposalField.NokiaCPQ_Is_Maintenance_Quote__c);
            }
            set
            {
                SetValue(ProposalField.NokiaCPQ_Is_Maintenance_Quote__c, value);
            }
        }

        public string Warranty_Credit__c
        {
            get
            {
                return GetValue<string>(ProposalField.Warranty_Credit__c);
            }
            set
            {
                SetValue(ProposalField.Warranty_Credit__c, value);
            }
        }
    }
}