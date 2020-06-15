namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class LocalBundleHeaderQueryModel
    {
        public string Id { get; set; }
        public string APTS_Component__c { get; set; }
        public LocalBundleComponentQueryModel APTS_Local_Bundle_Header__r { get; set; }
    }
}
