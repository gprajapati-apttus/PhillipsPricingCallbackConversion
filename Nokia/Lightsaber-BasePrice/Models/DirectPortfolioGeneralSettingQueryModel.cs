using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class DirectPortfolioGeneralSettingQueryModel
    {
        public string Id { get; set; }

        public string Portfolio__c { get; set; }

        public bool? Cost_Calculation_In_PCB__c { get; set; }
    }
}
