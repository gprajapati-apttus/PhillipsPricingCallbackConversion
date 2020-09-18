using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    [JsonConverter(typeof(BaseEntitySerializer))]
    public class Proposal : BaseEntity
    {
        private Proposal() : base(Constants.PROPOSAL) { }
        private Proposal(Dictionary<string, object> dbObject) : base(Constants.PROPOSAL, dbObject) { }

        public static Proposal Create(ProductConfigurationModel cart)
        {
            var proposalEntity = cart.Get<BaseEntity>(Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME);
            var proposalEntityDict = new Dictionary<string, object>();

            if (proposalEntity != null)
            {
                proposalEntityDict = proposalEntity.ToDictionary();
            }

            return new Proposal(proposalEntityDict);
        }

        public string Account_Sold_to__c 
        {
            get
            {
                return GetValue<string>(ProposalField.Account_Sold_to__c);
            }
            set
            {
                SetValue(ProposalField.Account_Sold_to__c, value);
            }
        }

        public string Apttus_Proposal__Payment_Term__c
        {
            get
            {
                return GetValue<string>(ProposalField.Apttus_Proposal__Payment_Term__c);
            }
            set
            {
                SetValue(ProposalField.Apttus_Proposal__Payment_Term__c, value);
            }
        }

        public string APTS_Inco_Term__c
        {
            get
            {
                return GetValue<string>(ProposalField.APTS_Inco_Term__c);
            }
            set
            {
                SetValue(ProposalField.APTS_Inco_Term__c, value);
            }
        }
    }
}
