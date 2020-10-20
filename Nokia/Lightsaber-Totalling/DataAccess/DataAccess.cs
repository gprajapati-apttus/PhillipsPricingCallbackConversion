using Apttus.Lightsaber.Extensibility.Library.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class DataAccess
    {
        private readonly IDBHelper dbHelper;

        public DataAccess(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public async Task<List<ContractedPriceListItemQueryModel>> GetContractedPriceListItem(HashSet<string> priceListItemIds)
        {
            var contractedPriceListItemsQuery = QueryHelper.GetContractedPriceListItemQuery(priceListItemIds);
            var contractedPriceListItems = await dbHelper.FindAsync<ContractedPriceListItemQueryModel>(contractedPriceListItemsQuery);
            return contractedPriceListItems;
        }

        public async Task<decimal?> GetDefaultExchangeRate(string CurrencyIsoCode)
        {
            var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(CurrencyIsoCode);
            var conversionRate = (await dbHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;
            return conversionRate;
        }

        public async Task<decimal?> GetPricingGuidanceSetting(string portfolio)
        {
            var pricingGuidanceSettingQuery = QueryHelper.GetPricingGuidanceSettingQuery(portfolio);
            var pricingGuidanceSettingThresold = (await dbHelper.FindAsync<PricingGuidanceSettingQueryModel>(pricingGuidanceSettingQuery)).FirstOrDefault()?.Threshold__c;
            return pricingGuidanceSettingThresold;
        }

        public async Task<List<DirectPortfolioGeneralSettingQueryModel>> GetDirectPortfolioGeneralSetting(string portfolio)
        {
            var directPortfolioGeneralSettingQuery = QueryHelper.GetDirectPortfolioGeneralSettingQuery(portfolio);
            var portfolioSettingList = await dbHelper.FindAsync<DirectPortfolioGeneralSettingQueryModel>(directPortfolioGeneralSettingQuery);
            return portfolioSettingList;
        }

        public async Task<List<DirectCareCostPercentageQueryModel>> GetDirectCareCostPercentage(string accountMarket)
        {
            var directCareCostPercentageQuery = QueryHelper.GetDirectCareCostPercentageQuery(accountMarket);
            var careCostPercentList = await dbHelper.FindAsync<DirectCareCostPercentageQueryModel>(directCareCostPercentageQuery);
            return careCostPercentList;
        }

        public async Task<List<ShippingLocationQueryModel>> GetShippingLocationForDirectQuote(string portfolio, string maintainanceType)
        {
            var shippingLocationQuery = QueryHelper.GetShippingLocationForDirectQuoteQuery(portfolio, maintainanceType);
            var shippingLocations = await dbHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);
            return shippingLocations;
        }

        public async Task<List<ShippingLocationQueryModel>> GetShippingLocationForIndirectQuote(string portfolio, string pricingCluster)
        {
            var shippingLocationQuery = QueryHelper.GetShippingLocationForIndirectQuoteQuery(portfolio, pricingCluster);
            var shippingLocations = await dbHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);
            return shippingLocations;
        }

        public async Task<List<ProductDiscountQueryModel>> GetProductDiscount(string market, List<string> discountCategories)
        {
            return await QueryHelper.ExecuteProductDiscountQuery(dbHelper, market, discountCategories);
        }

        public async Task<List<DiscountCategoryPriceListItemQueryModel>> GetDiscountCategoryPriceListItem(HashSet<string> discountCategories, string priceListName)
        {
            var discountCategoryPriceListItemQuery = QueryHelper.GetDiscountCategoryPriceListItemQuery(discountCategories, priceListName);
            var discountCategoryPriceListItems = await dbHelper.FindAsync<DiscountCategoryPriceListItemQueryModel>(discountCategoryPriceListItemQuery);
            return discountCategoryPriceListItems;
        }
    }
}
