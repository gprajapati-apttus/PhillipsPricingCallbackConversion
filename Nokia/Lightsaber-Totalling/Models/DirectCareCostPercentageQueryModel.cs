using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class DirectCareCostPercentageQueryModel
    {
        public string Id { get; set; }

        public string Market__c { get; set; }

        public decimal? Care_Cost__c { get; set; }
    }
}
