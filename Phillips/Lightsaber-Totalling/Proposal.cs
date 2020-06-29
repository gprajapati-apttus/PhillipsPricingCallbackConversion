using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class Proposal
    {
        public string Apttus_Proposal__Payment_Term__c
        {
            get
            {
                return Get<string>(ProposalField.Apttus_Proposal__Payment_Term__c);
            }
        }

        public string APTS_Inco_Term__c
        {
            get
            {
                return Get<string>(ProposalField.APTS_Inco_Term__c);
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
