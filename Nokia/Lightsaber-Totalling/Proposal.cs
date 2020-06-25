using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class Proposal
    {
        private readonly Dictionary<string, object> proposal;

        public string NokiaCPQ_Portfolio__c
        {
            get
            {
                return Get<string>(nameof(NokiaCPQ_Portfolio__c));
            }
        }

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
