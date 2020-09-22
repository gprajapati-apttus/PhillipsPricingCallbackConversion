using Apttus.Lightsaber.Pricing.Common.Models;

namespace Apttus.Lightsaber.Nokia.Common
{
    public class Proposal
    {
        public Proposal(ProductConfigurationModel cart)
        {
            CurrencyIsoCode = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.CurrencyIsoCode}");
            NokiaCPQ_Portfolio__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_Portfolio__c}");
            NokiaCPQ_LEO_Discount__c = cart.GetLookupValue<bool?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_LEO_Discount__c}");
            NokiaCPQ_No_Of_Years__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_No_Of_Years__c}");
            Apttus_Proposal__Account__r_L7Name__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_L7Name__c}");
            Is_List_Price_Only__c = cart.GetLookupValue<bool?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.Is_List_Price_Only__c}");
            NokiaCPQ_Maintenance_Type__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_Maintenance_Type__c}");
            NokiaCPQ_SSP_Level__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_SSP_Level__c}");
            NokiaCPQ_SRS_Level__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_SRS_Level__c}");
            Quote_Type__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.Quote_Type__c}");
            Exchange_Rate__c = cart.GetLookupValue<decimal?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.Exchange_Rate__c}");
            NokiaProductAccreditation__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaProductAccreditation__c}");
            NokiaCPQ_Maintenance_Accreditation__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_Maintenance_Accreditation__c}");
            NokiaCPQ_Maintenance_Level__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_Maintenance_Level__c}");
            NokiaCPQ_IsPMA__c = cart.GetLookupValue<bool?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_IsPMA__c}");
            Apttus_Proposal__Account__r_GEOLevel1ID__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_GEOLevel1ID__c}");
            NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c}");
            NokiaProductAccreditation__r_Pricing_Accreditation__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Accreditation__c}");
            NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c}");

            NokiaProductAccreditation__r_Pricing_Cluster__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Cluster__c}");
            Apttus_Proposal__Account__r_Partner_Program__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Program__c}");
            Apttus_Proposal__Account__r_Partner_Type__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_Partner_Type__c}");
            NokiaCPQ_Existing_IONMaint_Contract__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c}");
            Account_Market__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.Account_Market__c}");
            Maintenance_Y1__c = cart.GetLookupValue<decimal?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.Maintenance_Y1__c}");
            Maintenance_Y2__c = cart.GetLookupValue<decimal?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.Maintenance_Y2__c}");
            SSP__c = cart.GetLookupValue<decimal?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.SSP__c}");
            SRS__c = cart.GetLookupValue<decimal?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.SRS__c}");
            NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c}");
            Apttus_Proposal__Account__r_CountryCode__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_CountryCode__c}");
            NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c = cart.GetLookupValue<decimal?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c}");
            Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c = cart.GetLookupValue<bool?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c}");
            Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c = cart.GetLookupValue<bool?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c}");
            Apttus_Proposal__Account__r_NokiaCPQ_Performance__c = cart.GetLookupValue<bool?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalRelationshipField.Apttus_Proposal__Account__r_NokiaCPQ_Performance__c}");
            NokiaCPQ_Is_Maintenance_Quote__c = cart.GetLookupValue<bool?>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.NokiaCPQ_Is_Maintenance_Quote__c}");
            Warranty_Credit__c = cart.GetLookupValue<string>($"{Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME}.{ProposalField.Warranty_Credit__c}");
        }

        public string CurrencyIsoCode { get; private set; }


        public string NokiaCPQ_Portfolio__c { get; private set; }

        public bool? NokiaCPQ_LEO_Discount__c { get; private set; }

        public string NokiaCPQ_No_Of_Years__c { get; private set; }

        public string Apttus_Proposal__Account__r_L7Name__c { get; private set; }

        public bool? Is_List_Price_Only__c { get; private set; }

        public string NokiaCPQ_Maintenance_Type__c { get; private set; }

        public string NokiaCPQ_SSP_Level__c { get; private set; }

        public string NokiaCPQ_SRS_Level__c { get; private set; }

        public string Quote_Type__c { get; private set; }

        public decimal? Exchange_Rate__c { get; private set; }

        public string NokiaProductAccreditation__c { get; private set; }

        public string NokiaCPQ_Maintenance_Accreditation__c { get; private set; }

        public string NokiaCPQ_Maintenance_Level__c { get; private set; }

        public bool? NokiaCPQ_IsPMA__c { get; private set; }

        public string Apttus_Proposal__Account__r_GEOLevel1ID__c { get; private set; }

        public string NokiaCPQ_Maintenance_Accreditation__r_Pricing_Accreditation__c { get; private set; }

        public string NokiaProductAccreditation__r_Pricing_Accreditation__c { get; private set; }

        public string NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c { get; private set; }

        public string NokiaProductAccreditation__r_Pricing_Cluster__c { get; private set; }

        public string Apttus_Proposal__Account__r_Partner_Program__c { get; private set; }

        public string Apttus_Proposal__Account__r_Partner_Type__c { get; private set; }

        public string NokiaCPQ_Existing_IONMaint_Contract__c { get; private set; }

        public string Account_Market__c { get; private set; }

        public decimal? Maintenance_Y1__c { get; private set; }

        public decimal? Maintenance_Y2__c { get; private set; }

        public decimal? SSP__c { get; private set; }

        public decimal? SRS__c { get; private set; }

        public string NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c { get; private set; }

        public string Apttus_Proposal__Account__r_CountryCode__c { get; private set; }

        public decimal? NokiaProductAccreditation__r_NokiaCPQ_Incoterm_Percentage__c { get; private set; }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Renewal__c { get; private set; }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Attachment__c { get; private set; }

        public bool? Apttus_Proposal__Account__r_NokiaCPQ_Performance__c { get; private set; }

        public bool? NokiaCPQ_Is_Maintenance_Quote__c { get; private set; }

        public string Warranty_Credit__c { get; private set; }
    }
}