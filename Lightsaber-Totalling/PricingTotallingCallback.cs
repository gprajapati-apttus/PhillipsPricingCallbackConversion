using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using Apttus.Lightsaber.Pricing.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PricingTotallingCallback : CodeExtensibility, IPricingTotallingCallback
    {
        private PricingTotallingCallbackHelper pcbHelper = null;
        private List<LineItemModel> cartLineItems = null;

        public async Task BeforePricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList();
            var proposalSO = aggregateCartRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL);
            pcbHelper = new PricingTotallingCallbackHelper(proposalSO, GetDBHelper(), GetPricingHelper());

            await pcbHelper.IncentiveAdjustmentUnitRounding(cartLineItems);
            await pcbHelper.SetDiscountWithAdjustmentSpread(cartLineItems);
        }

        public async Task AfterPricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            await pcbHelper.PopulatePLICustomFields(cartLineItems);
            await pcbHelper.ComputeNetPriceAndNetAdjustment(cartLineItems);
            await pcbHelper.CalculatePricePointsForBundle(cartLineItems);
        }

        public async Task FinishAsync(AggregateCartRequest aggregateCartRequest)
        {
            await pcbHelper.PopulateCustomFields(cartLineItems);
            await pcbHelper.SetRollupsAndThresholdFlags(aggregateCartRequest.Cart, cartLineItems);
        }
    }
}
