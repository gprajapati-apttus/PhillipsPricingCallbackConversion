﻿using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipsConversion.Totalling
{
    public class Phillips_TotallingPricingCallback : CodeExtensibilityFramework, IPricingTotallingCallback
    {
        private Lightsaber_TotallingPricingCallBackHelper_Ultra pcbHelper_Ultra = null;
        private List<LineItemModel> cartLineItems = null;

        public async Task BeforePricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList();
            var proposalSO = aggregateCartRequest.Cart.Get<Dictionary<string, object>>("Apttus_Proposal__Proposal__c");
            pcbHelper_Ultra = new Lightsaber_TotallingPricingCallBackHelper_Ultra(proposalSO, GetDBHelper());

            await pcbHelper_Ultra.incentiveAdjustmentUnitRounding(cartLineItems);
            await pcbHelper_Ultra.setDiscountWithAdjustmentSpread(cartLineItems);

            await Task.CompletedTask;
        }

        public async Task AfterPricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            foreach (var cartLineItem in cartLineItems)
            {
                cartLineItem.Entity.NetPrice = cartLineItem.GetLineType() == LineType.ProductService 
                                                    ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Solution_Offered_Price__c) 
                                                    : cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Offered_Price_c__c);
            }

            await pcbHelper_Ultra.computeNetAdjustment(cartLineItems);
            //await pcbHelper_Ultra.populateCustomFields(allLines);
            //await pcbHelper_Ultra.calculatePricePointsForBundle(allLines);

            await Task.CompletedTask;
        }

        public async Task FinishAsync(AggregateCartRequest aggregateCartRequest)
        {
            await Task.CompletedTask;
        }
    }
}