using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using Apttus.Lightsaber.Pricing.Common.Models;
using PhillipsConversion.Nokia.Lightsaber_BasePrice;
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

        public async Task BeforePricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            //IsLeo()
            cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList();
            proposal = new Proposal(aggregateCartRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL));
            dBHelper = GetDBHelper();

            if (string.Compare(proposal.Get<string>(ProposalField.Quote_Type__c), Constants.QUOTE_TYPE_DIRECTCPQ, true) == 0)
            {
                var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(proposal.Get<string>(ProposalField.CurrencyIsoCode));
                defaultExchangeRate = (await dBHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;
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
            if (UsePricingGuidanceSettingThresold())
            {
                var pricingGuidanceSettingQuery = QueryHelper.GetPricingGuidanceSettingQuery(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c));
                pricingGuidanceSettingThresold = (await dBHelper.FindAsync<PricingGuidanceSettingQueryModel>(pricingGuidanceSettingQuery)).FirstOrDefault()?.Threshold__c;
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
            return  string.Compare(proposal.Get<string>(ProposalField.Quote_Type__c), Constants.QUOTE_TYPE_DIRECTCPQ, true) == 0 && 
                    (string.Compare(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c), Constants.QTC, true) == 0 || 
                    (string.Compare(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c), Constants.NOKIA_IP_ROUTING, true) == 0 && 
                    proposal.Get<bool>(ProposalField.Is_List_Price_Only__c) == false));
        }
    }
}
