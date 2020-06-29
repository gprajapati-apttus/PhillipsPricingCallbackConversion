using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class Proposal
    {
        public string Account_Sold_to__c
        {
            get
            {
                return Get<string>(ProposalField.Account_Sold_to__c);
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
