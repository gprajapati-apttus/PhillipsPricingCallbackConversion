using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Nokia.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class DataAccess
    {
        private readonly IDBHelper dbHelper;

        public DataAccess(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public async Task<decimal?> GetDefaultExchangeRate(string CurrencyIsoCode)
        {
            var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(CurrencyIsoCode);
            var defaultExchangeRate = (await dbHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;
            return defaultExchangeRate;
        }

        public async Task<List<CountryPriceListItemQueryModel>> GetCountryPriceListItem(List<string> productList)
        {
            var countryPriceListItemQuery = QueryHelper.GetCountryPriceListItemQuery(productList);
            var countryPriceListItems = await dbHelper.FindAsync<CountryPriceListItemQueryModel>(countryPriceListItemQuery);
            return countryPriceListItems;
        }

        public async Task<List<ProductExtensionsQueryModel>> GetProductExtensions(HashSet<string> productList, string currencycode)
        {
            var productExtensionsQuery = QueryHelper.GetProductExtensionsQuery(productList, currencycode);
            List<ProductExtensionsQueryModel> Prod_extList = await dbHelper.FindAsync<ProductExtensionsQueryModel>(productExtensionsQuery);
            return Prod_extList;
        }

        public async Task<List<MNDirectProductMapQueryModel>> GetMNDirectProductMap()
        {
            var mnDirectProductMapQuery = QueryHelper.GetMNDirectProductMapQuery();
            var mn_Direct_Products_List = await dbHelper.FindAsync<MNDirectProductMapQueryModel>(mnDirectProductMapQuery);
            return mn_Direct_Products_List;
        }

        public async Task<List<DirectPortfolioGeneralSettingQueryModel>> GetDirectPortfolioGeneralSetting(string portfolio)
        {
            var directPortfolioGeneralSettingQuery = QueryHelper.GetDirectPortfolioGeneralSettingQuery(portfolio);
            var portfolioSettingList = await dbHelper.FindAsync<DirectPortfolioGeneralSettingQueryModel>(directPortfolioGeneralSettingQuery);
            return portfolioSettingList;
        }

        public async Task<List<MaintenanceAndSSPRuleQueryModel>> GetMaintenanceAndSSPRule(string region, string maintenanceType)
        {
            var maintenanceAndSSPRuleQuery = QueryHelper.GetMaintenanceAndSSPRuleQuery(region, maintenanceType);
            var maintenanceSSPRuleList = await dbHelper.FindAsync<MaintenanceAndSSPRuleQueryModel>(maintenanceAndSSPRuleQuery);
            return maintenanceSSPRuleList;
        }

        public async Task<List<NokiaMaintenanceAndSSPRulesQueryModel>> GetNokiaMaintenanceAndSSPRules(Proposal proposal)
        {
            var nokiaMaintenanceAndSSPRulesQuery = QueryHelper.GetNokiaMaintenanceAndSSPRulesQuery(proposal);
            var nokiaMaintenanceSSPRules = await dbHelper.FindAsync<NokiaMaintenanceAndSSPRulesQueryModel>(nokiaMaintenanceAndSSPRulesQuery);
            return nokiaMaintenanceSSPRules;
        }

        public async Task<List<TierDiscountDetailQueryModel>> GetTierDiscountDetail(string partnerProgram, string partnerType)
        {
            var tierDiscountDetailQuery = QueryHelper.GetTierDiscountDetailQuery(partnerProgram, partnerType);
            var tierDiscountDetailQueryModels = await dbHelper.FindAsync<TierDiscountDetailQueryModel>(tierDiscountDetailQuery);
            return tierDiscountDetailQueryModels;
        }

        public async Task<List<SSPSRSDefaultValuesQueryModel>> GetSSPSRSDefaultValues(string portfolio)
        {
            var sspSRSDefaultValuesQuery = QueryHelper.GetSSPSRSDefaultValuesQuery(portfolio);
            var sspSRSDefaultsList = await dbHelper.FindAsync<SSPSRSDefaultValuesQueryModel>(sspSRSDefaultValuesQuery);
            return sspSRSDefaultsList;
        }

        public async Task<List<PriceListQueryModel>> GetIndirectMarketPriceList()
        {
            var indirectMarketPriceListQuery = QueryHelper.GetIndirectMarketPriceListQuery();
            var priceListCollection = await dbHelper.FindAsync<PriceListQueryModel>(indirectMarketPriceListQuery);
            return priceListCollection;
        }
    }
}
