using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Formula;
using Apttus.Lightsaber.Pricing.Common.Messages;
using Apttus.Lightsaber.Pricing.Common.Models;
using PhillipsConversion.Nokia.Lightsaber_BasePrice.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Nokia.Pricing
{
    public class PricingBasePriceCallback : CodeExtensibility, IPricingBasePriceCallback
    {
        private List<LineItemModel> batchLineItems = null;
        private List<LineItemModel> cartLineItems = null;
        private Proposal proposal = null;
        private IDBHelper dBHelper = null;

        public async Task BeforePricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            batchLineItems = batchPriceRequest.LineItems.SelectMany(x => x.ChargeLines).ToList();
            cartLineItems = batchPriceRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList();

            proposal = new Proposal(batchPriceRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL));
            dBHelper = GetDBHelper();

            foreach (var cartLineItem in cartLineItems)
            {
                PriceListItemModel priceListItemModel = cartLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
                {

                }
            }

            await Task.CompletedTask;
        }

        public async Task OnPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            //IsLeo()
            //Is1Year()

            decimal? defaultExchangeRate = null;
            List<string> productList = new List<string>();
            Dictionary<string, decimal?> productPriceMap = new Dictionary<string, decimal?>();
            Dictionary<string, decimal?> productCostMap = new Dictionary<string, decimal?>();
            HashSet<string> ProductId_set = new HashSet<string>();
            Dictionary<string, ProductExtensionsQueryModel> productExtensitonMap = new Dictionary<string, ProductExtensionsQueryModel>();
            List<MNDirectProductMapQueryModel> MN_Direct_Products_List = new List<MNDirectProductMapQueryModel>();
            List<DirectPortfolioGeneralSettingQueryModel> portfolioSettingList = new List<DirectPortfolioGeneralSettingQueryModel>();
            Dictionary<string, MaintenanceAndSSPRuleQueryModel> maintenanceSSPRuleMap_EP = new Dictionary<string, MaintenanceAndSSPRuleQueryModel>();
            Dictionary<string, List<NokiaMaintenanceAndSSPRulesQueryModel>> maintenanceSSPRuleMap = new Dictionary<string, List<NokiaMaintenanceAndSSPRulesQueryModel>>();

            foreach (var batchLineItem in batchLineItems)
            {
                PriceListItemModel priceListItemModel = batchLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
                {
                    productList.Add(batchLineItem.GetProductOrOptionId());

                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c) == Constants.QTC
                        || (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c) == Constants.NOKIA_IP_ROUTING
                        && proposal.Get<bool>(ProposalField.Is_List_Price_Only__c) == false))
                    {
                        ProductId_set.Add(batchLineItem.GetProductOrOptionId());
                    }
                }
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(proposal.Get<string>(ProposalField.CurrencyIsoCode));
                defaultExchangeRate = (await dBHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;

                var productExtensionsQuery = QueryHelper.GetProductExtensionsQuery(ProductId_set, proposal.Get<string>(ProposalField.CurrencyIsoCode));
                List<ProductExtensionsQueryModel> Prod_extList = await dBHelper.FindAsync<ProductExtensionsQueryModel>(productExtensionsQuery);

                foreach (ProductExtensionsQueryModel Prod_ext in Prod_extList)
                {
                    productExtensitonMap.Add(Prod_ext.Product__c, Prod_ext);
                }

                if (Constants.AIRSCALE_WIFI_STRING.equalsIgnoreCase(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c)))
                {
                    var mnDirectProductMapQuery = QueryHelper.GetMNDirectProductMapQuery();
                    MN_Direct_Products_List = await dBHelper.FindAsync<MNDirectProductMapQueryModel>(productExtensionsQuery);
                }

                var directPortfolioGeneralSettingQuery = QueryHelper.GetDirectPortfolioGeneralSettingQuery(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c));
                portfolioSettingList = await dBHelper.FindAsync<DirectPortfolioGeneralSettingQueryModel>(directPortfolioGeneralSettingQuery);

                if (Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c)) && !proposal.Get<bool>(ProposalField.Is_List_Price_Only__c))
                {
                    var maintenanceAndSSPRuleQuery = QueryHelper.GetMaintenanceAndSSPRuleQuery(
                        proposal.Get<string>(ProposalRelationshipField.Apttus_Proposal__Account__r_GEOLevel1ID__c), proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c));
                    var maintenanceSSPRuleList = await dBHelper.FindAsync<MaintenanceAndSSPRuleQueryModel>(maintenanceAndSSPRuleQuery);

                    foreach(var maintenanceSSPRule in maintenanceSSPRuleList)
                    {
                        if (maintenanceSSPRule.Maintenance_Type__c != null && maintenanceSSPRule.Maintenance_Category__c != null)
                        {
                            var key = maintenanceSSPRule.Maintenance_Type__c + Constants.NOKIA_STRING_APPENDER + maintenanceSSPRule.Maintenance_Category__c;
                            maintenanceSSPRuleMap_EP.Add(key, maintenanceSSPRule);
                        }
                    }
                }
            }

            if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                var nokiaMaintenanceAndSSPRulesQuery = QueryHelper.GetNokiaMaintenanceAndSSPRulesQuery(proposal);
                var nokiaMaintenanceSSPRules = await dBHelper.FindAsync<NokiaMaintenanceAndSSPRulesQueryModel>(nokiaMaintenanceAndSSPRulesQuery);

                foreach (var nokiaMaintenanceSSPRule in nokiaMaintenanceSSPRules)
                {
                    if (nokiaMaintenanceSSPRule.NokiaCPQ_Pricing_Cluster__c.equalsIgnoreCase(proposal.Get<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c)) 
                        || nokiaMaintenanceSSPRule.NokiaCPQ_Pricing_Cluster__c.equalsIgnoreCase(proposal.Get<string>(ProposalRelationshipField.NokiaProductAccreditation__r_Pricing_Cluster__c)))
                    {
                        var key = nokiaMaintenanceSSPRule.NokiaCPQ_Pricing_Cluster__c + Constants.NOKIA_STRING_APPENDER + nokiaMaintenanceSSPRule.NokiaCPQ_Product_Discount_Category__c + 
                            Constants.NOKIA_STRING_APPENDER + nokiaMaintenanceSSPRule.NokiaCPQ_withPMA__c + Constants.NOKIA_STRING_APPENDER + nokiaMaintenanceSSPRule.NokiaCPQ_Maintenance_Type__c;

                        if (maintenanceSSPRuleMap.ContainsKey(key))
                        {
                            maintenanceSSPRuleMap[key].Add(nokiaMaintenanceSSPRule);
                        }
                        else
                        {
                            maintenanceSSPRuleMap.Add(key, new List<NokiaMaintenanceAndSSPRulesQueryModel> { nokiaMaintenanceSSPRule });
                        }
                    }
                }
            }

            var countryPriceListItemQuery = QueryHelper.GetCountryPriceListItemQuery(productList);
            var countryPriceListItems = await dBHelper.FindAsync<CountryPriceListItemQueryModel>(countryPriceListItemQuery);

            foreach (CountryPriceListItemQueryModel pli in countryPriceListItems)
            {
                productPriceMap.Add(pli.Apttus_Config2__ProductId__c, pli.Apttus_Config2__ListPrice__c);
                productCostMap.Add(pli.Apttus_Config2__ProductId__c, pli.Apttus_Config2__Cost__c);
            }

            await Task.CompletedTask;
        }

        public async Task AfterPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }

        private bool IsLeo()
        {
            return Is1Year() && proposal.Get<bool>(ProposalField.NokiaCPQ_LEO_Discount__c) == true;
        }

        private bool Is1Year()
        {
            return Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c))
                && Constants.NOKIA_1YEAR.equalsIgnoreCase(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c));
        }
    }
}
