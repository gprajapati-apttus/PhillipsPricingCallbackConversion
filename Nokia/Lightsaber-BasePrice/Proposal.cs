using System;
using System.Collections.Generic;
using System.Text;

namespace PhillipsConversion.Nokia.Lightsaber_BasePrice
{
    public class Proposal
    {
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
