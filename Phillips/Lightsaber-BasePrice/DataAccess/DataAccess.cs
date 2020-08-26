using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Phillips.Pricing
{
    public class DataAccess
    {
        private readonly IDBHelper dbHelper;

        public DataAccess(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public async Task<List<PriceListItemQueryModel>> GetPLIPriceMultiplier(IEnumerable<string> spooProdIds)
        {
            var query = QueryHelper.GetPLIPriceMultiplierQuery(spooProdIds);
            List<PriceListItemQueryModel> spooPriceListItems = await dbHelper.FindAsync<PriceListItemQueryModel>(query);
            return spooPriceListItems;
        }

        public async Task<List<PriceListItemQueryModel>> GetPLITier(HashSet<string> priceListItemIdSet)
        {
            var pliTierQuery = QueryHelper.GetPLITierQuery(priceListItemIdSet);
            List<PriceListItemQueryModel> pliTierDetails = await dbHelper.FindAsync<PriceListItemQueryModel>(pliTierQuery);
            return pliTierDetails;
        }

        public async Task<List<AccountContractQueryModel>> GetAgreementTier(HashSet<string> pliRelatedAgreementSet, string soldToAccount)
        {
            return await QueryHelper.ExecuteAgreementTierQuery(dbHelper, pliRelatedAgreementSet, soldToAccount);
        }

        public async Task<List<LocalBundleHeaderQueryModel>> GetNAMBundle(HashSet<string> localBundleOptionSet)
        {
            var query = QueryHelper.GetNAMBundleQuery(localBundleOptionSet);
            List<LocalBundleHeaderQueryModel> namBundleItems = await dbHelper.FindAsync<LocalBundleHeaderQueryModel>(query);
            return namBundleItems;
        }
    }
}