using Apttus.Lightsaber.Pricing.Common.Models;
using Apttus.Lightsaber.Pricing.Common.Constants;
using System;
using System.Collections.Generic;
using Apttus.Lightsaber.Pricing.Common.Messages;
using System.Threading.Tasks;
using Apttus.Lightsaber.Extensibility.Framework.Library.Implementation;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Extensibility.Framework.Library.Common;
using PhillipsConversion.Lightsaber;
using System.Linq;

namespace PhillipsConversion.Totalling
{
    public class Lightsaber_TotallingPricingCallBackHelper_Ultra
    {
        private Dictionary<string, object> proposal;
        private IDBHelper dBHelper = null;

        public Lightsaber_TotallingPricingCallBackHelper_Ultra(Dictionary<string, object> proposal, IDBHelper dBHelper)
        {
            this.proposal = proposal;
            this.dBHelper = dBHelper;
        }

        public async Task incentiveAdjustmentUnitRounding(List<LineItemModel> cartLineItems)
        {
            foreach (LineItemModel cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;
                if (cartLineItemEntity.IncentiveBasePrice.HasValue && cartLineItemEntity.IncentiveBasePrice.Value != 0 && cartLineItemEntity.BasePrice.HasValue && cartLineItemEntity.BasePrice.Value != 0)
                {

                    decimal sellingTerm = cartLineItem.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                    decimal lineItemQ = cartLineItem.GetQuantity();

                    decimal? unitIncentiveAmount = cartLineItemEntity.BasePrice - cartLineItemEntity.IncentiveBasePrice;
                    unitIncentiveAmount = unitIncentiveAmount.HasValue ? PricingHelper.ApplyRounding(unitIncentiveAmount.Value, 2, RoundingMode.HALF_EVEN) : unitIncentiveAmount;

                    cartLineItem.Set(LineItemCustomField.APTS_Unit_Incentive_Adj_Amount__c, unitIncentiveAmount);
                    cartLineItemEntity.IncentiveBasePrice = cartLineItemEntity.BasePrice - unitIncentiveAmount;
                    cartLineItemEntity.IncentiveAdjustmentAmount = unitIncentiveAmount * lineItemQ * sellingTerm * -1;
                }
            }

            await Task.CompletedTask;
        }

        public async Task setDiscountWithAdjustmentSpread(List<LineItemModel> cartLineItems)
        {
            Dictionary<decimal, LineItemModel> bundleDiscountsDictionary = new Dictionary<decimal, LineItemModel>();

            foreach (var cartLineItem in cartLineItems)
            {
                decimal? promotionDiscount = null;
                
                if (cartLineItem.Entity.IncentiveAdjustmentAmount.HasValue)
                {
                    promotionDiscount = formatPrecisionCeiling(cartLineItem.Entity.IncentiveAdjustmentAmount.Value) * -1;
                }

                cartLineItem.Set(LineItemCustomField.APTS_Promotion_Discount_Amount_c__c, promotionDiscount);

                if (!cartLineItem.IsOptionLine())
                {
                    bundleDiscountsDictionary.Add(cartLineItem.GetLineNumber(), cartLineItem);
                }

                decimal? adjustmentsAmount = bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentAmount;

                if (adjustmentsAmount.HasValue)
                {
                    adjustmentsAmount = formatPrecisionCeiling(adjustmentsAmount.Value);
                }

                if(bundleDiscountsDictionary.ContainsKey(cartLineItem.GetLineNumber()) && cartLineItem.IsOptionLine() && cartLineItem.Entity.AllocateGroupAdjustment == true)
                {
                    if (bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentAmount != null &&
                           bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentType != null)
                    {
                        string bundleAdjTYpe = bundleDiscountsDictionary[cartLineItem.GetLineNumber()].Entity.AdjustmentType;
                        if (bundleAdjTYpe != Constants.PRICE_OVERRIDE && bundleAdjTYpe != Constants.DISCOUNT_AMOUNT)
                        {
                            cartLineItem.Entity.AdjustmentType = bundleAdjTYpe;
                            cartLineItem.Entity.AdjustmentAmount = adjustmentsAmount;
                        }
                    }
                    else if (cartLineItem.Entity.AdjustmentType == null)
                    {
                        cartLineItem.Entity.AdjustmentType = null;
                        cartLineItem.Entity.AdjustmentAmount = null;
                    }
                }
            }

            await Task.CompletedTask;
        }

