using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class Proposal
    {

        public string NokiaCPQ_Portfolio__c
        {
            get
            {
                return Get<string>(nameof(NokiaCPQ_Portfolio__c));
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
