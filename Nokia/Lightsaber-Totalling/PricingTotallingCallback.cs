using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Formula;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
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
        private IPricingHelper pricingHelper = null;
        private decimal? conversionRate;
        private Dictionary<string, List<LineItemModel>> lineItemIRPMapDirect = new Dictionary<string, List<LineItemModel>>();
        private Dictionary<string, List<LineItemModel>> primaryNoLineItemList = new Dictionary<string, List<LineItemModel>>();
        private Dictionary<string, LineItemModel> lineItemObjectMap = new Dictionary<string, LineItemModel>();

        public async Task BeforePricingCartAdjustmentAsync(AggregateCartRequest aggregateCartRequest)
        {
            //GP: Start Method
            //IsLeo()
            decimal? minMaintPrice_EP = null;
            decimal? minMaintPrice = null;
            string isIONExistingContract_EP = string.Empty;
            string maintenanceType = string.Empty;

            HashSet<string> sspFNSet = new HashSet<string>(Labels.FN_SSP_Product);

            cartLineItems = aggregateCartRequest.CartContext.LineItems.SelectMany(x => x.ChargeLines).ToList();
            proposal = new Proposal(aggregateCartRequest.Cart.Get<Dictionary<string, object>>(Constants.PROPOSAL));
            dBHelper = GetDBHelper();
            pricingHelper = GetPricingHelper();

            foreach (var lineItem in cartLineItems)
            {
                if (lineItem.GetLineType() == LineType.ProductService)
                {
                    lineItemObjectMap.Add(Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + lineItem.GetLineNumber(), lineItem);
                }
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery(proposal.Get<string>(ProposalField.CurrencyIsoCode));
                conversionRate = (await dBHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;

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

            Dictionary<int, LineItemModel> lineItemObjectMapDirect = new Dictionary<int, LineItemModel>();

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

            //GP: BeforePricing Method
            string isIONExistingContract = string.Empty;

            if (proposal.Get<string>(ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c) != null)
            {
                isIONExistingContract = proposal.Get<string>(ProposalField.NokiaCPQ_Existing_IONMaint_Contract__c);
            }

            List<string> pdcList = new List<string>(Labels.SRSPDC);

            if (Constants.QUOTE_TYPE_INDIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                decimal? totalExtendedMaintY1Price = 0;
                decimal? totalExtendedMaintY2Price = 0;
                decimal? totalExtendedSSPPrice = 0;
                decimal? totalExtendedSRSPrice = 0;

                decimal? totalOntQuantity = 0;
                decimal? totalFBAONTQty = 0;
                decimal? totalFBAP2PQty = 0;

                foreach (var cartLineItem in cartLineItems)
                {
                    if (cartLineItem.Entity.BasePrice.HasValue && cartLineItem.Entity.PriceIncludedInBundle == false &&
                        (!cartLineItem.Entity.BasePriceOverride.HasValue ||
                        (cartLineItem.Entity.BasePriceOverride.HasValue && cartLineItem.Entity.BasePriceOverride.Value != pricingHelper.ApplyRounding(cartLineItem.Entity.BasePrice.Value, 2, RoundingMode.HALF_UP))
                        ))
                    {
                        //Second time
                        cartLineItem.Entity.BasePriceOverride = pricingHelper.ApplyRounding(cartLineItem.Entity.BasePrice.Value, 2, RoundingMode.HALF_UP);
                        cartLineItem.Entity.PricingStatus = "Pending";
                    }

                    //GP: Commenting out below logic as this is already performed base price mode in beforepricing method 

                    //string partNumber = getPartNumber(item);
                    //if (partNumber != null && partNumber.equalsIgnoreCase(Constants.MAINTY2CODE))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 997;
                    //}
                    //else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.MAINTY1CODE))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 996;

                    //}
                    //else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.SSPCODE))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 998;
                    //}
                    //else if (partNumber != null && partNumber.equalsIgnoreCase(Constants.SRS))
                    //{
                    //    item.Apttus_Config2__LineSequence__c = 999;
                    //}

                    string productDiscountCat = getProductDiscountCategory(cartLineItem);
                    if ((cartLineItem.Entity.BasePrice.HasValue &&
                        cartLineItem.Entity.BasePrice.Value > 0 &&
                        cartLineItem.Get<bool?>(LineItemCustomField.NokiaCPQ_Spare__c) == false) ||
                        (cartLineItem.Get<bool?>(LineItemStandardField.Apttus_Config2__IsHidden__c) == true && cartLineItem.Entity.BasePrice.HasValue && cartLineItem.Entity.BasePrice.Value == 0))
                    {
                        if (cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c) != null)
                        {
                            totalExtendedMaintY1Price = totalExtendedMaintY1Price + cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c).Value;
                        }
                        if (cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c) != null)
                        {
                            totalExtendedMaintY2Price = totalExtendedMaintY2Price + cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c);
                        }

                        //Replace item.Portfolio_from_Quote_Line_Item__c formula field with 'this.proposalSO.NokiaCPQ_Portfolio__c', NokiaCPQ_Product_Discount_Category__c with value fetched from method
                        if (((productDiscountCat != null && !pdcList.isEmpty() && pdcList.Contains(productDiscountCat)) ||
                        (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase(Constants.NOKIA_NUAGE) &&
                        Constants.PRODUCTITEMTYPESOFTWARE.equalsIgnoreCase(cartLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c)))) || cartLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == true)
                        {
                            if (cartLineItem.Get<decimal?>(LineItemCustomField.Nokia_SRS_Base_Extended_Price__c) != null)
                            {
                                totalExtendedSRSPrice = totalExtendedSRSPrice + cartLineItem.Get<decimal?>(LineItemCustomField.Nokia_SRS_Base_Extended_Price__c).Value;
                            }
                        }
                    }

                    if ((productDiscountCat != null && !productDiscountCat.Contains(Constants.NOKIA_NFM_P) && cartLineItem.Get<bool?>(LineItemCustomField.NokiaCPQ_Spare__c) == false) ||
                        cartLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == true)
                    {
                        if (cartLineItem.Get<decimal?>(LineItemCustomField.Nokia_SSP_Base_Extended_Price__c) != null)
                        {
                            totalExtendedSSPPrice = totalExtendedSSPPrice + cartLineItem.Get<decimal?>(LineItemCustomField.Nokia_SSP_Base_Extended_Price__c).Value;
                        }
                    }

                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase("Fixed Access - POL") && cartLineItem.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity__c) != null)
                    {
                        totalOntQuantity = totalOntQuantity + cartLineItem.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity__c).Value;
                    }
                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase("Fixed Access - FBA"))
                    {
                        if (cartLineItem.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity_FBA__c) != null)
                        {
                            totalFBAONTQty = totalFBAONTQty + cartLineItem.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity_FBA__c).Value;
                        }
                        else if (cartLineItem.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity_P2P__c) != null)
                        {
                            totalFBAP2PQty = totalFBAP2PQty + cartLineItem.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity_P2P__c).Value;
                        }
                    }
                }

                foreach (var cartLineItem in cartLineItems)
                {
                    string partNumber = getPartNumber(cartLineItem);
                    bool isUpdate = false;

                    //Execute third time only for Product/Service lines to complete price calculations after quantity update
                    if (cartLineItem.Entity.BasePrice.HasValue && cartLineItem.Entity.PriceIncludedInBundle == false &&
                        cartLineItem.GetLineType() == LineType.ProductService)
                    {
                        isUpdate = true;
                    }

                    if (cartLineItem.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == false && cartLineItem.Get<bool?>(LineItemCustomField.Is_Contract_Pricing_2__c) == true &&
                        cartLineItem.Entity.PriceListItemId != null)
                    {
                        cartLineItem.Entity.BasePriceOverride = cartLineItem.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Partner_Price__c);
                        cartLineItem.Entity.BasePrice = cartLineItem.Entity.BasePriceOverride;
                        isUpdate = true;
                    }

                    //Varsha: End: Changes in Sprint 7 for Req 3354
                    if (partNumber != null && partNumber.Contains(Constants.MAINTY1CODE))
                    {
                        if (totalExtendedMaintY1Price > 0)
                        {
                            if (Constants.NOKIA_NO.equalsIgnoreCase(isIONExistingContract) && totalExtendedMaintY1Price < minMaintPrice)
                            {
                                totalExtendedMaintY1Price = minMaintPrice;
                            }
                        }

                        //GP: Instead of looking up on the cart lineitem, we are using proposal for same
                        if (proposal.Get<decimal?>(ProposalField.Maintenance_Y1__c) != null)
                        {
                            cartLineItem.Entity.BasePriceOverride = proposal.Get<decimal?>(ProposalField.Maintenance_Y1__c);
                            cartLineItem.Entity.BasePrice = proposal.Get<decimal?>(ProposalField.Maintenance_Y1__c);
                        }
                        else
                        {
                            cartLineItem.Entity.BasePriceOverride = totalExtendedMaintY1Price;
                            cartLineItem.Entity.BasePrice = totalExtendedMaintY1Price;
                        }

                        isUpdate = true;
                    }
                    else if (partNumber != null && partNumber.Contains(Constants.MAINTY2CODE))
                    {
                        if (proposal.Get<decimal?>(ProposalField.Maintenance_Y2__c) != null)
                        {
                            cartLineItem.Entity.BasePriceOverride = proposal.Get<decimal?>(ProposalField.Maintenance_Y2__c);
                            cartLineItem.Entity.BasePrice = proposal.Get<decimal?>(ProposalField.Maintenance_Y2__c);
                        }
                        else
                        {
                            cartLineItem.Entity.BasePriceOverride = totalExtendedMaintY2Price;
                            cartLineItem.Entity.BasePrice = totalExtendedMaintY2Price;
                        }

                        isUpdate = true;
                    }
                    else if (cartLineItem.Entity.ChargeType != null &&
                        !string.IsNullOrEmpty(cartLineItem.Entity.ChargeType) &&
                        cartLineItem.Entity.ChargeType.Contains(Constants.NOKIA_PRODUCT_NAME_SSP))
                    {
                        if (proposal.Get<decimal?>(ProposalField.SSP__c) != null)
                        {
                            cartLineItem.Entity.BasePriceOverride = proposal.Get<decimal?>(ProposalField.SSP__c);
                            cartLineItem.Entity.BasePrice = proposal.Get<decimal?>(ProposalField.SSP__c);
                        }
                        else if (IsLeo())
                        {
                            cartLineItem.Entity.BasePriceOverride = 0;
                            cartLineItem.Entity.BasePrice = 0; ;

                        }
                        else
                        {
                            cartLineItem.Entity.BasePriceOverride = totalExtendedSSPPrice;
                            cartLineItem.Entity.BasePrice = totalExtendedSSPPrice;
                        }

                        isUpdate = true;
                    }
                    else if (cartLineItem.Entity.ChargeType != null &&
                        !string.IsNullOrEmpty(cartLineItem.Entity.ChargeType) &&
                        cartLineItem.Entity.ChargeType.Contains(Constants.NOKIA_PRODUCT_NAME_SRS))
                    {
                        if (proposal.Get<decimal?>(ProposalField.SRS__c) != null)
                        {
                            cartLineItem.Entity.BasePriceOverride = proposal.Get<decimal?>(ProposalField.SRS__c);
                            cartLineItem.Entity.BasePrice = proposal.Get<decimal?>(ProposalField.SRS__c);
                        }
                        else if (IsLeo())
                        {
                            cartLineItem.Entity.BasePriceOverride = 0;
                            cartLineItem.Entity.BasePrice = 0;

                        }
                        else
                        {
                            cartLineItem.Entity.BasePriceOverride = totalExtendedSRSPrice;
                            cartLineItem.Entity.BasePrice = totalExtendedSRSPrice;
                        }
                        isUpdate = true;
                    }

                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase("Fixed Access - POL") && sspFNSet.Contains(partNumber))
                    {
                        cartLineItem.Entity.Quantity = Convert.ToInt32(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * totalOntQuantity;
                        isUpdate = true;
                    }

                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase("Fixed Access - FBA") && sspFNSet.Contains(partNumber))
                    {
                        cartLineItem.Entity.Quantity = Convert.ToInt32(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * totalFBAONTQty +
                            Convert.ToInt32(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * totalFBAP2PQty;

                        isUpdate = true;
                    }

                    if (isUpdate)
                    {
                        pricingHelper.UpdatePrice(cartLineItem);
                    }
                }
            }

            if (Constants.QUOTE_TYPE_DIRECTCPQ.equalsIgnoreCase(proposal.Get<string>(ProposalField.Quote_Type__c)))
            {
                //initialize the maintenance price
                decimal? totalExtendedMaintY1Price = 0;
                decimal? totalExtendedMaintY2Price = 0;

                Dictionary<string, LineItemModel> maintenanceLinesMap = new Dictionary<string, LineItemModel>();
                Dictionary<string, LineItemModel> maintenanceLinesMap_EP = new Dictionary<string, LineItemModel>();
                Dictionary<string, LineItemModel> productServiceMap = new Dictionary<string, LineItemModel>();
                Dictionary<string, LineItemModel> careSRSOptionMap = new Dictionary<string, LineItemModel>();

                Dictionary<decimal?, decimal?> linenumberQuantity = new Dictionary<decimal?, decimal?>();

                foreach (var cartLineItem in cartLineItems)
                {
                    string partNumber = getPartNumber(cartLineItem);

                    //Logic from Workflow: Enable Manual Adjustment For Options
                    if ((proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase(Constants.NOKIA_SOFTWARE) || 
                        (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase(Constants.NOKIA_IP_ROUTING) && 
                        proposal.Get<bool?>(ProposalField.Is_List_Price_Only__c) == false)) && !cartLineItem.Entity.ChargeType.equalsIgnoreCase(Constants.STANDARD))
                    {
                        cartLineItem.Entity.AllowManualAdjustment = false;
                    }
                    if (cartLineItem.Entity.BasePrice != null && cartLineItem.Entity.BasePrice > 0 && cartLineItem.Entity.ChargeType != null 
                        && cartLineItem.Entity.ChargeType.equalsIgnoreCase("Standard Price") && !(cartLineItem.Get<string>(LineItemCustomField.Source__c) == "BOMXAE" &&
                        proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c) == "QTC"))
                    {

                        decimal? convertedBasePriceTwoDecimal = pricingHelper.ApplyRounding((cartLineItem.Entity.BasePrice / conversionRate) * (proposal.Get<decimal?>(ProposalField.exchange_rate__c)), 2, RoundingMode.HALF_UP);
                        decimal? convertedBasePriceFiveDecimal = pricingHelper.ApplyRounding((cartLineItem.Entity.BasePrice / conversionRate) * (proposal.Get<decimal?>(ProposalField.exchange_rate__c)), 5, RoundingMode.HALF_UP);

                        if (cartLineItem.Entity.PriceListId == cartLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c) &&
                            cartLineItem.Entity.BasePriceOverride != convertedBasePriceTwoDecimal)
                        {
                            cartLineItem.Entity.BasePriceOverride = convertedBasePriceFiveDecimal;
                            cartLineItem.Entity.BasePriceOverride = convertedBasePriceTwoDecimal;
                            cartLineItem.Entity.PricingStatus = "Pending";
                        }
                        else if (cartLineItem.Entity.PriceListId != cartLineItem.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c) &&
                            cartLineItem.Entity.BasePriceOverride != pricingHelper.ApplyRounding(cartLineItem.Entity.BasePrice, 2, RoundingMode.HALF_UP))
                        {
                            cartLineItem.Entity.BasePriceOverride = pricingHelper.ApplyRounding(cartLineItem.Entity.BasePrice, 5, RoundingMode.HALF_UP);
                            cartLineItem.Entity.BasePriceOverride = pricingHelper.ApplyRounding(cartLineItem.Entity.BasePrice, 2, RoundingMode.HALF_UP);
                            cartLineItem.Entity.PricingStatus = "Pending";
                        }
                    }
                    //To set quantities for other charge types on main bundle
                    if (cartLineItem.Entity.ChargeType != null && cartLineItem.Entity.ChargeType.equalsIgnoreCase(Constants.STANDARD) && cartLineItem.GetLineType() == LineType.ProductService)
                    {
                        linenumberQuantity.Add(cartLineItem.GetLineNumber(), cartLineItem.GetQuantity());
                    }
                    //Map of Airscale wifi Maintenance lines
                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                    {
                        if (partNumber != null && partNumber.Contains(Constants.MAINTY1CODE))
                        {
                            maintenanceLinesMap.Add("Year1", cartLineItem);
                        }
                        else if (partNumber != null && partNumber.Contains(Constants.MAINTY2CODE))
                        {
                            maintenanceLinesMap.Add("Year2", cartLineItem);
                        }
                    }
                    //Start: creating maps to limit for loop iterations
                    if (cartLineItem.Entity.ChargeType != null && cartLineItem.GetLineType() == LineType.ProductService)
                    {
                        productServiceMap.Add(cartLineItem.Entity.Id, cartLineItem);
                    }
                    if (cartLineItem.Entity.ChargeType != null && 
                        (cartLineItem.Entity.ChargeType.equalsIgnoreCase(Constants.NOKIA_YEAR1_MAINTENANCE) || cartLineItem.Entity.ChargeType.equalsIgnoreCase(Constants.NOKIA_SRS)) && 
                        cartLineItem.GetLineType() != LineType.ProductService)
                    {
                        careSRSOptionMap.Add(cartLineItem.Entity.Id, cartLineItem);
                    }
                    //End
                }

                //R-6508
                if (Constants.NOKIA_IP_ROUTING.equalsIgnoreCase(proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c)) && proposal.Get<bool?>(ProposalField.Is_List_Price_Only__c) == false)
                {
                    foreach (var cartLineItem in cartLineItems)
                    {
                        string partNumber = getPartNumber(cartLineItem);

                        //Piece of code written below calculates Maintenance for each line item and saves it on line item for Direct Enterprise
                        if (partNumber != null &&
                              !partNumber.Contains(Constants.MAINTY1CODE) &&
                              !partNumber.Contains(Constants.MAINTY2CODE) &&
                              !partNumber.Contains(Constants.SSPCODE) &&
                              !partNumber.Contains(Constants.SRS) &&
                  cartLineItem.Get<bool?>(LineItemCustomField.Is_List_Price_Only__c) == false)
                        {
                            Dictionary<string, decimal?> maintenanceValueMap = calculateMaintenance_EP_Direct(cartLineItem, totalExtendedMaintY1Price, totalExtendedMaintY2Price, partNumber);
                            totalExtendedMaintY1Price = maintenanceValueMap["ExtendedMaintY1Price"];
                            totalExtendedMaintY2Price = maintenanceValueMap["ExtendedMaintY2Price"];
                        }

                        //Map Of Miantenance Line items for IP routing- Enterprise
                        if (partNumber != null && partNumber.Contains(Constants.MAINTY1CODE))
                        {
                            maintenanceLinesMap_EP.Add("Year1", cartLineItem);
                        }
                        else if (partNumber != null && partNumber.Contains(Constants.MAINTY2CODE))
                        {
                            maintenanceLinesMap_EP.Add("Year2", cartLineItem);
                        }
                    }

                    LineItemModel lineItemVarSO;
                    bool isUpdate = false;
                    if (maintenanceLinesMap_EP.Count > 0)
                    {
                        if (maintenanceLinesMap_EP.ContainsKey("Year1"))
                        {
                            lineItemVarSO = maintenanceLinesMap_EP["Year1"];

                            if (Constants.NOKIA_NO.equalsIgnoreCase(isIONExistingContract_EP) && minMaintPrice_EP != null && minMaintPrice_EP > totalExtendedMaintY1Price)
                            {
                                if (lineItemVarSO.Entity.BasePriceOverride != minMaintPrice_EP)
                                {
                                    lineItemVarSO.Entity.BasePriceOverride = minMaintPrice_EP;
                                    lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, minMaintPrice_EP);
                                    lineItemVarSO.Entity.LineSequence = 996;
                                    isUpdate = true;
                                }
                            }
                            else
                            {
                                if (lineItemVarSO.Entity.BasePriceOverride != totalExtendedMaintY1Price)
                                {
                                    lineItemVarSO.Entity.BasePriceOverride = totalExtendedMaintY1Price;
                                    lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, totalExtendedMaintY1Price);
                                    lineItemVarSO.Entity.LineSequence = 996;
                                    isUpdate = true;
                                }
                            }

                            if (isUpdate)
                            {
                                pricingHelper.UpdatePrice(lineItemVarSO);
                                isUpdate = true;
                            }
                        }
                        if (maintenanceLinesMap_EP.ContainsKey("Year2"))
                        {
                            lineItemVarSO = maintenanceLinesMap_EP["Year2"];

                            if (lineItemVarSO.Entity.BasePriceOverride != totalExtendedMaintY2Price)
                            {
                                lineItemVarSO.Entity.BasePriceOverride = totalExtendedMaintY2Price;
                                lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, totalExtendedMaintY2Price);
                                lineItemVarSO.Entity.LineSequence = 997;
                                isUpdate = true;
                            }

                            if (isUpdate)
                            {
                                pricingHelper.UpdatePrice(lineItemVarSO);
                                isUpdate = false;
                            }
                        }
                    }
                }
                //R-6508 End

                //Piyush Tawari Req 6229 Airscale Wifi Direct
                //Copy Discounts from Groups to SIs
                if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                {
                    foreach (var cartLineItem in cartLineItems)
                    {
                        string partNumber = getPartNumber(cartLineItem);
                        string configType = getConfigType(cartLineItem);

                        if (!configType.equalsIgnoreCase("Bundle") && cartLineItem.GetLineType() == LineType.Option
                                && lineItemObjectMapDirect != null && cartLineItem.Get<bool?>(LineItemCustomField.Advanced_pricing_done__c) == false)
                        {
                            var parentBundleNumber = cartLineItem.Entity.ParentBundleNumber.Value;

                            if (lineItemObjectMapDirect.ContainsKey(parentBundleNumber) && 
                                cartLineItem.Entity.ParentBundleNumber.Value != cartLineItem.GetLineNumber())
                            {
                                if (lineItemObjectMapDirect[parentBundleNumber].Entity.AdjustmentType == Constants.DISCOUNT_PERCENT)
                                {
                                    cartLineItem.Entity.AdjustmentType = lineItemObjectMapDirect[parentBundleNumber].Entity.AdjustmentType;
                                    cartLineItem.Entity.AdjustmentAmount = lineItemObjectMapDirect[parentBundleNumber].Entity.AdjustmentAmount;
                                }
                                else if (lineItemObjectMapDirect[parentBundleNumber].Entity.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                                {
                                    if ((lineItemObjectMapDirect[parentBundleNumber].Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CLP__c) != 0 || 
                                        lineItemObjectMapDirect[parentBundleNumber].Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CLP__c) != null) && 
                                        lineItemObjectMapDirect[parentBundleNumber].Entity.AdjustmentAmount != null)
                                    {
                                        decimal? disPer = 0;
                                        disPer = lineItemObjectMapDirect[parentBundleNumber].Entity.AdjustmentAmount / lineItemObjectMapDirect[parentBundleNumber].Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CLP__c) * 100;

                                        cartLineItem.Entity.AdjustmentType = Constants.DISCOUNT_PERCENT;
                                        cartLineItem.Entity.AdjustmentAmount = disPer;
                                    }
                                }
                                else if (lineItemObjectMapDirect[parentBundleNumber].Entity.AdjustmentType == null)
                                {
                                    cartLineItem.Entity.AdjustmentType = null;
                                    cartLineItem.Entity.AdjustmentAmount = null;
                                }
                            }
                        }
                        //Piece of cde written below calculates Maintenance for each line item and saves it on line item for Airscale Wifi
                        if (partNumber != null && !partNumber.Contains(Constants.MAINTY1CODE) && !partNumber.Contains(Constants.MAINTY2CODE))
                        {
                            Dictionary<string, decimal?> maintenanceValueMap = calculateMaintenance_MN_Direct(cartLineItem, totalExtendedMaintY1Price, totalExtendedMaintY2Price, partNumber, configType);

                            totalExtendedMaintY1Price = maintenanceValueMap["ExtendedMaintY1Price"];
                            totalExtendedMaintY2Price = maintenanceValueMap["ExtendedMaintY2Price"];
                        }
                        if (cartLineItem.Entity.ChargeType != null && !cartLineItem.Entity.ChargeType.equalsIgnoreCase(Constants.STANDARD) && cartLineItem.GetLineType() == LineType.ProductService)
                        {
                            if (linenumberQuantity.Count > 0 && linenumberQuantity.ContainsKey(cartLineItem.GetLineNumber()))
                            {
                                cartLineItem.Entity.Quantity = linenumberQuantity[cartLineItem.GetLineNumber()];

                            }
                        }
                    }

                    foreach (var cartLineItem in cartLineItems)
                    {
                        string configType = getConfigType(cartLineItem);

                        /**Piyush Tawari Start**/
                        //Req-6228 MN Airscale wifi - Price for the groups to be aggregated from SI
                        //For Direct MN Airscale Wifi
                        //checking if its Group
                        if (cartLineItem.Entity.ChargeType != null &&
                            cartLineItem.GetLineType() == LineType.Option && Constants.BUNDLE.equalsIgnoreCase(configType))
                        {
                            cartLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c, 0);
                            cartLineItem.Set(LineItemCustomField.NCPQ_Unitary_CLP__c, 0);
                            cartLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, 0);
                            cartLineItem.Set(LineItemCustomField.NokiaCPQ_Extended_CUP__c, 0);
                            cartLineItem.Set(LineItemCustomField.NokiaCPQ_Extended_CNP__c, 0);

                            var primaryLineNumberChargeTypeKey = cartLineItem.Entity.PrimaryLineNumber + Constants.NOKIA_UNDERSCORE + cartLineItem.Entity.ChargeType;

                            if (primaryNoLineItemList.ContainsKey(primaryLineNumberChargeTypeKey))
                            {
                                foreach (var optionItem in primaryNoLineItemList[primaryLineNumberChargeTypeKey])
                                {
                                    //Stamping IRP at Group Level
                                    if (optionItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_IRP__c) != null && optionItem.Entity.Quantity != null)
                                    {
                                        var unitaryIRP = pricingHelper.ApplyRounding(cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_IRP__c) + optionItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_IRP__c) * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                        cartLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, unitaryIRP);
                                    }
                                    
                                    //stamping CLP at Group level
                                    if (optionItem.Entity.BasePriceOverride != null)
                                    {
                                        var roundedBasePriceOverride = pricingHelper.ApplyRounding(optionItem.Entity.BasePriceOverride, 2, RoundingMode.HALF_UP);
                                        var unitaryCLP = pricingHelper.ApplyRounding(cartLineItem.Get<decimal?>(LineItemCustomField.NCPQ_Unitary_CLP__c) + roundedBasePriceOverride * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);

                                        cartLineItem.Set(LineItemCustomField.NCPQ_Unitary_CLP__c, unitaryCLP);
                                    }
                                    else if (optionItem.Entity.BasePrice != null)
                                    {
                                        var roundedBasePrice = pricingHelper.ApplyRounding(optionItem.Entity.BasePrice, 2, RoundingMode.HALF_UP);
                                        var unitaryCLP = pricingHelper.ApplyRounding(cartLineItem.Get<decimal?>(LineItemCustomField.NCPQ_Unitary_CLP__c) + roundedBasePrice * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                        cartLineItem.Set(LineItemCustomField.NCPQ_Unitary_CLP__c, unitaryCLP);
                                    }
                                    //Stamping CUP at Group level
                                    cartLineItem.Set(LineItemCustomField.NokiaCPQ_Extended_CUP__c, cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CUP__c) + optionItem.Entity.AdjustedPrice);
                                    //stamping CNP at Group Level
                                    cartLineItem.Set(LineItemCustomField.NokiaCPQ_Extended_CNP__c, cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CNP__c) + optionItem.Entity.NetPrice);
                                    
                                    if (optionItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c) != null && optionItem.Entity.Quantity != null)
                                    {
                                        var roundedUnitaryCost = pricingHelper.ApplyRounding(optionItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c), 2, RoundingMode.HALF_UP);
                                        var unitaryCost = pricingHelper.ApplyRounding(cartLineItem.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c) + roundedUnitaryCost * optionItem.GetQuantity(), 2, RoundingMode.HALF_UP);
                                        cartLineItem.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c, unitaryCost);
                                    }
                                }
                            }
                            if (cartLineItem.Entity.AdjustmentType == Constants.DISCOUNT_AMOUNT)
                                cartLineItem.Entity.BasePriceOverride = cartLineItem.Entity.AdjustmentAmount;
                            else
                                cartLineItem.Entity.BasePriceOverride = 0;
                        }/**Piyush Tawari End**/
                    }

                    //Calculate MaintY1 & MaintY2 for MN Direct(Airscale wifi)
                    LineItemModel lineItemVarSO;
                    if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING) && maintenanceLinesMap.Count > 0)
                    {
                        if (maintenanceLinesMap.ContainsKey("Year1"))
                        {
                            lineItemVarSO = maintenanceLinesMap["Year1"];
   
                            if (lineItemVarSO.Entity.PriceListId == lineItemVarSO.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c))
                            {
                                lineItemVarSO.Entity.BasePriceOverride = totalExtendedMaintY1Price;
                                lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, totalExtendedMaintY1Price);
                            }
                            else
                            {
                                lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, totalExtendedMaintY1Price);
                            }

                            lineItemVarSO.Entity.LineSequence = 997;
                            pricingHelper.UpdatePrice(lineItemVarSO);
                        }
                        if (maintenanceLinesMap.ContainsKey("Year2"))
                        {
                            lineItemVarSO = maintenanceLinesMap["Year2"];

                            if (!string.IsNullOrWhiteSpace(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) && Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) > 1)
                            {
                                if (lineItemVarSO.Entity.PriceListId == lineItemVarSO.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c))
                                {
                                    lineItemVarSO.Entity.BasePriceOverride = totalExtendedMaintY2Price;
                                    lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, totalExtendedMaintY2Price);
                                }
                                else
                                {
                                    lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, totalExtendedMaintY2Price);
                                }
                            }
                            else
                            {
                                lineItemVarSO.Entity.BasePriceOverride = 0;
                                lineItemVarSO.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, 0);
                            }
                            lineItemVarSO.Entity.LineSequence = 998;
                            pricingHelper.UpdatePrice(lineItemVarSO);
                        }
                    }
                }

                Dictionary<decimal?, List<decimal?>> BundleCareSRSPriceMap = new Dictionary<decimal?, List<decimal?>>();
                List<decimal?> careSRSList = new List<decimal?>();

                if (!proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase(Constants.AIRSCALE_WIFI_STRING))
                {
                    foreach (var item in productServiceMap.Values)
                    {
                        if (!item.Entity.ChargeType.equalsIgnoreCase(Constants.STANDARD))
                        {
                            if (linenumberQuantity.Count > 0 && linenumberQuantity.ContainsKey(item.GetLineNumber()))
                            {
                                item.Entity.Quantity = linenumberQuantity[item.GetLineNumber()];
                            }
                        }
                        //Care & SRS calculation for NSW
                        if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase("Nokia Software"))
                        {
                            if (item.Entity.ChargeType.equalsIgnoreCase(Constants.STANDARD) && lineItemIRPMapDirect.ContainsKey(item.GetLineNumber() + "_" + item.Entity.ChargeType))
                            {
                                careSRSList = careSRSCalculationForNSW(item);
                                if (!careSRSList.isEmpty())
                                {
                                    BundleCareSRSPriceMap.Add(item.GetLineNumber(), careSRSList);
                                }
                            }
                        }
                    }

                }

                //Stamp prices to Care & SRS
                List<string> careProActiveList = Labels.NokiaCPQ_Care_Proactive;
                List<string> careAdvanceList = Labels.NokiaCPQ_Care_Advance;
                HashSet<string> careProactiveSet = new HashSet<string>(careProActiveList);
                HashSet<string> careAdvanceSet = new HashSet<string>(careAdvanceList);

                if (proposal.Get<string>(ProposalField.NokiaCPQ_Portfolio__c).equalsIgnoreCase("Nokia Software"))
                {
                    foreach (var item in careSRSOptionMap.Values)
                    {
                        if (item.Entity.ChargeType.equalsIgnoreCase(Constants.NOKIA_YEAR1_MAINTENANCE))
                        {
                            var productCode = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_ProductCode);

                            if (item.Entity.OptionId != null && careAdvanceSet.Contains(productCode) && BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && 
                                BundleCareSRSPriceMap[item.GetLineNumber()].Count > 0 &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()][0] != null &&
                                item.Get<decimal?>(LineItemCustomField.NokiaCPQ_CareSRSBasePrice__c) != pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.10m), 2, RoundingMode.HALF_UP))
                            {
                                var careSRSBasePrice = pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.10m), 2, RoundingMode.HALF_UP);

                                item.Entity.BasePriceOverride = careSRSBasePrice;
                                item.Set(LineItemCustomField.NokiaCPQ_CareSRSBasePrice__c, careSRSBasePrice);
                                item.Entity.PricingStatus = "Pending";
                            }
                            else if (item.Entity.OptionId != null && careProactiveSet.Contains(productCode) &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()].Count > 0 &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()][0] != null &&
                                item.Get<decimal?>(LineItemCustomField.NokiaCPQ_CareSRSBasePrice__c) != pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.15m), 2, RoundingMode.HALF_UP))
                            {
                                var careSRSBasePrice = pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][0] * 0.15m), 2, RoundingMode.HALF_UP);

                                item.Entity.BasePriceOverride = careSRSBasePrice;
                                item.Set(LineItemCustomField.NokiaCPQ_CareSRSBasePrice__c, careSRSBasePrice);
                                item.Entity.PricingStatus = "Pending";
                            }
                        }
                        else if (item.Entity.ChargeType.equalsIgnoreCase(Constants.NOKIA_SRS))
                        {
                            if (BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()].Count > 1 &&
                                BundleCareSRSPriceMap.ContainsKey(item.GetLineNumber()) && BundleCareSRSPriceMap[item.GetLineNumber()][1] != null &&
                                item.Get<decimal?>(LineItemCustomField.NokiaCPQ_SRSBasePrice__c) != pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][1] * 0.15m), 2, RoundingMode.HALF_UP))
                            {
                                var srsBasePrice = pricingHelper.ApplyRounding((BundleCareSRSPriceMap[item.GetLineNumber()][1] * 0.15m), 2, RoundingMode.HALF_UP);

                                item.Entity.BasePriceOverride = srsBasePrice;
                                item.Set(LineItemCustomField.NokiaCPQ_SRSBasePrice__c, srsBasePrice);
                                item.Entity.PricingStatus = "Pending";
                            }
                        }
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

        private string getPartNumber(LineItemModel lineItemModel)
        {
            string partNumber = string.Empty;
            if (lineItemModel.Get<bool?>(LineItemCustomField.is_Custom_Product__c) == true)
            {
                partNumber = lineItemModel.Get<string>(LineItemCustomField.Custom_Product_Code__c);
            }
            else
            {
                if (lineItemModel.GetLineType() == LineType.ProductService)
                {
                    partNumber = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_ProductCode);
                }
                else
                {
                    partNumber = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_ProductCode);
                }
            }

            return partNumber;
        }

        string getConfigType(LineItemModel lineItemModel)
        {
            string configType = string.Empty;

            if (lineItemModel.GetLineType() == LineType.ProductService)
            {
                configType = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c);
                //configType = String.valueOf(item.Apttus_Config2__ProductId__r.Apttus_Config2__ConfigurationType__c);
            }
            else
            {
                configType = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c);
                //configType = String.valueOf(item.Apttus_Config2__OptionId__r.Apttus_Config2__ConfigurationType__c);
            }

            return configType;
        }

        private string getProductDiscountCategory(LineItemModel lineItemModel)
        {
            string productDiscountCat = string.Empty;

            if (lineItemModel.GetLineType() == LineType.ProductService)
            {
                productDiscountCat = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Product_Discount_Category__c);
            }
            else
            {
                productDiscountCat = lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Product_Discount_Category__c);
            }

            return productDiscountCat;
        }

        //R-6508
        /* Method Name   : calculateMaintenance_EP_Direct
        * Developer	  : Accenture
        * Description	:  The method calculates Maintenance per line item for Direct Enterprise Quotes */
        private Dictionary<string, decimal?> calculateMaintenance_EP_Direct(LineItemModel item, decimal? totalExtendedMaintY1Price, decimal? totalExtendedMaintY2Price, string partNumber)
        {
            Dictionary<string, decimal?> maintenanceValueMap = new Dictionary<string, decimal?>();
            string itemName = item.Entity.ChargeType;

            decimal? ExtendedMaintY1Price = totalExtendedMaintY1Price;
            decimal? ExtendedMaintY2Price = totalExtendedMaintY2Price;
            int quantityBundle = 1;

            if (partNumber != null && !partNumber.Contains(Constants.MAINTY1CODE) &&
                !partNumber.Contains(Constants.MAINTY2CODE) &&
                !partNumber.Contains(Constants.SSPCODE) &&
                !partNumber.Contains(Constants.SRS))
            {
                if (item.GetLineType() == LineType.Option)
                {
                    var key = Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.GetLineNumber();
                    if (lineItemObjectMap.ContainsKey(key) && lineItemObjectMap[key] != null)
                    {
                        if (lineItemObjectMap[key].Entity.Quantity != null)
                        {
                            quantityBundle = Convert.ToInt32(Math.Ceiling(lineItemObjectMap[key].Entity.Quantity.Value));
                        }
                    }
                }
            }

            if (item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c) != null && item.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y1_Per__c) != null &&
                item.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y2_Per__c) != null)
            {
                item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding(item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c) * item.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y1_Per__c) * 0.01m, 2, RoundingMode.HALF_UP) * quantityBundle);
                item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, pricingHelper.ApplyRounding(item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c) * item.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y2_Per__c) * 0.01m, 2, RoundingMode.HALF_UP) * quantityBundle);
            }

            ExtendedMaintY1Price = ExtendedMaintY1Price + pricingHelper.ApplyRounding(item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c), 2, RoundingMode.HALF_UP);
            ExtendedMaintY2Price = ExtendedMaintY2Price + pricingHelper.ApplyRounding(item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c), 2, RoundingMode.HALF_UP);

            maintenanceValueMap.Add("ExtendedMaintY1Price", ExtendedMaintY1Price);
            maintenanceValueMap.Add("ExtendedMaintY2Price", ExtendedMaintY2Price);
            
            return maintenanceValueMap;

        }

        private Dictionary<string, decimal?> calculateMaintenance_MN_Direct(LineItemModel item, decimal? totalExtendedMaintY1Price, decimal? totalExtendedMaintY2Price, string partNumber, string configType)
        {
            Dictionary<string, decimal?> maintenanceValueMap = new Dictionary<string, decimal?>();
            decimal? ExtendedMaintY1Price = totalExtendedMaintY1Price;
            decimal? ExtendedMaintY2Price = totalExtendedMaintY2Price;
            decimal? groupQuantity = 1;
            decimal? mainBundleQuantity = 1;

            if (item.Get<bool?>(LineItemCustomField.NokiaCPQ_Spare__c) == false)
            {
                string itemType = getItemType(item);
                if (item.GetLineType() == LineType.Option &&
                Constants.NOKIA_STANDALONE.equalsIgnoreCase(configType))
                {
                    if (lineItemObjectMap.ContainsKey(Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.GetLineNumber()))
                    {
                        mainBundleQuantity = lineItemObjectMap[Constants.NOKIA_PRODUCT_SERVICES + Constants.NOKIA_UNDERSCORE + item.GetLineNumber()].Entity.Quantity;
                    }

                    if (item.Entity.ExtendedQuantity != null && item.Entity.Quantity != 0)
                    {
                        groupQuantity = item.Entity.ExtendedQuantity / item.Entity.Quantity;
                    }
                }

                if (itemType != null && itemType.equalsIgnoreCase("Software"))
                {
                    if (item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c) != null && item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c).equalsIgnoreCase("Controller"))
                    {
                        decimal? basicYear1 = 0;
                        decimal? basicYear2 = 0;
                        decimal? enhanceYear1 = 0;
                        decimal? enhanceYear2 = 0;
                        decimal? enhanceEmergencyYear1 = 0;
                        decimal? enhanceEmergencyYear2 = 0;

                        //Software Maintenance Basic = Controller SW IRP * 25%  
                        //Software Maintenance Enhanced = Software Maintenance Basic Price +25%
                        //Software Maintenance Enhanced Emergency = Software Maintenance Enhanced + 25%
                        //multiplication by no of years

                        if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("1"))
                        {
                            basicYear1 = pricingHelper.ApplyRounding((item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c) * 0.25m), 2, RoundingMode.HALF_UP);
                            basicYear2 = 0;
                            enhanceYear1 = pricingHelper.ApplyRounding((basicYear1 + (basicYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceYear2 = 0;
                            enhanceEmergencyYear1 = pricingHelper.ApplyRounding((enhanceYear1 + (enhanceYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear2 = 0;
                        }

                        if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("2") || proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("3")))
                        {
                            basicYear1 = pricingHelper.ApplyRounding((item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c) * 0.25m), 2, RoundingMode.HALF_UP);
                            basicYear2 = pricingHelper.ApplyRounding(((basicYear1 * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.85m)) - basicYear1), 2, RoundingMode.HALF_UP);
                            enhanceYear1 = pricingHelper.ApplyRounding((basicYear1 + (basicYear1 * 0.25m)),2, RoundingMode.HALF_UP);
                            enhanceYear2 = pricingHelper.ApplyRounding((((basicYear1 + (basicYear1 * 0.25m)) * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.85m)) - enhanceYear1), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear1 = pricingHelper.ApplyRounding((enhanceYear1 + (enhanceYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear2 = pricingHelper.ApplyRounding((((enhanceYear1 + (enhanceYear1 * 0.25m)) * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.85m)) - enhanceEmergencyYear1), 2, RoundingMode.HALF_UP);
                        }

                        else if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("4") || proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("5")))
                        {
                            basicYear1 = pricingHelper.ApplyRounding((item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c) * 0.25m), 2, RoundingMode.HALF_UP);
                            basicYear2 = pricingHelper.ApplyRounding(((basicYear1 * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.70m)) - basicYear1), 2, RoundingMode.HALF_UP);
                            enhanceYear1 = pricingHelper.ApplyRounding((basicYear1 + (basicYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceYear2 = pricingHelper.ApplyRounding((((basicYear1 + (basicYear1 * 0.25m)) * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.70m)) - enhanceYear1), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear1 = pricingHelper.ApplyRounding((enhanceYear1 + (enhanceYear1 * 0.25m)), 2, RoundingMode.HALF_UP);
                            enhanceEmergencyYear2 = pricingHelper.ApplyRounding((((enhanceYear1 + (enhanceYear1 * 0.25m)) * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.70m)) - enhanceEmergencyYear1), 2, RoundingMode.HALF_UP);
                        }

                        if (proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c) != null && proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c).equalsIgnoreCase("MN GS TSS Basic"))
                        {
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, basicYear1);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, basicYear2);
                        }
                        else if (proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c) != null && proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c).equalsIgnoreCase("MN GS TSS Enhanced"))
                        {
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, enhanceYear1);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, enhanceYear2);
                        }
                        else if (proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c) != null && proposal.Get<string>(ProposalField.NokiaCPQ_Maintenance_Type__c).equalsIgnoreCase("MN GS TSS Enhanced Emergency"))
                        {
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, enhanceEmergencyYear1);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, enhanceEmergencyYear2);
                        }
                    }
                }

                else if (itemType != null && itemType.equalsIgnoreCase("Hardware"))
                {
                    if (item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c) != null && item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c).equalsIgnoreCase("Access Point"))
                    {
                        //multiplication by no of years
                        if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("1"))
                        {
                            var extendedIRP = item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding((extendedIRP * 0.02m) + (extendedIRP * 0.053m), 2, RoundingMode.HALF_UP));
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, 0);
                        }

                        else if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("2") || proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("3")))
                        {
                            var extendedIRP = item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding((extendedIRP * 0.02m) + (extendedIRP * 0.053m), 2, RoundingMode.HALF_UP));
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, pricingHelper.ApplyRounding(((extendedIRP * 0.02m * ((Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 1.00m) - 1)) + (extendedIRP * 0.053m * ((Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.85m) - 1))), 2, RoundingMode.HALF_UP));
                        }

                        else if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("4") || proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("5")))
                        {
                            var extendedIRP = item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding(((extendedIRP * 0.02m) + (extendedIRP * 0.053m)), 2, RoundingMode.HALF_UP));
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, pricingHelper.ApplyRounding(((extendedIRP * 0.02m * ((Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.85m) - 1)) + (extendedIRP * 0.053m * ((Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.70m) - 1))), 2, RoundingMode.HALF_UP));
                        }
                    }
                    else if (item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c) != null && item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c).equalsIgnoreCase("Controller"))
                    {
                        if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("1"))
                        {
                            var extendedIRP = item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding((extendedIRP * 0.02m), 2, RoundingMode.HALF_UP));
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, 0);
                        }

                        else if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("2") || proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("3")))
                        {
                            var extendedIRP = item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding(extendedIRP * 0.02m, 2, RoundingMode.HALF_UP));
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, pricingHelper.ApplyRounding(extendedIRP * 0.02m * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) - 1), 2, RoundingMode.HALF_UP));
                        }

                        else if (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c) != null && (proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("4") || proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c).equalsIgnoreCase("5")))
                        {
                            var extendedIRP = item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c);
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding((extendedIRP * 0.02m), 2, RoundingMode.HALF_UP));
                            item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, pricingHelper.ApplyRounding(extendedIRP * 0.02m * ((Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) * 0.85m) - 1), 2, RoundingMode.HALF_UP));
                        }
                    }
                }

                if (item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c) != null && item.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c).equalsIgnoreCase("Third Party Wavespot"))
                {
                    if (!string.IsNullOrWhiteSpace(partNumber))
                    {
                        var extendedCLP = item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CLP__c);
                        item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, pricingHelper.ApplyRounding((extendedCLP * 0.18m * 0.85m), 2, RoundingMode.HALF_UP));
                        item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, pricingHelper.ApplyRounding(extendedCLP * 0.18m * 0.85m * (Convert.ToDecimal(proposal.Get<string>(ProposalField.NokiaCPQ_No_of_Years__c)) - 1), 2, RoundingMode.HALF_UP));
                    }
                }
            }
            else
            {
                item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, 0);
                item.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, 0);
            }

            ExtendedMaintY1Price = ExtendedMaintY1Price + pricingHelper.ApplyRounding((item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c) * groupQuantity * mainBundleQuantity), 2, RoundingMode.HALF_UP);
            ExtendedMaintY2Price = ExtendedMaintY2Price + pricingHelper.ApplyRounding((item.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c) * groupQuantity * mainBundleQuantity), 2, RoundingMode.HALF_UP);

            maintenanceValueMap.Add("ExtendedMaintY1Price", ExtendedMaintY1Price);
            maintenanceValueMap.Add("ExtendedMaintY2Price", ExtendedMaintY2Price);

            return maintenanceValueMap;
        }

        private string getItemType(LineItemModel item)
        {
            string itemType = string.Empty;

            if (item.GetLineType() == LineType.ProductService)
            {
                itemType = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c);
            }
            else
            {
                itemType = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Item_Type__c);
            }

            return itemType;
        }

        /*  Method Name : careSRSCalculationForNSW
	        Description : The method calculates Care/SRS for NSW quotes
	        */
        List<decimal?> careSRSCalculationForNSW(LineItemModel item)
        {
            List<decimal?> careSRSList = new List<decimal?>();
            decimal? sumOfCareItemsRTUOEM = 0;
            decimal? sumOfCareItemsOEMSubscription = 0;
            decimal? sumOfRTUForSSP = 0;

            foreach (var optionLineItem in lineItemIRPMapDirect[item.GetLineNumber() + "_" + item.Entity.ChargeType])
            {
                string classification = getClassification(optionLineItem);
                string itemType = getItemType(optionLineItem);
                string licenseUsage = getLicenseUsage(optionLineItem);
                if (itemType == "Hardware" && classification == "Base")
                {
                    if (optionLineItem.Entity.BasePriceOverride != null && optionLineItem.Entity.Quantity != null)
                    {
                        sumOfCareItemsRTUOEM = sumOfCareItemsRTUOEM + pricingHelper.ApplyRounding((optionLineItem.Entity.BasePriceOverride * optionLineItem.Entity.Quantity), 2, RoundingMode.HALF_UP);
                    }
                    else if (optionLineItem.Entity.BasePrice != null && optionLineItem.Entity.Quantity != null)
                    {
                        sumOfCareItemsRTUOEM = sumOfCareItemsRTUOEM + pricingHelper.ApplyRounding((optionLineItem.Entity.BasePrice * optionLineItem.Entity.Quantity), 2, RoundingMode.HALF_UP);
                    }
                }
                else if (((itemType == "Software" || itemType == "Software SI") &&
                         (classification == "Standard SW (STD)" || classification == "High Value SW (HVF)" || classification == "Customer Specific Software (CSS)") &&
                         (licenseUsage == "Commercial Licence" || licenseUsage == "Testbed License" || licenseUsage == "Trial License")) || (itemType == "Service" && classification == "Customisation Services"))
                {

                    if (optionLineItem.Entity.BasePriceOverride != null && optionLineItem.Entity.Quantity != null)
                    {
                        sumOfCareItemsOEMSubscription = sumOfCareItemsOEMSubscription + pricingHelper.ApplyRounding((optionLineItem.Entity.BasePriceOverride * optionLineItem.Entity.Quantity), 2, RoundingMode.HALF_UP);
                    }
                    else if (optionLineItem.Entity.BasePrice != null && optionLineItem.Entity.Quantity != null)
                    {
                        sumOfCareItemsOEMSubscription = sumOfCareItemsOEMSubscription + pricingHelper.ApplyRounding((optionLineItem.Entity.BasePrice * optionLineItem.Entity.Quantity), 2, RoundingMode.HALF_UP);
                    }
                }
                else if ((itemType == "Software" || itemType == "Software SI") && licenseUsage == "Commercial Term License" && (classification == "Standard SW (STD)" || classification == "High Value SW (HVF)"))
                {
                    if (optionLineItem.Entity.BasePriceOverride != null && optionLineItem.Entity.Quantity != null)
                    {
                        sumOfRTUForSSP = sumOfRTUForSSP + pricingHelper.ApplyRounding((optionLineItem.Entity.BasePriceOverride * optionLineItem.Entity.Quantity), 2, RoundingMode.HALF_UP);
                    }
                    else if (optionLineItem.Entity.BasePrice != null && optionLineItem.Entity.Quantity != null)
                    {
                        sumOfRTUForSSP = sumOfRTUForSSP + pricingHelper.ApplyRounding((optionLineItem.Entity.BasePrice * optionLineItem.Entity.Quantity), 2, RoundingMode.HALF_UP);
                    }
                }
            }
            decimal? carePrice = (sumOfCareItemsRTUOEM + sumOfCareItemsOEMSubscription + sumOfRTUForSSP * 3);
            decimal? srsPrice = (sumOfCareItemsOEMSubscription + sumOfRTUForSSP * 3);
            careSRSList.Add(carePrice);
            careSRSList.Add(srsPrice);

            return careSRSList;
        }

        private string getClassification(LineItemModel item)
        {
            string classification = string.Empty;

            if (item.GetLineType() == LineType.ProductService)
            {
                classification = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Classification2__c);
            }
            else
            {
                classification = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Classification2__c);
            }

            return classification;
        }

        private string getLicenseUsage(LineItemModel item)
        {
            string licenseUsage = string.Empty;

            if (item.GetLineType() == LineType.ProductService)
            {
                licenseUsage = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_License_Usage__c);
            }
            else
            {
                licenseUsage = item.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_License_Usage__c);
            }

            return licenseUsage;
        }
    }
}