        public async Task computeNetAdjustment(List<LineItemModel> cartLineItems)
        {
            Dictionary<string, decimal> mapBundleAdjustments = new Dictionary<string, decimal>();
            Dictionary<string, pricePointsWrapper> mapPricePoints = new Dictionary<string, pricePointsWrapper>();

            for (Apttus_Config2.LineItem lineItemMO : lineItems)
            {
                Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
                String uniqIdentifier = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c;
                if (liSO.Apttus_Config2__OptionId__c == null)
                {
                    if (!mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c))
                        mapPricePoints.put(uniqIdentifier, new pricePointsWrapper(liSO));
                }
                else if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
                {
                    if (!mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c))
                        mapPricePoints.put(uniqIdentifier + liSO.Apttus_Config2__PrimaryLineNumber__c, new pricePointsWrapper(liSO));
                }
                else
                {
                    if (liSO.APTS_Extended_List_Price__c != null && !liSO.Apttus_Config2__IsOptional__c)
                    {
                        if (mapPricePoints.containsKey(uniqIdentifier))
                            mapPricePoints.get(uniqIdentifier).listPrice += liSO.APTS_Extended_List_Price__c;
                        if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                            mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).listPrice += liSO.APTS_Extended_List_Price__c;
                    }
                }
            }
            for (Apttus_Config2.LineItem lineItemMO : lineItems)
            {
                Apttus_Config2__LineItem__c lineItemSO = lineItemMO.getLineItemSO();
                String uniqIdentifier = String.valueOf(lineItemSO.Apttus_Config2__LineNumber__c) + lineItemSO.Apttus_Config2__ChargeType__c;
                if (lineItemSO.Apttus_Config2__OptionId__c == null && lineItemSO.Apttus_Config2__AdjustmentType__c != null &&
                        lineItemSO.Apttus_Config2__AdjustmentAmount__c != null && lineItemSO.Apttus_Config2__AdjustmentType__c == 'Discount Amount')
                {
                    Decimal netAdjPercentage = 0;
                    Decimal extendedListPrice = lineItemSO.APTS_Extended_List_Price__c != null && lineItemSO.Apttus_Config2__IsOptional__c ? 0 : lineItemSO.APTS_Extended_List_Price__c;
                    //Mithilesh - calculated Sol List Price
                    lineItemSO.APTS_Solution_list_Price_c__c = mapPricePoints.containsKey(uniqIdentifier + lineItemSO.Apttus_Config2__PrimaryLineNumber__c) ? mapPricePoints.get(uniqIdentifier + lineItemSO.Apttus_Config2__PrimaryLineNumber__c).listPrice + extendedListPrice : mapPricePoints.get(uniqIdentifier).listPrice + extendedListPrice;
                    if (lineItemSO.APTS_Solution_List_Price__c != null)
                    {
                        netAdjPercentage = -1 * formatPrecisionCeiling(lineItemSO.Apttus_Config2__AdjustmentAmount__c) / formatPrecisionCeiling(lineItemSO.APTS_Solution_List_Price_c__c) * 100;
                    }
                    mapBundleAdjustments.put(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c, netAdjPercentage);
                }
            }

            for (Apttus_Config2.LineItem lineItemMO : lineItems)
            {
                Apttus_Config2__LineItem__c lineItemSO = lineItemMO.getLineItemSO();
                if (lineItemSO.Apttus_Config2__AdjustmentType__c != null &&
                        lineItemSO.Apttus_Config2__AdjustmentAmount__c != null)
                {
                    if (lineItemSO.Apttus_Config2__AdjustmentType__c == 'Discount %')
                    {
                        lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = -1 * formatPrecisionCeiling(lineItemSO.Apttus_Config2__AdjustmentAmount__c);

                    }
                    else if (lineItemSO.Apttus_Config2__AdjustmentType__c == 'Discount Amount')
                    {
                        if (!mapBundleAdjustments.isEmpty() && mapBundleAdjustments.containsKey(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c)
                           && lineItemSO.Apttus_Config2__AllocateGroupAdjustment__c == true)
                        {
                            lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = mapBundleAdjustments.get(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c);
                            if (lineItemSO.Apttus_Config2__OptionId__c != null)
                                lineItemSO.Apttus_Config2__AdjustmentAmount__c = 0;
                        }
                        else if (lineItemSO.APTS_Philips_List_Price__c != null && lineItemSO.APTS_Philips_List_Price__c != 0)
                        {
                            lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = -1 * formatPrecisionCeiling(lineItemSO.Apttus_Config2__AdjustmentAmount__c) / formatPrecisionCeiling(lineItemSO.APTS_Philips_List_Price__c) * 100;
                        }
                    }
                    //F254432 - Added by Bogdan Botcharov
                }
                else if (!mapBundleAdjustments.isEmpty() && mapBundleAdjustments.containsKey(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c)
                         && lineItemSO.APTS_Philips_List_Price__c != null && lineItemSO.APTS_Philips_List_Price__c > 0
                         && lineItemSO.Apttus_Config2__AllocateGroupAdjustment__c == true)
                {
                    lineItemSO.Apttus_Config2__NetAdjustmentPercent__c = mapBundleAdjustments.get(lineItemSO.Apttus_Config2__LineNumber__c + lineItemSO.Apttus_Config2__ChargeType__c);
                }
                //End F254432
                lineItemSO.APTS_Net_Adjustment_Percent_c__c = lineItemSO.Apttus_Config2__NetAdjustmentPercent__c;
            }
        }

        public decimal formatPrecisionCeiling(decimal fieldValue)
        {
            var result = PricingHelper.ApplyRounding(fieldValue, 2, RoundingMode.UP);
            return result.Value;
        }
    }
}
