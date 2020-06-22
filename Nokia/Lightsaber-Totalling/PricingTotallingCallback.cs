using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Formula;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using Apttus.Lightsaber.Pricing.Common.Models;
using PhillipsConversion.Nokia.Lightsaber_Totalling.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Nokia.Totalling
{
    public class PricingTotallingCallback : CodeExtensibility, IPricingTotallingCallback
    {
        private List<LineItemModel> cartLineItems = null;
        private Proposal proposal = null;
        private IDBHelper dBHelper = null;
        private decimal? defaultExchangeRate;
        private Dictionary<string, List<LineItemModel>> lineItemIRPMapDirect = new Dictionary<string, List<LineItemModel>>();
        private Dictionary<string, List<LineItemModel>> primaryNoLineItemList = new Dictionary<string, List<LineItemModel>>();

        public async Task BeforePricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            //IsLeo()
            decimal? minMaintPrice_EP = null;
            decimal? minMaintPrice = null;
            string isIONExistingContract_EP = string.Empty;
            string maintenanceType = string.Empty;

            cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList();
            proposal = new Proposal(aggregateCartRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL));
            dBHelper = GetDBHelper();

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(proposal.Get<string>(ProposalField.CurrencyIsoCode));
                defaultExchangeRate = (await dBHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;

                if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c) == Constants.NOKIA_IP_ROUTING && !proposal.Get<bool>(ProposalField.Is_List_Price_Only__c))
                {
                    var shippingLocationQuery = QueryHelper.GetShippingLocationForDirectQuoteQuery(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c), proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c));
                    var shippingLocations = await dBHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);

                    if (shippingLocations != null && shippingLocations.Count != 0)
                    {
                        if (proposal.Get<string>(ProposalField.CurrencyIsoCode) == Constants.USDCURRENCY)
                        {
                            minMaintPrice_EP = shippingLocations[0].Min_Maint_USD__c;
                        }
                        else if (proposal.Get<string>(ProposalField.CurrencyIsoCode) == Constants.EUR_CURR)
                        {
                            minMaintPrice_EP = shippingLocations[0].Min_Maint_EUR__c;
                        }
                    }

                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c) != null)
                    {
                        isIONExistingContract_EP = proposal.Get<string>(ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c);
                    }
                }
            }

            if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                var shippingLocationQuery = QueryHelper.GetShippingLocationForIndirectQuoteQuery(proposal.Get<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Portfolio__c), 
                    proposal.Get<string>(ProposalRelationshipField.NokiaCPQ_Maintenance_Accreditation__r_Pricing_Cluster__c));

                var shippingLocations = await dBHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);

                if (shippingLocations != null && shippingLocations.Count != 0)
                {
                    if (proposal.Get<string>(ProposalField.CurrencyIsoCode) == Constants.USDCURRENCY)
                    {
                        if (proposal.Get<bool>(ProposalField.NokiaCPQ_LEO_Discount__c))
                        {
                            minMaintPrice = shippingLocations[0].LEO_Mini_Maint_USD__c;
                        }
                        else
                        {
                            minMaintPrice = shippingLocations[0].Min_Maint_USD__c;
                        }
                    }
                    else
                    {
                        if (proposal.Get<bool>(ProposalField.NokiaCPQ_LEO_Discount__c))
                        {
                            minMaintPrice = shippingLocations[0].LEO_Mini_Maint_EUR__c;
                        }
                        else
                        {
                            minMaintPrice = shippingLocations[0].Min_Maint_EUR__c;
                        }
                    }
                }
            }

            Dictionary<decimal, LineItemModel> lineItemObjectMapDirect = new Dictionary<decimal, LineItemModel>();

            foreach (var cartLineItem in cartLineItems)
            {
                PriceListItemModel priceListItemModel = cartLineItem.GetPriceListItem();
                PriceListItem priceListItemEntity = priceListItemModel.Entity;

                if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
                {
                    string configType = GetConfigType(cartLineItem);
                    string irpMapKey = cartLineItem.GetLineNumber() + Constants.NOKIA_UNDERSCORE + cartLineItem.Entity.ChargeType;

                    if (lineItemIRPMapDirect.ContainsKey(irpMapKey))
                    {
                        lineItemIRPMapDirect[irpMapKey].Add(cartLineItem);
                    }
                    else
                    {
                        lineItemIRPMapDirect.Add(irpMapKey, new List<LineItemModel> { cartLineItem });
                    }

                    if (cartLineItem.GetLineType() != LineType.ProductService)
                    {
                        string linePrimaryNoChargeTypeKey = cartLineItem.Entity.ParentBundleNumber + Constants.NOKIA_UNDERSCORE + cartLineItem.Entity.ChargeType;
                        if (primaryNoLineItemList.ContainsKey(linePrimaryNoChargeTypeKey))
                        {
                            primaryNoLineItemList[linePrimaryNoChargeTypeKey].Add(cartLineItem);
                        }
                        else
                        {
                            primaryNoLineItemList.Add(linePrimaryNoChargeTypeKey, new List<LineItemModel> { cartLineItem });
                        }
                    }

                    if (string.Compare(configType, Constants.BUNDLE, true) == 0)
                    {
                        lineItemObjectMapDirect.Add(cartLineItem.Entity.PrimaryLineNumber, cartLineItem);
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task AfterPricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            await Task.CompletedTask;
        }

        public async Task FinishAsync(AggregateCartRequest aggregateCartRequest)
        {
            //defaultExchangeRate

            decimal? pricingGuidanceSettingThresold;
            List<DirectPortfolioGeneralSettingQueryModel> portfolioSettingList = new List<DirectPortfolioGeneralSettingQueryModel>();
            List<DirectCareCostPercentageQueryModel> careCostPercentList = new List<DirectCareCostPercentageQueryModel>();

            if (UsePricingGuidanceSettingThresold())
            {
                var pricingGuidanceSettingQuery = QueryHelper.GetPricingGuidanceSettingQuery(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c));
                pricingGuidanceSettingThresold = (await dBHelper.FindAsync<PricingGuidanceSettingQueryModel>(pricingGuidanceSettingQuery)).FirstOrDefault()?.Threshold__c;
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                var directPortfolioGeneralSettingQuery = QueryHelper.GetDirectPortfolioGeneralSettingQuery(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c));
                portfolioSettingList = await dBHelper.FindAsync<DirectPortfolioGeneralSettingQueryModel>(directPortfolioGeneralSettingQuery);

                var directCareCostPercentageQuery = QueryHelper.GetDirectCareCostPercentageQuery(proposal.Get<string>(ProposalField.Account_Market__c));
                careCostPercentList = await dBHelper.FindAsync<DirectCareCostPercentageQueryModel>(directCareCostPercentageQuery);
            }

            await Task.CompletedTask;
        }

        private bool IsLeo()
        {
            return string.Compare(proposal.Get<string>(ProposalField.Quote_Type__c), Constants.QUOTE_TYPE_INDIRECTCPQ, true) == 0
                && string.Compare(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c), Constants.NOKIA_1YEAR, true) == 0
                && proposal.Get<bool>(ProposalField.NokiaCPQ_LEO_Discount__c) == true;
        }

        private bool UsePricingGuidanceSettingThresold()
        {
            return string.Compare(proposal.Get<string>(ProposalField.Quote_Type__c), Constants.QUOTE_TYPE_DIRECTCPQ, true) == 0 &&
                    (string.Compare(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c), Constants.QTC, true) == 0 ||
                    (string.Compare(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c), Constants.NOKIA_IP_ROUTING, true) == 0 &&
                    proposal.Get<bool>(ProposalField.Is_List_Price_Only__c) == false));
        }

        private string GetConfigType(LineItemModel lineItemModel)
        {
            string configType = string.Empty;

            if (lineItemModel.GetLineType() == LineType.ProductService)
            {
                configType = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c);
            }
            else
            {
                configType = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c);
            }

            return configType;
        }

        private bool IsOptionLineFromSubBundle(LineItemModel lineItemModel)
        {
            if (lineItemModel.GetLineType() != LineType.Option)
                return false;

            return lineItemModel.Entity.ParentBundleNumber != lineItemModel.GetRootParentLineItem().GetPrimaryLineNumber();
        }
    }
}
