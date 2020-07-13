﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Totalling
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

        public string NokiaCPQ_No_of_Years__c
        {
            get
            {
                return Get<string>(ProposalField.NokiaCPQ_No_of_Years__c);
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

        public string Apttus_Proposal__Account__r_GEOLevel1ID__c
        {
            get
            {
                return Get<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_GEOLevel1ID__c);
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
