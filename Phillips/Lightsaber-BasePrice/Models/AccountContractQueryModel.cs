using System;

namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class AccountContractQueryModel
    {
        public string Id { get; set; }

        public string APTS_Agreement_Group__c { get; set; }

        public string APTS_Related_Agreement__c { get; set; }

        public string APTS_Volume_Tier__c { get; set; }

        public string APTS_Member__c { get; set; }

        public DateTime? APTS_Start_Date__c { get; set; }

        public DateTime? APTS_End_Date__c { get; set; }

        public RelatedAgreementQueryModel APTS_Related_Agreement__r { get; set; }
    }
}
