using Apttus.Lightsaber.Extensibility.Framework.Library.Extension;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Pricing.Common.Constants;
using Apttus.Lightsaber.Pricing.Common.Entities;
using Apttus.Lightsaber.Pricing.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apttus.Lightsaber.Nokia.Common
{
    public class LineItem
    {
        private readonly LineItemModel lineItemModel;

        public LineItem(LineItemModel lineItemModel)
        {
            this.lineItemModel = lineItemModel;
        }

        public string Id { get { return lineItemModel.Entity.Id; } set { lineItemModel.Entity.Id = value; } }
        public string Name { get { return lineItemModel.Entity.Name; } set { lineItemModel.Entity.Name = value; } }
        public string CurrencyIsoCode { get { return lineItemModel.Entity.CurrencyIsoCode; } set { lineItemModel.Entity.CurrencyIsoCode = value; } }
        public string AdHocGroupId { get { return lineItemModel.Entity.AdHocGroupId; } set { lineItemModel.Entity.AdHocGroupId = value; } }
        public decimal? AdjustedPrice { get { return lineItemModel.Entity.AdjustedPrice; } set { lineItemModel.Entity.AdjustedPrice = value; } }
        public decimal? AdjustmentAmount { get { return lineItemModel.Entity.AdjustmentAmount; } set { lineItemModel.Entity.AdjustmentAmount = value; } }
        public string AdjustmentType { get { return lineItemModel.Entity.AdjustmentType; } set { lineItemModel.Entity.AdjustmentType = value; } }
        public bool? AllocateGroupAdjustment { get { return lineItemModel.Entity.AllocateGroupAdjustment; } set { lineItemModel.Entity.AllocateGroupAdjustment = value; } }
        public string AllowableAction { get { return lineItemModel.Entity.AllowableAction; } set { lineItemModel.Entity.AllowableAction = value; } }
        public bool? AllowManualAdjustment { get { return lineItemModel.Entity.AllowManualAdjustment; } set { lineItemModel.Entity.AllowManualAdjustment = value; } }
        public bool? AllowProration { get { return lineItemModel.Entity.AllowProration; } set { lineItemModel.Entity.AllowProration = value; } }
        public string ApprovalStatus { get { return lineItemModel.Entity.ApprovalStatus; } set { lineItemModel.Entity.ApprovalStatus = value; } }
        public string AssetLineItemId { get { return lineItemModel.Entity.AssetLineItemId; } set { lineItemModel.Entity.AssetLineItemId = value; } }
        public decimal? AssetQuantity { get { return lineItemModel.Entity.AssetQuantity; } set { lineItemModel.Entity.AssetQuantity = value; } }
        public string AttributeValueId { get { return lineItemModel.Entity.AttributeValueId; } set { lineItemModel.Entity.AttributeValueId = value; } }
        public bool? AutoRenew { get { return lineItemModel.Entity.AutoRenew; } set { lineItemModel.Entity.AutoRenew = value; } }
        public int? AutoRenewalTerm { get { return lineItemModel.Entity.AutoRenewalTerm; } set { lineItemModel.Entity.AutoRenewalTerm = value; } }
        public string AutoRenewalType { get { return lineItemModel.Entity.AutoRenewalType; } set { lineItemModel.Entity.AutoRenewalType = value; } }
        public decimal? BaseCost { get { return lineItemModel.Entity.BaseCost; } set { lineItemModel.Entity.BaseCost = value; } }
        public decimal? BaseCostOverride { get { return lineItemModel.Entity.BaseCostOverride; } set { lineItemModel.Entity.BaseCostOverride = value; } }
        public decimal? BaseExtendedCost { get { return lineItemModel.Entity.BaseExtendedCost; } set { lineItemModel.Entity.BaseExtendedCost = value; } }
        public decimal? BaseExtendedPrice { get { return lineItemModel.Entity.BaseExtendedPrice; } set { lineItemModel.Entity.BaseExtendedPrice = value; } }
        public decimal? BasePrice { get { return lineItemModel.Entity.BasePrice; } set { lineItemModel.Entity.BasePrice = value; } }
        public decimal? BasePriceOverride { get { return lineItemModel.Entity.BasePriceOverride; } set { lineItemModel.Entity.BasePriceOverride = value; } }
        public string BasePriceMethod { get { return lineItemModel.Entity.BasePriceMethod; } set { lineItemModel.Entity.BasePriceMethod = value; } }
        public string BillingFrequency { get { return lineItemModel.Entity.BillingFrequency; } set { lineItemModel.Entity.BillingFrequency = value; } }
        public string BillingPreferenceId { get { return lineItemModel.Entity.BillingPreferenceId; } set { lineItemModel.Entity.BillingPreferenceId = value; } }
        public string BillingRule { get { return lineItemModel.Entity.BillingRule; } set { lineItemModel.Entity.BillingRule = value; } }
        public string BillToAccountId { get { return lineItemModel.Entity.BillToAccountId; } set { lineItemModel.Entity.BillToAccountId = value; } }
        public string ChargeType { get { return lineItemModel.Entity.ChargeType; } set { lineItemModel.Entity.ChargeType = value; } }
        public string ClassificationHierarchy { get { return lineItemModel.Entity.ClassificationHierarchy; } set { lineItemModel.Entity.ClassificationHierarchy = value; } }
        public string ClassificationHierarchyInfo { get { return lineItemModel.Entity.ClassificationHierarchyInfo; } set { lineItemModel.Entity.ClassificationHierarchyInfo = value; } }
        public string ClassificationId { get { return lineItemModel.Entity.ClassificationId; } set { lineItemModel.Entity.ClassificationId = value; } }
        public string Comments { get { return lineItemModel.Entity.Comments; } set { lineItemModel.Entity.Comments = value; } }
        public string ConfigurationId { get { return lineItemModel.Entity.ConfigurationId; } set { lineItemModel.Entity.ConfigurationId = value; } }
        public string ContractNumbers { get { return lineItemModel.Entity.ContractNumbers; } set { lineItemModel.Entity.ContractNumbers = value; } }
        public decimal? Cost { get { return lineItemModel.Entity.Cost; } set { lineItemModel.Entity.Cost = value; } }
        public string CouponCode { get { return lineItemModel.Entity.CouponCode; } set { lineItemModel.Entity.CouponCode = value; } }
        public bool? Customizable { get { return lineItemModel.Entity.Customizable; } set { lineItemModel.Entity.Customizable = value; } }
        public decimal? DeltaPrice { get { return lineItemModel.Entity.DeltaPrice; } set { lineItemModel.Entity.DeltaPrice = value; } }
        public decimal? DeltaQuantity { get { return lineItemModel.Entity.DeltaQuantity; } set { lineItemModel.Entity.DeltaQuantity = value; } }
        public DateTime? EndDate { get { return lineItemModel.Entity.EndDate; } set { lineItemModel.Entity.EndDate = value; } }
        public decimal? ExtendedCost { get { return lineItemModel.Entity.ExtendedCost; } set { lineItemModel.Entity.ExtendedCost = value; } }
        public decimal? ExtendedPrice { get { return lineItemModel.Entity.ExtendedPrice; } set { lineItemModel.Entity.ExtendedPrice = value; } }
        public decimal? ExtendedQuantity { get { return lineItemModel.Entity.ExtendedQuantity; } set { lineItemModel.Entity.ExtendedQuantity = value; } }
        public decimal? FlatOptionPrice { get { return lineItemModel.Entity.FlatOptionPrice; } set { lineItemModel.Entity.FlatOptionPrice = value; } }
        public string Frequency { get { return lineItemModel.Entity.Frequency; } set { lineItemModel.Entity.Frequency = value; } }
        public decimal? GroupAdjustmentPercent { get { return lineItemModel.Entity.GroupAdjustmentPercent; } set { lineItemModel.Entity.GroupAdjustmentPercent = value; } }
        public string Guidance { get { return lineItemModel.Entity.Guidance; } set { lineItemModel.Entity.Guidance = value; } }
        public bool? HasAttributes { get { return lineItemModel.Entity.HasAttributes; } set { lineItemModel.Entity.HasAttributes = value; } }
        public bool? HasBaseProduct { get { return lineItemModel.Entity.HasBaseProduct; } set { lineItemModel.Entity.HasBaseProduct = value; } }
        public bool? HasDefaults { get { return lineItemModel.Entity.HasDefaults; } set { lineItemModel.Entity.HasDefaults = value; } }
        public bool HasIncentives { get { return lineItemModel.Entity.HasIncentives; } set { lineItemModel.Entity.HasIncentives = value; } }
        public bool? HasOptions { get { return lineItemModel.Entity.HasOptions; } set { lineItemModel.Entity.HasOptions = value; } }
        public bool? HasTieredPrice { get { return lineItemModel.Entity.HasTieredPrice; } set { lineItemModel.Entity.HasTieredPrice = value; } }
        public string IncentiveId { get { return lineItemModel.Entity.IncentiveId; } set { lineItemModel.Entity.IncentiveId = value; } }
        public decimal? IncentiveAdjustmentAmount { get { return lineItemModel.Entity.IncentiveAdjustmentAmount; } set { lineItemModel.Entity.IncentiveAdjustmentAmount = value; } }
        public decimal? IncentiveBasePrice { get { return lineItemModel.Entity.IncentiveBasePrice; } set { lineItemModel.Entity.IncentiveBasePrice = value; } }
        public string IncentiveCode { get { return lineItemModel.Entity.IncentiveCode; } set { lineItemModel.Entity.IncentiveCode = value; } }
        public decimal? IncentiveExtendedPrice { get { return lineItemModel.Entity.IncentiveExtendedPrice; } set { lineItemModel.Entity.IncentiveExtendedPrice = value; } }
        public string IncentiveType { get { return lineItemModel.Entity.IncentiveType; } set { lineItemModel.Entity.IncentiveType = value; } }
        public bool? IsAssetPricing { get { return lineItemModel.Entity.IsAssetPricing; } set { lineItemModel.Entity.IsAssetPricing = value; } }
        public bool? IsCustomPricing { get { return lineItemModel.Entity.IsCustomPricing; } set { lineItemModel.Entity.IsCustomPricing = value; } }
        public bool? IsOptional { get { return lineItemModel.Entity.IsOptional; } set { lineItemModel.Entity.IsOptional = value; } }
        public bool? IsOptionRollupLine { get { return lineItemModel.Entity.IsOptionRollupLine; } set { lineItemModel.Entity.IsOptionRollupLine = value; } }
        public bool? IsPrimaryLine { get { return lineItemModel.Entity.IsPrimaryLine; } set { lineItemModel.Entity.IsPrimaryLine = value; } }
        public bool? IsPrimaryRampLine { get { return lineItemModel.Entity.IsPrimaryRampLine; } set { lineItemModel.Entity.IsPrimaryRampLine = value; } }
        public bool? IsQuantityModifiable { get { return lineItemModel.Entity.IsQuantityModifiable; } set { lineItemModel.Entity.IsQuantityModifiable = value; } }
        public bool? IsSellingTermReadOnly { get { return lineItemModel.Entity.IsSellingTermReadOnly; } set { lineItemModel.Entity.IsSellingTermReadOnly = value; } }
        public bool? IsUsageTierModifiable { get { return lineItemModel.Entity.IsUsageTierModifiable; } set { lineItemModel.Entity.IsUsageTierModifiable = value; } }
        public int ItemSequence { get { return lineItemModel.Entity.ItemSequence; } set { lineItemModel.Entity.ItemSequence = value; } }
        public int LineNumber { get { return lineItemModel.Entity.LineNumber; } set { lineItemModel.Entity.LineNumber = value; } }
        public int? LineSequence { get { return lineItemModel.Entity.LineSequence; } set { lineItemModel.Entity.LineSequence = value; } }
        public string LineStatus { get { return lineItemModel.Entity.LineStatus; } set { lineItemModel.Entity.LineStatus = value; } }
        public string LineType { get { return lineItemModel.Entity.LineType; } set { lineItemModel.Entity.LineType = value; } }
        public decimal? ListPrice { get { return lineItemModel.Entity.ListPrice; } set { lineItemModel.Entity.ListPrice = value; } }
        public decimal? MaxPrice { get { return lineItemModel.Entity.MaxPrice; } set { lineItemModel.Entity.MaxPrice = value; } }
        public decimal? MaxUsageQuantity { get { return lineItemModel.Entity.MaxUsageQuantity; } set { lineItemModel.Entity.MaxUsageQuantity = value; } }
        public string MinMaxPriceAppliesTo { get { return lineItemModel.Entity.MinMaxPriceAppliesTo; } set { lineItemModel.Entity.MinMaxPriceAppliesTo = value; } }
        public decimal? MinPrice { get { return lineItemModel.Entity.MinPrice; } set { lineItemModel.Entity.MinPrice = value; } }
        public decimal? MinUsageQuantity { get { return lineItemModel.Entity.MinUsageQuantity; } set { lineItemModel.Entity.MinUsageQuantity = value; } }
        public decimal? NetAdjustmentPercent { get { return lineItemModel.Entity.NetAdjustmentPercent; } set { lineItemModel.Entity.NetAdjustmentPercent = value; } }
        public decimal? NetPrice { get { return lineItemModel.Entity.NetPrice; } set { lineItemModel.Entity.NetPrice = value; } }
        public decimal? NetUnitPrice { get { return lineItemModel.Entity.NetUnitPrice; } set { lineItemModel.Entity.NetUnitPrice = value; } }
        public string OptionId { get { return lineItemModel.Entity.OptionId; } set { lineItemModel.Entity.OptionId = value; } }
        public decimal? OptionCost { get { return lineItemModel.Entity.OptionCost; } set { lineItemModel.Entity.OptionCost = value; } }
        public decimal? OptionPrice { get { return lineItemModel.Entity.OptionPrice; } set { lineItemModel.Entity.OptionPrice = value; } }
        public int? OptionSequence { get { return lineItemModel.Entity.OptionSequence; } set { lineItemModel.Entity.OptionSequence = value; } }
        public int? ParentBundleNumber { get { return lineItemModel.Entity.ParentBundleNumber; } set { lineItemModel.Entity.ParentBundleNumber = value; } }
        public string PaymentTermId { get { return lineItemModel.Entity.PaymentTermId; } set { lineItemModel.Entity.PaymentTermId = value; } }
        public decimal? PriceAdjustment { get { return lineItemModel.Entity.PriceAdjustment; } set { lineItemModel.Entity.PriceAdjustment = value; } }
        public decimal? PriceAdjustmentAmount { get { return lineItemModel.Entity.PriceAdjustmentAmount; } set { lineItemModel.Entity.PriceAdjustmentAmount = value; } }
        public string PriceAdjustmentAppliesTo { get { return lineItemModel.Entity.PriceAdjustmentAppliesTo; } set { lineItemModel.Entity.PriceAdjustmentAppliesTo = value; } }
        public string PriceAdjustmentType { get { return lineItemModel.Entity.PriceAdjustmentType; } set { lineItemModel.Entity.PriceAdjustmentType = value; } }
        public string PriceGroup { get { return lineItemModel.Entity.PriceGroup; } set { lineItemModel.Entity.PriceGroup = value; } }
        public bool? PriceIncludedInBundle { get { return lineItemModel.Entity.PriceIncludedInBundle; } set { lineItemModel.Entity.PriceIncludedInBundle = value; } }
        public string PriceListId { get { return lineItemModel.Entity.PriceListId; } set { lineItemModel.Entity.PriceListId = value; } }
        public string PriceListItemId { get { return lineItemModel.Entity.PriceListItemId; } set { lineItemModel.Entity.PriceListItemId = value; } }
        public string PriceMethod { get { return lineItemModel.Entity.PriceMethod; } set { lineItemModel.Entity.PriceMethod = value; } }
        public string PriceType { get { return lineItemModel.Entity.PriceType; } set { lineItemModel.Entity.PriceType = value; } }
        public string PriceUom { get { return lineItemModel.Entity.PriceUom; } set { lineItemModel.Entity.PriceUom = value; } }
        public DateTime? PricingDate { get { return lineItemModel.Entity.PricingDate; } set { lineItemModel.Entity.PricingDate = value; } }
        public string PricingGuidance { get { return lineItemModel.Entity.PricingGuidance; } set { lineItemModel.Entity.PricingGuidance = value; } }
        public string PricingStatus { get { return lineItemModel.Entity.PricingStatus; } set { lineItemModel.Entity.PricingStatus = value; } }
        public string PricingSteps { get { return lineItemModel.Entity.PricingSteps; } set { lineItemModel.Entity.PricingSteps = value; } }
        public string LocationId { get { return lineItemModel.Entity.LocationId; } set { lineItemModel.Entity.LocationId = value; } }
        public int PrimaryLineNumber { get { return lineItemModel.Entity.PrimaryLineNumber; } set { lineItemModel.Entity.PrimaryLineNumber = value; } }
        public string ProductId { get { return lineItemModel.Entity.ProductId; } set { lineItemModel.Entity.ProductId = value; } }
        public string ProductOptionId { get { return lineItemModel.Entity.ProductOptionId; } set { lineItemModel.Entity.ProductOptionId = value; } }
        public decimal? RelatedAdjustmentAmount { get { return lineItemModel.Entity.RelatedAdjustmentAmount; } set { lineItemModel.Entity.RelatedAdjustmentAmount = value; } }
        public string RelatedAdjustmentAppliesTo { get { return lineItemModel.Entity.RelatedAdjustmentAppliesTo; } set { lineItemModel.Entity.RelatedAdjustmentAppliesTo = value; } }
        public string RelatedAdjustmentType { get { return lineItemModel.Entity.RelatedAdjustmentType; } set { lineItemModel.Entity.RelatedAdjustmentType = value; } }
        public string RelatedItemId { get { return lineItemModel.Entity.RelatedItemId; } set { lineItemModel.Entity.RelatedItemId = value; } }
        public decimal? RelatedPercent { get { return lineItemModel.Entity.RelatedPercent; } set { lineItemModel.Entity.RelatedPercent = value; } }
        public string RelatedPercentAppliesTo { get { return lineItemModel.Entity.RelatedPercentAppliesTo; } set { lineItemModel.Entity.RelatedPercentAppliesTo = value; } }
        public decimal? Quantity { get { return lineItemModel.Entity.Quantity; } set { lineItemModel.Entity.Quantity = value; } }
        public decimal? RenewalAdjustmentAmount { get { return lineItemModel.Entity.RenewalAdjustmentAmount; } set { lineItemModel.Entity.RenewalAdjustmentAmount = value; } }
        public string RenewalAdjustmentType { get { return lineItemModel.Entity.RenewalAdjustmentType; } set { lineItemModel.Entity.RenewalAdjustmentType = value; } }
        public string RollupPriceMethod { get { return lineItemModel.Entity.RollupPriceMethod; } set { lineItemModel.Entity.RollupPriceMethod = value; } }
        public bool? RollupPriceToBundle { get { return lineItemModel.Entity.RollupPriceToBundle; } set { lineItemModel.Entity.RollupPriceToBundle = value; } }
        public string ShipToAccountId { get { return lineItemModel.Entity.ShipToAccountId; } set { lineItemModel.Entity.ShipToAccountId = value; } }
        public DateTime? StartDate { get { return lineItemModel.Entity.StartDate; } set { lineItemModel.Entity.StartDate = value; } }
        public string SellingFrequency { get { return lineItemModel.Entity.SellingFrequency; } set { lineItemModel.Entity.SellingFrequency = value; } }
        public decimal? SellingTerm { get { return lineItemModel.Entity.SellingTerm; } set { lineItemModel.Entity.SellingTerm = value; } }
        public string SellingUom { get { return lineItemModel.Entity.SellingUom; } set { lineItemModel.Entity.SellingUom = value; } }
        public string SummaryGroupId { get { return lineItemModel.Entity.SummaryGroupId; } set { lineItemModel.Entity.SummaryGroupId = value; } }
        public bool? Taxable { get { return lineItemModel.Entity.Taxable; } set { lineItemModel.Entity.Taxable = value; } }
        public string TaxCodeId { get { return lineItemModel.Entity.TaxCodeId; } set { lineItemModel.Entity.TaxCodeId = value; } }
        public bool? TaxInclusive { get { return lineItemModel.Entity.TaxInclusive; } set { lineItemModel.Entity.TaxInclusive = value; } }
        public decimal? Term { get { return lineItemModel.Entity.Term; } set { lineItemModel.Entity.Term = value; } }
        public string TransferPriceLineItemId { get { return lineItemModel.Entity.TransferPriceLineItemId; } set { lineItemModel.Entity.TransferPriceLineItemId = value; } }
        public decimal? TotalQuantity { get { return lineItemModel.Entity.TotalQuantity; } set { lineItemModel.Entity.TotalQuantity = value; } }
        public decimal? UnitCostAdjustment { get { return lineItemModel.Entity.UnitCostAdjustment; } set { lineItemModel.Entity.UnitCostAdjustment = value; } }
        public decimal? UnitPriceAdjustmentAuto { get { return lineItemModel.Entity.UnitPriceAdjustmentAuto; } set { lineItemModel.Entity.UnitPriceAdjustmentAuto = value; } }
        public decimal? UnitPriceAdjustmentManual { get { return lineItemModel.Entity.UnitPriceAdjustmentManual; } set { lineItemModel.Entity.UnitPriceAdjustmentManual = value; } }
        public string Uom { get { return lineItemModel.Entity.Uom; } set { lineItemModel.Entity.Uom = value; } }

        public PriceListItemModel GetPriceListItem()
        {
            return lineItemModel.GetPriceListItem();
        }

        public decimal GetQuantity()
        {
            return lineItemModel.GetQuantity();
        }

        public bool IsOptionLine()
        {
            return lineItemModel.IsOptionLine();
        }

        public ProductLineItemModel GetRootParentLineItem()
        {
            return lineItemModel.GetRootParentLineItem();
        }

        public string GetProductOrOptionId()
        {
            return lineItemModel.GetProductOrOptionId();
        }

        public int GetLineNumber()
        {
            return lineItemModel.GetLineNumber();
        }

        public decimal GetValuetOrDefault(string fieldName, decimal defaultValue)
        {
            return lineItemModel.GetValuetOrDefault(fieldName, defaultValue);
        }

        public LineType GetLineType()
        {
            return lineItemModel.GetLineType();
        }

        public decimal GetExtendedQuantity()
        {
            return lineItemModel.GetExtendedQuantity();
        }
        public LineItem GetRootParentPrimaryChargeTypeLineItem()
        {
            return new LineItem(lineItemModel.GetRootParentPrimaryChargeTypeLineItem());
        }
        public void UpdatePrice(IPricingHelper pricingHelper)
        {
            pricingHelper.UpdatePrice(lineItemModel);
        }
        #region Custom & Not available Standard LineItem Fields

        public string Apttus_Config2__ConfigStatus__c { get { return lineItemModel.Get<string>(LineItemStandardField.Apttus_Config2__ConfigStatus__c); } set { lineItemModel.Set(LineItemStandardField.Apttus_Config2__ConfigStatus__c, value); } }
        public bool? Apttus_Config2__IsReadOnly__c { get { return lineItemModel.Get<bool?>(LineItemStandardField.Apttus_Config2__IsReadOnly__c); } set { lineItemModel.Set(LineItemStandardField.Apttus_Config2__IsReadOnly__c, value); } }
        public bool? Apttus_Config2__IsHidden__c { get { return lineItemModel.Get<bool?>(LineItemStandardField.Apttus_Config2__IsHidden__c); } set { lineItemModel.Set(LineItemStandardField.Apttus_Config2__IsHidden__c, value); } }
        public string Custom_Product_Code__c { get { return lineItemModel.Get<string>(LineItemCustomField.Custom_Product_Code__c); } set { lineItemModel.Set(LineItemCustomField.Custom_Product_Code__c, value); } }
        public string CustomProductValue__c { get { return lineItemModel.Get<string>(LineItemCustomField.CustomProductValue__c); } set { lineItemModel.Set(LineItemCustomField.CustomProductValue__c, value); } }
        public bool? NokiaCPQ_Is_SI__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.NokiaCPQ_Is_SI__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Is_SI__c, value); } }
        public string Source__c { get { return lineItemModel.Get<string>(LineItemCustomField.Source__c); } set { lineItemModel.Set(LineItemCustomField.Source__c, value); } }
        public decimal? Total_ONT_Quantity__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity__c); } set { lineItemModel.Set(LineItemCustomField.Total_ONT_Quantity__c, value); } }
        public decimal? Total_ONT_Quantity_FBA__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity_FBA__c); } set { lineItemModel.Set(LineItemCustomField.Total_ONT_Quantity_FBA__c, value); } }
        public decimal? Total_ONT_Quantity_P2P__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Total_ONT_Quantity_P2P__c); } set { lineItemModel.Set(LineItemCustomField.Total_ONT_Quantity_P2P__c, value); } }
        public decimal? NCPQ_Unitary_CLP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NCPQ_Unitary_CLP__c); } set { lineItemModel.Set(LineItemCustomField.NCPQ_Unitary_CLP__c, value); } }
        public bool? NokiaCPQ_Spare__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.NokiaCPQ_Spare__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Spare__c, value); } }
        public string NokiaCPQ_Product_Type__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQ_Product_Type__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Product_Type__c, value); } }
        public decimal? NokiaCPQ_Extended_CLP_2__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CLP_2__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_CLP_2__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr1_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Extended_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_Price__c, value); } }
        public decimal? Nokia_SRS_Base_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_SRS_Base_Extended_Price__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_SRS_Base_Extended_Price__c, value); } }
        public decimal? Nokia_SSP_Base_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_SSP_Base_Extended_Price__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_SSP_Base_Extended_Price__c, value); } }
        public decimal? NokiaCPQ_Unitary_IRP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_IRP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Unitary_IRP__c, value); } }
        public decimal? NokiaCPQ_Extended_CNP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CNP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_CNP__c, value); } }
        public decimal? NokiaCPQ_AdvancePricing_CUP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_AdvancePricing_CUP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_AdvancePricing_CUP__c, value); } }
        //GP: This field is not available on LineItem
        public decimal? NokiaCPQAdv_Net_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQAdv_Net_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQAdv_Net_Price__c, value); } }
        public decimal? NokiaCPQ_ExtendedPrice_CNP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_ExtendedPrice_CNP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_ExtendedPrice_CNP__c, value); } }
        public decimal? NokiaCPQ_ExtendedAdvance_NP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_ExtendedAdvance_NP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_ExtendedAdvance_NP__c, value); } }
        public bool? Advanced_pricing_done__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.Advanced_pricing_done__c); } set { lineItemModel.Set(LineItemCustomField.Advanced_pricing_done__c, value); } }
        public decimal? NokiaCPQ_AdvancePricing_NP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_AdvancePricing_NP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_AdvancePricing_NP__c, value); } }
        public decimal? NokiaCPQ_Extended_CUP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CUP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_CUP__c, value); } }
        public decimal? NokiaCPQ_Unitary_Cost__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost__c, value); } }
        public decimal? NokiaCPQ_Extended_CLP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CLP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_CLP__c, value); } }
        public decimal? NokiaCPQ_Extended_IRP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_IRP__c, value); } }
        public decimal? NokiaCPQ_ExtendedPrice_CUP__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_ExtendedPrice_CUP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_ExtendedPrice_CUP__c, value); } }
        public bool? NokiaCPQ_Is_CLP__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.NokiaCPQ_Is_CLP__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Is_CLP__c, value); } }
        public bool? OEM__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.OEM__c); } set { lineItemModel.Set(LineItemCustomField.OEM__c, value); } }
        public string NokiaCPQ_Light_Color__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQ_Light_Color__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Light_Color__c, value); } }
        public decimal? NokiaCPQ_Maximum_IRP_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maximum_IRP_Discount__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maximum_IRP_Discount__c, value); } }
        public decimal? NokiaCPQ_IRP_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_IRP_Discount__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_IRP_Discount__c, value); } }
        public string NokiaCPQ_Account_Region__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQ_Account_Region__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Account_Region__c, value); } }
        public string NokiaCPQ_Org__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQ_Org__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Org__c, value); } }
        public string NokiaCPQ_BU__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQ_BU__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_BU__c, value); } }
        public string NokiaCPQ_BG__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQ_BG__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_BG__c, value); } }
        public decimal? NokiaCPQ_Floor_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Floor_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Floor_Price__c, value); } }
        public decimal? NokiaCPQ_Market_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Market_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Market_Price__c, value); } }
        public bool? NokiaCPQ_Custom_Bid__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.NokiaCPQ_Custom_Bid__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Custom_Bid__c, value); } }
        public string Product_Extension__c { get { return lineItemModel.Get<string>(LineItemCustomField.Product_Extension__c); } set { lineItemModel.Set(LineItemCustomField.Product_Extension__c, value); } }
        public bool? Is_List_Price_Only__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.Is_List_Price_Only__c); } set { lineItemModel.Set(LineItemCustomField.Is_List_Price_Only__c, value); } }
        public bool? NokiaCPQ_Is_Direct_Option__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.NokiaCPQ_Is_Direct_Option__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Is_Direct_Option__c, value); } }
        public decimal? Nokia_Maint_Y1_Per__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y1_Per__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_Maint_Y1_Per__c, value); } }
        public decimal? Nokia_Maint_Y2_Per__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y2_Per__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_Maint_Y2_Per__c, value); } }
        public decimal? NokiaCPQ_SSP_Rate__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_SSP_Rate__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_SSP_Rate__c, value); } }
        public decimal? NokiaCPQ_SRS_Rate__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_SRS_Rate__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_SRS_Rate__c, value); } }
        public decimal? NokiaCPQ_Unitary_Cost_Initial__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Unitary_Cost_Initial__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Unitary_Cost_Initial__c, value); } }
        public decimal? Total_Option_Quantity__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Total_Option_Quantity__c); } set { lineItemModel.Set(LineItemCustomField.Total_Option_Quantity__c, value); } }
        public decimal? NokiaCPQ_IncotermNew__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_IncotermNew__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_IncotermNew__c, value); } }
        public bool? Is_Contract_Pricing_2__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.Is_Contract_Pricing_2__c); } set { lineItemModel.Set(LineItemCustomField.Is_Contract_Pricing_2__c, value); } }
        public string NokiaCPQAccreditationType__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQAccreditationType__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQAccreditationType__c, value); } }
        public string Nokia_Pricing_Cluster__c { get { return lineItemModel.Get<string>(LineItemCustomField.Nokia_Pricing_Cluster__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_Pricing_Cluster__c, value); } }
        public string Nokia_Maintenance_Level__c { get { return lineItemModel.Get<string>(LineItemCustomField.Nokia_Maintenance_Level__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_Maintenance_Level__c, value); } }
        public string Nokia_Maint_Pricing_Cluster__c { get { return lineItemModel.Get<string>(LineItemCustomField.Nokia_Maint_Pricing_Cluster__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_Maint_Pricing_Cluster__c, value); } }
        public decimal? NokiaCPQ_Attachment_Per__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Attachment_Per__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Attachment_Per__c, value); } }
        public decimal? NokiaCPQ_Renewal_Per__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Renewal_Per__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Renewal_Per__c, value); } }
        public decimal? NokiaCPQ_Performance_Per__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Performance_Per__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Performance_Per__c, value); } }
        public decimal? NokiaCPQ_Multi_Yr_Per__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Multi_Yr_Per__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Multi_Yr_Per__c, value); } }
        public decimal? NokiaCPQ_Maint_Accreditation_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Accreditation_Discount__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Accreditation_Discount__c, value); } }
        public decimal? NokiaCPQ_Total_Maintenance_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Total_Maintenance_Discount__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Total_Maintenance_Discount__c, value); } }
        public decimal? NokiaCPQ_Accreditation_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Accreditation_Discount__c, value); } }
        public bool? NokiaCPQ_Static_Bundle_Option__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.NokiaCPQ_Static_Bundle_Option__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Static_Bundle_Option__c, value); } }
        public decimal? Nokia_CPQ_Maint_Prod_Cat_Disc__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_CPQ_Maint_Prod_Cat_Disc__c, value); } }
        public bool? Is_Custom_Product__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.Is_Custom_Product__c); } set { lineItemModel.Set(LineItemCustomField.Is_Custom_Product__c, value); } }
        public decimal? Nokia_SSP_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_SSP_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_SSP_List_Price__c, value); } }
        public decimal? Nokia_SSP_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_SSP_Base_Price__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_SSP_Base_Price__c, value); } }
        public decimal? Nokia_SRS_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_SRS_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_SRS_List_Price__c, value); } }
        public decimal? Nokia_SRS_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_SRS_Base_Price__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_SRS_Base_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Y1_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Y1_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Y1_List_Price__c, value); } }
        public decimal? Nokia_Maint_Y1_Extended_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Nokia_Maint_Y1_Extended_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.Nokia_Maint_Y1_Extended_List_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_List_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_Extended_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_List_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Extended_List_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr1_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr1_Base_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Yr1_Base_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Maint_Yr2_Base_Price__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Maint_Yr2_Base_Price__c, value); } }
        public string NokiaCPQ_Category__c { get { return lineItemModel.Get<string>(LineItemCustomField.NokiaCPQ_Category__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Category__c, value); } }
        public decimal? Product_Number_Of_Ports__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Product_Number_Of_Ports__c); } set { lineItemModel.Set(LineItemCustomField.Product_Number_Of_Ports__c, value); } }
        public bool? Is_FBA__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.Is_FBA__c); } set { lineItemModel.Set(LineItemCustomField.Is_FBA__c, value); } }
        public bool? Is_P2P__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.Is_P2P__c); } set { lineItemModel.Set(LineItemCustomField.Is_P2P__c, value); } }
        public decimal? NokiaCPQ_SRSBasePrice__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_SRSBasePrice__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_SRSBasePrice__c, value); } }
        public decimal? NokiaCPQ_CareSRSBasePrice__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_CareSRSBasePrice__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_CareSRSBasePrice__c, value); } }
        public decimal? Reference_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Reference_Price__c); } set { lineItemModel.Set(LineItemCustomField.Reference_Price__c, value); } }
        public decimal? Equivalent_Price__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Equivalent_Price__c); } set { lineItemModel.Set(LineItemCustomField.Equivalent_Price__c, value); } }
        public decimal? NokiaCPQ_Extended_CUP_2__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_CUP_2__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_CUP_2__c, value); } }
        public decimal? NokiaCPQ_Extended_IRP2__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_IRP2__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_IRP2__c, value); } }
        public decimal? NokiaCPQ_Extended_Cost__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.NokiaCPQ_Extended_Cost__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_Extended_Cost__c, value); } }
        public decimal? Sales_Margin__c { get { return lineItemModel.Get<decimal?>(LineItemCustomField.Sales_Margin__c); } set { lineItemModel.Set(LineItemCustomField.Sales_Margin__c, value); } }
        public string Item_Type_From_CAT__c { get { return lineItemModel.Get<string>(LineItemCustomField.Item_Type_From_CAT__c); } set { lineItemModel.Set(LineItemCustomField.Item_Type_From_CAT__c, value); } }
        public bool? NokiaCPQ_IsArcadiaBundle__c { get { return lineItemModel.Get<bool?>(LineItemCustomField.NokiaCPQ_IsArcadiaBundle__c); } set { lineItemModel.Set(LineItemCustomField.NokiaCPQ_IsArcadiaBundle__c, value); } }

        #endregion

        #region Relationship LineItem Fields

        public string Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c); } }
        public string Apttus_Config2__ProductId__r_Business_Group__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Business_Group__c); } }
        public string Apttus_Config2__ProductId__r_NokiaCPQ_Product_Discount_Category__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Product_Discount_Category__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_Product_Discount_Category__c { get { return lineItemModel.GetLookupValue<string?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Product_Discount_Category__c); } }
        public string Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c); } }
        public string Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c); } }
        public string Apttus_Config2__ProductId__r_NokiaCPQ_Classification2__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Classification2__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_Classification2__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Classification2__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_Item_Type__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Item_Type__c); } }
        public string Apttus_Config2__ProductId__r_NokiaCPQ_License_Usage__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_License_Usage__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_License_Usage__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_License_Usage__c); } }
        public string Apttus_Config2__ProductId__r_Id { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Id); } }
        public string Apttus_Config2__OptionId__r_Id { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Id); } }
        public string Apttus_Config2__ProductId__r_ProductCode { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_ProductCode); } }
        public string Apttus_Config2__OptionId__r_ProductCode { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_ProductCode); } }
        public string Apttus_Config2__ProductId__r_Family { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Family); } }
        public bool? Apttus_Config2__ProductId__r_IsSSP__c { get { return lineItemModel.GetLookupValue<bool?>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_IsSSP__c); } }
        public string Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Apttus_Config2__PriceListId__c); } }
        public string Apttus_Config2__ProductId__r_Is_Dummy_Bundle_CPQ__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Is_Dummy_Bundle_CPQ__c); } }
        public string Apttus_Config2__OptionId__r_Is_Dummy_Bundle_CPQ__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Is_Dummy_Bundle_CPQ__c); } }
        public string Apttus_Config2__ProductId__r_Portfolio__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Portfolio__c); } }
        public decimal? Apttus_Config2__OptionId__r_Number_of_GE_Ports__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_Number_of_GE_Ports__c); } }
        public decimal? Apttus_Config2__ProductId__r_Number_of_GE_Ports__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_Number_of_GE_Ports__c); } }
        public decimal? Apttus_Config2__PriceListItemId__r_Partner_Price__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Partner_Price__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_North_America__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_North_America__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Latin_America__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Latin_America__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQMin_SM_Middle_East_and_Africa__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQMin_SM_Middle_East_and_Africa__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_India__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_India__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Asia_Pacific_Japan__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Asia_Pacific_Japan__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Greater_China__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Greater_China__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Europe__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Europe__c); } }
        public decimal? Apttus_Config2__ProductId__r_NokiaCPQ_Maximum_IRP_Discount__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_Maximum_IRP_Discount__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Maximum_IRP_Discount__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_Maximum_IRP_Discount__c); } }
        public bool? Apttus_Config2__ProductId__r_NokiaCPQ_OEM__c { get { return lineItemModel.GetLookupValue<bool?>(LineItemStandardRelationshipField.Apttus_Config2__ProductId__r_NokiaCPQ_OEM__c); } }
        public bool? Apttus_Config2__OptionId__r_NokiaCPQ_OEM__c { get { return lineItemModel.GetLookupValue<bool?>(LineItemStandardRelationshipField.Apttus_Config2__OptionId__r_NokiaCPQ_OEM__c); } }
        public string Apttus_Config2__PriceListItemId__r_Contracted__c { get { return lineItemModel.GetLookupValue<string>(LineItemStandardRelationshipField.Apttus_Config2__PriceListItemId__r_Contracted__c); } }
        #endregion
    }
}