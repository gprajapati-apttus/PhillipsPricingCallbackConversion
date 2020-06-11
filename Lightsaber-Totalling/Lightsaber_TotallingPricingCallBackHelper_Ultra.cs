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

        public async Task computeNetPriceAndNetAdjustment(List<LineItemModel> cartLineItems)
        {
            Dictionary<string, decimal> mapBundleAdjustments = new Dictionary<string, decimal>();
            Dictionary<string, pricePointsWrapper> mapPricePoints = new Dictionary<string, pricePointsWrapper>();

            foreach(var cartLineItem in cartLineItems)
            {
                cartLineItem.Entity.NetPrice = cartLineItem.GetLineType() == LineType.ProductService
                                    ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Solution_Offered_Price__c)
                                    : cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Offered_Price_c__c);

                var cartLineItemEntity = cartLineItem.Entity;
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType;
                
                if (cartLineItemEntity.OptionId == null)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier, new pricePointsWrapper(cartLineItem));
                    }
                }
                else if (cartLineItemEntity.OptionId != null && cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c) == Constants.CONFIGURATIONTYPE_BUNDLE)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber, new pricePointsWrapper(cartLineItem));
                    }
                }
                else
                {
                    if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).HasValue && !cartLineItem.IsOptional())
                    {
                        if (mapPricePoints.ContainsKey(uniqIdentifier))
                        {
                            mapPricePoints[uniqIdentifier].listPrice += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).Value;
                        }

                        if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                        {
                            mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].listPrice += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).Value;
                        }
                    }
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType;

                if (cartLineItemEntity.OptionId == null && cartLineItemEntity.AdjustmentType == null &&
                        cartLineItemEntity.AdjustmentAmount.HasValue && cartLineItemEntity.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                {
                    decimal netAdjPercentage = 0;
                    decimal extendedListPrice = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).HasValue && cartLineItem.IsOptional() ? 0 : cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c).Value;

                    var solutionListPrice = mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber) 
                        ? mapPricePoints[uniqIdentifier + cartLineItemEntity.PrimaryLineNumber].listPrice + extendedListPrice 
                        : mapPricePoints[uniqIdentifier].listPrice + extendedListPrice;

                    cartLineItem.Set(LineItemCustomField.APTS_Solution_list_Price_c__c, solutionListPrice);
                    
                    if (solutionListPrice.HasValue)
                    {
                        netAdjPercentage = -1 * formatPrecisionCeiling(cartLineItemEntity.AdjustmentAmount.Value) / formatPrecisionCeiling(solutionListPrice.Value) * 100;
                    }

                    mapBundleAdjustments.Add(uniqIdentifier, netAdjPercentage);
                }
            }

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;

                if (cartLineItemEntity.AdjustmentType != null && cartLineItemEntity.AdjustmentAmount != null)
                {
                    if (cartLineItemEntity.AdjustmentType == Constants.DISCOUNT_PERCENTAGE)
                    {
                        cartLineItemEntity.NetAdjustmentPercent = -1 * formatPrecisionCeiling(cartLineItemEntity.AdjustmentAmount.Value);

                    }
                    else if (cartLineItemEntity.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                    {
                        if (mapBundleAdjustments.ContainsKey(cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType) && cartLineItemEntity.AllocateGroupAdjustment == true)
                        {
                            cartLineItemEntity.NetAdjustmentPercent = mapBundleAdjustments[cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType];

                            if (cartLineItemEntity.OptionId != null)
                            {
                                cartLineItemEntity.AdjustmentAmount = 0;
                            }
                        }
                        else if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).HasValue && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).Value != 0)
                        {
                            cartLineItemEntity.NetAdjustmentPercent = -1 * formatPrecisionCeiling(cartLineItemEntity.AdjustmentAmount.Value) / formatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).Value) * 100;
                        }
                    }
                }
                else if (mapBundleAdjustments.ContainsKey(cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType)
                         && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).HasValue && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Philips_List_Price__c).Value > 0
                         && cartLineItemEntity.AllocateGroupAdjustment == true)
                {
                    cartLineItemEntity.NetAdjustmentPercent = mapBundleAdjustments[cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType];
                }

                cartLineItem.Set(LineItemCustomField.APTS_Net_Adjustment_Percent_c__c, cartLineItemEntity.NetAdjustmentPercent);
            }

            await Task.CompletedTask;
        }

        public async Task calculatePricePointsForBundle(List<LineItemModel> cartLineItems)
        {
            Dictionary<string, pricePointsWrapper> mapPricePoints = new Dictionary<string, pricePointsWrapper>();
            string bundleAdjustmentType = string.Empty;
            string uniqIdentifier_li = string.Empty;

            Dictionary<string, string> mapBundleAdjustment = new Dictionary<string, string>();

            foreach (var cartLineItem in cartLineItems)
            {
                var cartLineItemEntity = cartLineItem.Entity;
                string uniqIdentifier = cartLineItem.GetLineNumber() + cartLineItemEntity.ChargeType;
                uniqIdentifier_li = uniqIdentifier;

                var productType = cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ProductType__c);
                var configurationType = cartLineItem.Get<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c);

                if (productType == Constants.SOLUTION_SUBSCRIPTION && (cartLineItem.GetLineType() == LineType.ProductService || configurationType == Constants.CONFIGURATIONTYPE_BUNDLE))
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItemEntity.PrimaryLineNumber + cartLineItemEntity.ChargeType;
                }
                else if (productType == Constants.SOLUTION_SUBSCRIPTION && configurationType == Constants.CONFIGURATIONTYPE_OPTION)
                {
                    uniqIdentifier_li = cartLineItem.GetLineNumber() + cartLineItemEntity.ParentBundleNumber + cartLineItemEntity.ChargeType;
                }

                if (cartLineItemEntity.OptionId == null)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier, new pricePointsWrapper(cartLineItem));
                    }

                    mapBundleAdjustment.Add(uniqIdentifier_li, cartLineItemEntity.AdjustmentType);
                }
                else if (cartLineItemEntity.OptionId != null && configurationType == Constants.CONFIGURATIONTYPE_BUNDLE && productType == Constants.SOLUTION_SUBSCRIPTION)
                {
                    if (!mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber))
                    {
                        mapPricePoints.Add(uniqIdentifier + cartLineItemEntity.PrimaryLineNumber, new pricePointsWrapper(cartLineItem));
                    }

                    mapBundleAdjustment.Add(uniqIdentifier_li, cartLineItemEntity.AdjustmentType);
                }
                else
                {
                    if (mapPricePoints.ContainsKey(uniqIdentifier))
                    {
                        decimal? targetPrice = 0;
                        decimal? minPrice = 0;
                        decimal? optEscPrice = 0;
                        decimal? optionOfferedPrice = 0;
                        decimal? sellingTerm = cartLineItem.GetValuetOrDefault(LineItem.PropertyNames.SellingTerm, 1);
                        decimal? extQty = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c).HasValue && cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c).Value != 0 ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_Quantity__c).Value : 1;

                        bundleAdjustmentType = mapBundleAdjustment.GetValueOrDefault(uniqIdentifier_li) != null ? mapBundleAdjustment.GetValueOrDefault(uniqIdentifier_li) : string.Empty;

                        var extendedListPrice = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Extended_List_Price__c);

                        if (extendedListPrice.HasValue && !cartLineItem.IsOptional())
                        {
                            mapPricePoints[uniqIdentifier].listPrice += extendedListPrice.Value;

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].listPrice += extendedListPrice.Value;
                            }
                        }

                        decimal listPrice = extendedListPrice.HasValue ? extendedListPrice.Value : 0;
                        decimal contractDiscount = cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).HasValue ? cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).Value : 0;
                        decimal contractAmt = listPrice * contractDiscount / 100;

                        cartLineItem.Set(LineItemCustomField.APTS_Contract_Discount_Amount__c, contractAmt);
                        decimal? netAdjPercent = cartLineItemEntity.NetAdjustmentPercent;

                        decimal? stdAdj = getStrategicDiscount(netAdjPercent, cartLineItemEntity.AdjustmentType, listPrice);
                        decimal incAdjAmt = cartLineItemEntity.IncentiveAdjustmentAmount != null ? formatPrecisionCeiling(cartLineItemEntity.IncentiveAdjustmentAmount.Value) : 0;

                        decimal? unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                        decimal? strategicDiscountAmount = listPrice * stdAdj / 100;

                        if (productType != null && productType.Contains("Solution"))
                        {
                            if (cartLineItem.Get<string>(LineItemCustomField.APTS_Billing_Plan__c) != null && cartLineItem.Get<string>(LineItemCustomField.APTS_Billing_Plan__c) == "Annual")
                            {
                                var roundedUnitStrategicDisount = PricingHelper.ApplyRounding(unitStrategicDiscountAmount * 12, 2, RoundingMode.HALF_EVEN);
                                cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, roundedUnitStrategicDisount);
                            }
                            else
                            {
                                var roundedUnitStrategicDisount = PricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN);
                                cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, roundedUnitStrategicDisount);
                            }
                        }
                        else if (productType == "Service")
                        {
                            cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, unitStrategicDiscountAmount);
                            cartLineItem.Set(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c, strategicDiscountAmount);
                        }
                        else
                        {
                            var roundedUnitStrategicDisount = PricingHelper.ApplyRounding(unitStrategicDiscountAmount, 2, RoundingMode.HALF_EVEN);
                            cartLineItem.Set(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c, roundedUnitStrategicDisount);

                            cartLineItem.Set(LineItemCustomField.APTS_Strategic_Discount_Amount_c__c, roundedUnitStrategicDisount * sellingTerm * extQty);
                        }

                        if (productType != null && productType.Contains("Solution"))
                        {
                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_SAP_List_Price__c).HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].sapListPrice += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_SAP_List_Price__c).Value;
                                }
                                if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c).HasValue)
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].unitStrategicDiscountAmount 
                                        += cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Unit_Strategic_Discount_Amount__c).Value;
                                }
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c).HasValue)
                        {
                            targetPrice = formatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Target_Price__c).Value);
                            if (!cartLineItem.IsOptional() && targetPrice != null)
                            {
                                mapPricePoints[uniqIdentifier].targetPrice += targetPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].targetPrice += targetPrice;
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c).HasValue)
                        {
                            minPrice = formatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Minimum_Price__c).Value);
                            if (!cartLineItem.IsOptional())
                            {
                                mapPricePoints[uniqIdentifier].minPrice += minPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].minPrice += minPrice;
                                }
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price__c).HasValue)
                        {
                            optEscPrice = formatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Escalation_Price__c).Value);
                            if (!cartLineItem.IsOptional())
                            {
                                mapPricePoints[uniqIdentifier].preEscalationPrice += optEscPrice;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].preEscalationPrice += optEscPrice;
                                }
                            }
                        }

                        if (cartLineItemEntity.ListPrice.HasValue && cartLineItem.Get<bool?>(LineItemCustomField.APTS_Local_Bundle_Component_Flag__c) != true)
                        {
                            decimal? qty = cartLineItem.GetQuantity();

                            if (!cartLineItem.IsOptional())
                            {
                                var countryPriceListListPrice = cartLineItem.Get<decimal?>(LineItemCustomField.Apttus_Config2__PriceListItemId__r_APTS_Country_Pricelist_List_Price__c);

                                mapPricePoints[uniqIdentifier].optionPrice += (countryPriceListListPrice == null) ? formatPrecisionCeiling(cartLineItemEntity.ListPrice.Value) * qty : countryPriceListListPrice.Value * qty;

                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].optionPrice += (countryPriceListListPrice == null) ? formatPrecisionCeiling(cartLineItemEntity.ListPrice.Value) * qty : countryPriceListListPrice.Value * qty;
                                }
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Contract_Net_Price__c).HasValue && !cartLineItem.IsOptional())
                        {
                            mapPricePoints[uniqIdentifier].contractNetPrice += formatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Contract_Net_Price__c).Value);

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].contractNetPrice += formatPrecisionCeiling(cartLineItem.Get<decimal?>(LineItemCustomField.APTS_Contract_Net_Price__c).Value);
                            }
                        }

                        if (cartLineItemEntity.ExtendedPrice.HasValue && !cartLineItem.IsOptional())
                        {
                            mapPricePoints[uniqIdentifier].bundleExtendedPrice += mapPricePoints[uniqIdentifier].qty != null ? formatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value) * mapPricePoints[uniqIdentifier].qty : formatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value);

                            if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                            {
                                mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].bundleExtendedPrice += 
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].qty != null 
                                    ? formatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value) * mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].qty 
                                    : formatPrecisionCeiling(cartLineItemEntity.ExtendedPrice.Value);
                            }
                        }

                        if (cartLineItem.Get<decimal?>(LineItemCustomField.APTS_ContractDiscount__c).HasValue && cartLineItem.Get<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__ListPrice__c).HasValue)
                        {
                            pricePointsWrapper points = mapPricePoints[uniqIdentifier];
                            if (points.solutionContractDiscountAmount == null)
                            {
                                points.solutionContractDiscountAmount = 0;
                            }

                            if (!cartLineItem.IsOptional())
                            {
                                mapPricePoints[uniqIdentifier].solutionContractDiscountAmount += contractAmt;
                                if (mapPricePoints.ContainsKey(uniqIdentifier + cartLineItemEntity.ParentBundleNumber))
                                {
                                    mapPricePoints[uniqIdentifier + cartLineItemEntity.ParentBundleNumber].solutionContractDiscountAmount += contractAmt;
                                }
                            }
                        }

                        //GP: Start from here.....
                        if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null && liSO.Apttus_Config2__IncentiveAdjustmentAmount__c < 0 && !liSO.Apttus_Config2__IsOptional__c)
                        {
                            mapPricePoints.get(uniqIdentifier).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) * -1);

                            if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                                mapPricePoints.get(uniqIdentifier).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c * -1;

                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                            {
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) * -1);

                                if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                                    mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c * -1;
                            }
                        }
                        else if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null && liSO.Apttus_Config2__IncentiveAdjustmentAmount__c > 0 && !liSO.Apttus_Config2__IsOptional__c)
                        {
                            mapPricePoints.get(uniqIdentifier).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c));

                            if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                                mapPricePoints.get(uniqIdentifier).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c;

                            if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                            {
                                mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).incentiveAdjAmount += (formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c));

                                if (liSO.APTS_Unit_Incentive_Adj_Amount__c != null)
                                    mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).solutionUnitIncentiveAmount += liSO.APTS_Unit_Incentive_Adj_Amount__c;
                            }
                        }

                        if (liSO.APTS_Offered_Price_c__c != null)
                        {
                            optionOfferedPrice = liSO.APTS_Extended_List_Price__c != null ? liSO.APTS_Extended_List_Price__c : 0;
                            Decimal totalDiscounts = 0;
                            if (contractAmt != null)
                            {
                                totalDiscounts = totalDiscounts - contractAmt;
                            }
                            if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null)
                            {
                                Decimal incAdjAmt1 = formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c);
                                totalDiscounts = totalDiscounts + incAdjAmt1;
                            }
                            if (liSO.Apttus_Config2__NetAdjustmentPercent__c != null)
                            {
                                totalDiscounts = totalDiscounts - liSO.APTS_Strategic_Discount_Amount_c__c;
                            }
                            optionOfferedPrice = optionOfferedPrice + totalDiscounts;

                            if (!liSO.Apttus_Config2__IsOptional__c && optionOfferedPrice != null)
                            {
                                mapPricePoints.get(uniqIdentifier/* + liSO.Apttus_Config2__ParentBundleNumber__c*/).offeredPrice += optionOfferedPrice;
                                if (mapPricePoints.containsKey(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c))
                                    mapPricePoints.get(uniqIdentifier + liSO.Apttus_Config2__ParentBundleNumber__c).offeredPrice += optionOfferedPrice;
                            }
                        }

                        if (optEscPrice != null && optEscPrice != 0)
                            liSO.APTS_Escalation_Price_Attainment_c__c = (optionOfferedPrice / formatPrecisionCeiling(liSO.APTS_Escalation_Price__c)) * 100;
                        if (targetPrice != null && targetPrice != 0)
                            liSO.APTS_Target_Price_Attainment__c = (optionOfferedPrice / (formatPrecisionCeiling(liSO.APTS_Target_Price__c))) * 100;
                        if (minPrice != null && minPrice != 0)
                            liSO.APTS_Minimum_Price_Attainment_c__c = (optionOfferedPrice / (formatPrecisionCeiling(liSO.APTS_Minimum_Price__c))) * 100;

                        liSO.APTS_Price_Attainment_Color__c = returnColor(optionOfferedPrice, optEscPrice, minPrice, liSO.APTS_Extended_List_Price__c); //US#368606 Removed by Mario Yordanov on 15.10.2019
                    }
                }
            }
            

            for (Apttus_Config2.LineItem lineItemMO : allLines)
            {
                Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
                Decimal sellingTerm = liSO.Apttus_Config2__SellingTerm__c != null && liSO.Apttus_Config2__SellingTerm__c != 0 ? liSO.Apttus_Config2__SellingTerm__c : 1;
                Decimal extQty = liSO.APTS_Extended_Quantity__c != null ? liSO.APTS_Extended_Quantity__c : 1;

                String uniqIdentifier = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c/* + liSO.Apttus_Config2__PrimaryLineNumber__c*/;

                //GBS:US-520662        
                if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Solution Subscription' && (liSO.Apttus_Config2__LineType__c == 'Product/Service' || liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle'))
                    uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__PrimaryLineNumber__c + liSO.Apttus_Config2__ChargeType__c;
                else if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Solution Subscription' && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Option')
                    uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ParentBundleNumber__c + liSO.Apttus_Config2__ChargeType__c;
                else
                    uniqIdentifier_li = String.valueOf(liSO.Apttus_Config2__LineNumber__c) + liSO.Apttus_Config2__ChargeType__c;

                bundleAdjustmentType = mapBundleAdjustment.get(uniqIdentifier_li) != null ? mapBundleAdjustment.get(uniqIdentifier_li) : '';

                if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
                    uniqIdentifier += String.valueOf(liSO.Apttus_Config2__PrimaryLineNumber__c);

                if (mapPricePoints.containsKey(uniqIdentifier))
                {
                    if (liSO.Apttus_Config2__OptionId__c == null || liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
                    {
                        Decimal extendedListPrice = liSO.APTS_Extended_List_Price__c != null && liSO.Apttus_Config2__IsOptional__c ? 0 : liSO.APTS_Extended_List_Price__c;
                        if (liSO.APTS_Extended_List_Price__c != null)
                        {
                            liSO.APTS_Solution_list_Price_c__c = mapPricePoints.get(uniqIdentifier).listPrice + extendedListPrice;
                        }

                        Decimal listPrice = liSO.APTS_Extended_List_Price__c != null ? liSO.APTS_Extended_List_Price__c : 0;
                        Decimal bundleOfferedPrice = liSO.APTS_Extended_List_Price__c != null ? liSO.APTS_Extended_List_Price__c : 0;
                        Decimal totalDiscounts = 0;
                        Decimal contractAmt = 0;
                        Decimal unitContractAmt = 0;
                        //525556 - Modified by Bogdan Botcharov
                        if (liSO.APTS_ContractDiscount__c != null)
                        {
                            Decimal contractDiscount = liSO.APTS_ContractDiscount__c != null ? liSO.APTS_ContractDiscount__c : 0;
                            unitContractAmt = ((bundleOfferedPrice / extQty / sellingTerm) * contractDiscount / 100).setScale(2);
                            contractAmt = unitContractAmt * extQty * sellingTerm;
                            totalDiscounts = totalDiscounts - contractAmt;
                        }
                        if (liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null)
                        {
                            totalDiscounts = totalDiscounts + formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c);
                        }
                        if (liSO.Apttus_Config2__NetAdjustmentPercent__c != null)
                        {
                            Decimal stdAdj = getStrategicDiscount(liSO.Apttus_Config2__NetAdjustmentPercent__c, liSO.Apttus_Config2__AdjustmentType__c, listPrice, contractAmt);
                            Decimal incAdjAmt = liSO.Apttus_Config2__IncentiveAdjustmentAmount__c != null ? formatPrecisionCeiling(liSO.Apttus_Config2__IncentiveAdjustmentAmount__c) : 0;

                            //System.debug('pj 1352*****liSO.APTS_Strategic_Discount_Amount__c: '+liSO.APTS_Strategic_Discount_Amount__c);
                            //471758 - Added by Bogdan Botcharov on October 24 2019 - Calculate unit strategic discount to 2 decimal places and then multiply by Selling Term and Quantity
                            Decimal unitStrategicDiscountAmount = (listPrice / extQty / sellingTerm) * stdAdj / 100;
                            Decimal strategicDiscountAmount = listPrice * stdAdj / 100;
                            if (liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Service')
                            {
                                liSO.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount;
                                liSO.APTS_Strategic_Discount_Amount_c__c = strategicDiscountAmount;
                            }
                            else
                            {
                                liSO.APTS_Unit_Strategic_Discount_Amount__c = unitStrategicDiscountAmount.setScale(2);
                                liSO.APTS_Strategic_Discount_Amount_c__c = unitStrategicDiscountAmount.setScale(2) * sellingTerm * extQty;
                            }

                            totalDiscounts = totalDiscounts - liSO.APTS_Strategic_Discount_Amount_c__c;
                        }
                        bundleOfferedPrice = bundleOfferedPrice + totalDiscounts;

                        //462170 - Added by Bogdan Botcharov [11 November 2019]
                        String productType = liSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c;
                        if (productType != null && productType.contains('Solution') && liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c == 'Bundle')
                        {
                            if (liSO.APTS_Billing_Plan__c != null &&
                                (liSO.APTS_Billing_Plan__c == 'Monthly' || liSO.APTS_Billing_Plan__c == 'Quarterly' || liSO.APTS_Billing_Plan__c == 'Annual'))
                            {
                                liSO.APTS_SAP_List_Price__c = mapPricePoints.get(uniqIdentifier).sapListPrice;
                            }
                            liSO.APTS_Unit_Strategic_Discount_Amount__c = mapPricePoints.get(uniqIdentifier).unitStrategicDiscountAmount;
                        }

                        Decimal optEscPrice = formatPrecisionCeiling(liSO.APTS_Escalation_Price__c);
                        if (optEscPrice != null && optEscPrice != 0)
                        {
                            liSO.APTS_Escalation_Price_Attainment_c__c = (!ListTradeSpoo.contains(liSO.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c)) ? (bundleOfferedPrice / optEscPrice) * 100 : bundleOfferedPrice != 0 ? (optEscPrice / bundleOfferedPrice) * 100 : 0;  //[US #519734]-Changed for SPOO Trade products
                        }
                        Decimal minPrice = formatPrecisionCeiling(liSO.APTS_Minimum_Price__c);
                        if (minPrice != null && minPrice != 0)
                        {
                            liSO.APTS_Minimum_Price_Attainment_c__c = (!ListTradeSpoo.contains(liSO.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c)) ? (bundleOfferedPrice / minPrice) * 100 : bundleOfferedPrice != 0 ? (minPrice / bundleOfferedPrice) * 100 : 0;  //[US #519734]-Changed for SPOO Trade products
                        }
                        Decimal targetPrice = formatPrecisionCeiling(liSO.APTS_Target_Price__c);
                        if (targetPrice != null && targetPrice != 0)
                        {
                            liSO.APTS_Target_Price_Attainment__c = (!ListTradeSpoo.contains(liSO.Apttus_Config2__ProductId__r.APTS_SPOO_Type__c)) ? (bundleOfferedPrice / targetPrice) * 100 : bundleOfferedPrice != 0 ? (targetPrice / bundleOfferedPrice) * 100 : 0;  //[US #519734]-Changed for SPOO Trade products
                        }

                        //DE60436 - Added by Bogdan Botcharov
                        liSO.APTS_Price_Attainment_Color__c = returnColor(bundleOfferedPrice, optEscPrice, minPrice, liSO.APTS_Extended_List_Price__c); //US#368606 Removed by Mario Yordanov on 15.10.2019

                        Decimal solMinPrice = liSO.Apttus_Config2__IsOptional__c ? mapPricePoints.get(uniqIdentifier).minPrice : mapPricePoints.get(uniqIdentifier).minPrice + formatPrecisionCeiling(liSO.APTS_Minimum_Price__c);
                        liSO.APTS_Minimum_Price_Bundle__c = solMinPrice;
                        Decimal solEscPrice = liSO.Apttus_Config2__IsOptional__c ? mapPricePoints.get(uniqIdentifier).preEscalationPrice : mapPricePoints.get(uniqIdentifier).preEscalationPrice + optEscPrice;
                        liSO.APTS_Escalation_Price_Bundle__c = solEscPrice;
                        if (liSO.APTS_Country_Target_Price__c != null)
                        {
                            liSO.APTS_Target_Price_Bundle__c = liSO.Apttus_Config2__IsOptional__c ? mapPricePoints.get(uniqIdentifier).targetPrice : mapPricePoints.get(uniqIdentifier).targetPrice + formatPrecisionCeiling(liSO.APTS_Target_Price__c);
                        }
                        liSO.APTS_Option_List_Price__c = mapPricePoints.get(uniqIdentifier).optionPrice; //US368606 Removed by Mario Yordanov on 15.10.2019

                        liSO.APTS_Contract_Discount_Amount__c = mapPricePoints.get(uniqIdentifier).contractDiscountAmount;
                        liSO.APTS_Solution_Contract_Discount_Amount__c = mapPricePoints.get(uniqIdentifier).solutionContractDiscountAmount;
                        liSO.APTS_Incentive_Adjustment_Amount_Bundle__c = mapPricePoints.get(uniqIdentifier).incentiveAdjAmount;
                        liSO.APTS_Solution_Unit_Incentive_Adj_Amount__c = mapPricePoints.get(uniqIdentifier).solutionUnitIncentiveAmount;

                        Decimal solOfferedPrice = mapPricePoints.get(uniqIdentifier).offeredPrice;
                        liSO.APTS_Solution_Offered_Price_c__c = solOfferedPrice;
                        liSO.Apttus_Config2__NetPrice__c = solOfferedPrice + bundleOfferedPrice;

                        if (liSO.APTS_Solution_List_Price_c__c != null && liSO.APTS_Solution_Offered_Price__c != null)
                        {
                            liSO.APTS_Total_Resulting_Discount_Amount__c = liSO.APTS_Solution_List_Price_c__c - (liSO.APTS_Solution_Offered_Price_c__c + bundleOfferedPrice);
                        }

                        liSO.APTS_Solution_Price_Attainment_Color__c = returnColor(solOfferedPrice + bundleOfferedPrice, solEscPrice, solMinPrice, liSO.APTS_Extended_List_Price__c);
                    }
                }
            }
        }

        private decimal? getStrategicDiscount(decimal? netAdj, string adjType, decimal listPrice)
        {
            decimal? stdAdj = netAdj * -1;
            
            if(listPrice < 0 && adjType == Constants.DISCOUNT_PERCENTAGE) 
            {
                stdAdj = netAdj;
            }

            if (listPrice < 0 && adjType == Constants.DISCOUNT_AMOUNT)
            {
                stdAdj = netAdj;
            }

            return stdAdj;
        }

        public decimal formatPrecisionCeiling(decimal fieldValue)
        {
            var result = PricingHelper.ApplyRounding(fieldValue, 2, RoundingMode.UP);
            return result.Value;
        }
    }
}
