using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class PricingTotallingCallback : CodeExtensibility, IPricingTotallingCallback
    {
        private PricingTotallingCallbackHelper pcbHelper = null;

        public async Task BeforePricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            var cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).Select(s => new LineItem(s)).ToList();
            var proposalSO = Proposal.Create(aggregateCartRequest.Cart);
            pcbHelper = new PricingTotallingCallbackHelper(proposalSO, GetDBHelper(), GetPricingHelper());

            await pcbHelper.IncentiveAdjustmentUnitRounding(cartLineItems);
            await pcbHelper.SetDiscountWithAdjustmentSpread(cartLineItems);
        }

        public async Task AfterPricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            var cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).Select(s => new LineItem(s)).ToList();

            await pcbHelper.PopulatePLICustomFields(cartLineItems);
            await pcbHelper.SetPLIModel(cartLineItems);
            await pcbHelper.ComputeNetPriceAndNetAdjustment(cartLineItems);
            await pcbHelper.CalculatePricePointsForBundle(cartLineItems);
        }

        public async Task OnCartPricingCompleteAsync(AggregateCartRequest aggregateCartRequest)
        {
            var cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).Select(s => new LineItem(s)).ToList();

            await pcbHelper.SetPLIModel(cartLineItems);
            await pcbHelper.PopulateCustomFields(cartLineItems);
            await pcbHelper.SetRollupsAndThresholdFlags(aggregateCartRequest.Cart, cartLineItems);
        }
    }
}
