using Apttus.Lightsaber.Extensibility.Library.Extension;
using Apttus.Lightsaber.Extensibility.Library.Interface;
using Apttus.Lightsaber.Pricing.Common.Callback.Enums;
using Apttus.Lightsaber.Pricing.Common.Callback.Models;
using System;

namespace Apttus.Lightsaber.Nokia.Common
{
    public class LineItem
    {
        private readonly ILineItemModel lineItemModel;

        public LineItem(ILineItemModel lineItemModel)
        {
            this.lineItemModel = lineItemModel;
        }

        public string Id { get { return lineItemModel.GetEntity().Id; } set { lineItemModel.GetEntity().Id = value; } }
        public string Name { get { return lineItemModel.GetEntity().Name; } set { lineItemModel.GetEntity().Name = value; } }
        public string CurrencyIsoCode { get { return lineItemModel.GetEntity().CurrencyIsoCode; } set { lineItemModel.GetEntity().CurrencyIsoCode = value; } }
        public string AdHocGroupId { get { return lineItemModel.GetEntity().AdHocGroupId; } set { lineItemModel.GetEntity().AdHocGroupId = value; } }
        public decimal? AdjustedPrice { get { return lineItemModel.GetEntity().AdjustedPrice; } set { lineItemModel.GetEntity().AdjustedPrice = value; } }
        public decimal? AdjustmentAmount { get { return lineItemModel.GetEntity().AdjustmentAmount; } set { lineItemModel.GetEntity().AdjustmentAmount = value; } }
        public string AdjustmentType { get { return lineItemModel.GetEntity().AdjustmentType; } set { lineItemModel.GetEntity().AdjustmentType = value; } }
        public bool? AllocateGroupAdjustment { get { return lineItemModel.GetEntity().AllocateGroupAdjustment; } set { lineItemModel.GetEntity().AllocateGroupAdjustment = value; } }
        public string AllowableAction { get { return lineItemModel.GetEntity().AllowableAction; } set { lineItemModel.GetEntity().AllowableAction = value; } }
        public bool? AllowManualAdjustment { get { return lineItemModel.GetEntity().AllowManualAdjustment; } set { lineItemModel.GetEntity().AllowManualAdjustment = value; } }
        public bool? AllowProration { get { return lineItemModel.GetEntity().AllowProration; } set { lineItemModel.GetEntity().AllowProration = value; } }
        public string ApprovalStatus { get { return lineItemModel.GetEntity().ApprovalStatus; } set { lineItemModel.GetEntity().ApprovalStatus = value; } }
        public string AssetLineItemId { get { return lineItemModel.GetEntity().AssetLineItemId; } set { lineItemModel.GetEntity().AssetLineItemId = value; } }
        public decimal? AssetQuantity { get { return lineItemModel.GetEntity().AssetQuantity; } set { lineItemModel.GetEntity().AssetQuantity = value; } }
        public string AttributeValueId { get { return lineItemModel.GetEntity().AttributeValueId; } set { lineItemModel.GetEntity().AttributeValueId = value; } }
        public bool? AutoRenew { get { return lineItemModel.GetEntity().AutoRenew; } set { lineItemModel.GetEntity().AutoRenew = value; } }
        public int? AutoRenewalTerm { get { return lineItemModel.GetEntity().AutoRenewalTerm; } set { lineItemModel.GetEntity().AutoRenewalTerm = value; } }
        public string AutoRenewalType { get { return lineItemModel.GetEntity().AutoRenewalType; } set { lineItemModel.GetEntity().AutoRenewalType = value; } }
        public decimal? BaseCost { get { return lineItemModel.GetEntity().BaseCost; } set { lineItemModel.GetEntity().BaseCost = value; } }
        public decimal? BaseCostOverride { get { return lineItemModel.GetEntity().BaseCostOverride; } set { lineItemModel.GetEntity().BaseCostOverride = value; } }
        public decimal? BaseExtendedCost { get { return lineItemModel.GetEntity().BaseExtendedCost; } set { lineItemModel.GetEntity().BaseExtendedCost = value; } }
        public decimal? BaseExtendedPrice { get { return lineItemModel.GetEntity().BaseExtendedPrice; } set { lineItemModel.GetEntity().BaseExtendedPrice = value; } }
        public decimal? BasePrice { get { return lineItemModel.GetEntity().BasePrice; } set { lineItemModel.GetEntity().BasePrice = value; } }
        public decimal? BasePriceOverride { get { return lineItemModel.GetEntity().BasePriceOverride; } set { lineItemModel.GetEntity().BasePriceOverride = value; } }
        public string BasePriceMethod { get { return lineItemModel.GetEntity().BasePriceMethod; } set { lineItemModel.GetEntity().BasePriceMethod = value; } }
        public string BillingFrequency { get { return lineItemModel.GetEntity().BillingFrequency; } set { lineItemModel.GetEntity().BillingFrequency = value; } }
        public string BillingPreferenceId { get { return lineItemModel.GetEntity().BillingPreferenceId; } set { lineItemModel.GetEntity().BillingPreferenceId = value; } }
        public string BillingRule { get { return lineItemModel.GetEntity().BillingRule; } set { lineItemModel.GetEntity().BillingRule = value; } }
        public string BillToAccountId { get { return lineItemModel.GetEntity().BillToAccountId; } set { lineItemModel.GetEntity().BillToAccountId = value; } }
        public string ChargeType { get { return lineItemModel.GetEntity().ChargeType; } set { lineItemModel.GetEntity().ChargeType = value; } }
        public string ClassificationHierarchy { get { return lineItemModel.GetEntity().ClassificationHierarchy; } set { lineItemModel.GetEntity().ClassificationHierarchy = value; } }
        public string ClassificationHierarchyInfo { get { return lineItemModel.GetEntity().ClassificationHierarchyInfo; } set { lineItemModel.GetEntity().ClassificationHierarchyInfo = value; } }
        public string ClassificationId { get { return lineItemModel.GetEntity().ClassificationId; } set { lineItemModel.GetEntity().ClassificationId = value; } }
        public string Comments { get { return lineItemModel.GetEntity().Comments; } set { lineItemModel.GetEntity().Comments = value; } }
        public string ConfigurationId { get { return lineItemModel.GetEntity().ConfigurationId; } set { lineItemModel.GetEntity().ConfigurationId = value; } }
        public string ContractNumbers { get { return lineItemModel.GetEntity().ContractNumbers; } set { lineItemModel.GetEntity().ContractNumbers = value; } }
        public decimal? Cost { get { return lineItemModel.GetEntity().Cost; } set { lineItemModel.GetEntity().Cost = value; } }
        public string CouponCode { get { return lineItemModel.GetEntity().CouponCode; } set { lineItemModel.GetEntity().CouponCode = value; } }
        public bool? Customizable { get { return lineItemModel.GetEntity().Customizable; } set { lineItemModel.GetEntity().Customizable = value; } }
        public decimal? DeltaPrice { get { return lineItemModel.GetEntity().DeltaPrice; } set { lineItemModel.GetEntity().DeltaPrice = value; } }
        public decimal? DeltaQuantity { get { return lineItemModel.GetEntity().DeltaQuantity; } set { lineItemModel.GetEntity().DeltaQuantity = value; } }
        public DateTime? EndDate { get { return lineItemModel.GetEntity().EndDate; } set { lineItemModel.GetEntity().EndDate = value; } }
        public decimal? ExtendedCost { get { return lineItemModel.GetEntity().ExtendedCost; } set { lineItemModel.GetEntity().ExtendedCost = value; } }
        public decimal? ExtendedPrice { get { return lineItemModel.GetEntity().ExtendedPrice; } set { lineItemModel.GetEntity().ExtendedPrice = value; } }
        public decimal? ExtendedQuantity { get { return lineItemModel.GetEntity().ExtendedQuantity; } set { lineItemModel.GetEntity().ExtendedQuantity = value; } }
        public decimal? FlatOptionPrice { get { return lineItemModel.GetEntity().FlatOptionPrice; } set { lineItemModel.GetEntity().FlatOptionPrice = value; } }
        public string Frequency { get { return lineItemModel.GetEntity().Frequency; } set { lineItemModel.GetEntity().Frequency = value; } }
        public decimal? GroupAdjustmentPercent { get { return lineItemModel.GetEntity().GroupAdjustmentPercent; } set { lineItemModel.GetEntity().GroupAdjustmentPercent = value; } }
        public string Guidance { get { return lineItemModel.GetEntity().Guidance; } set { lineItemModel.GetEntity().Guidance = value; } }
        public bool? HasAttributes { get { return lineItemModel.GetEntity().HasAttributes; } set { lineItemModel.GetEntity().HasAttributes = value; } }
        public bool? HasBaseProduct { get { return lineItemModel.GetEntity().HasBaseProduct; } set { lineItemModel.GetEntity().HasBaseProduct = value; } }
        public bool? HasDefaults { get { return lineItemModel.GetEntity().HasDefaults; } set { lineItemModel.GetEntity().HasDefaults = value; } }
        public bool HasIncentives { get { return lineItemModel.GetEntity().HasIncentives; } set { lineItemModel.GetEntity().HasIncentives = value; } }
        public bool? HasOptions { get { return lineItemModel.GetEntity().HasOptions; } set { lineItemModel.GetEntity().HasOptions = value; } }
        public bool? HasTieredPrice { get { return lineItemModel.GetEntity().HasTieredPrice; } set { lineItemModel.GetEntity().HasTieredPrice = value; } }
        public string IncentiveId { get { return lineItemModel.GetEntity().IncentiveId; } set { lineItemModel.GetEntity().IncentiveId = value; } }
        public decimal? IncentiveAdjustmentAmount { get { return lineItemModel.GetEntity().IncentiveAdjustmentAmount; } set { lineItemModel.GetEntity().IncentiveAdjustmentAmount = value; } }
        public decimal? IncentiveBasePrice { get { return lineItemModel.GetEntity().IncentiveBasePrice; } set { lineItemModel.GetEntity().IncentiveBasePrice = value; } }
        public string IncentiveCode { get { return lineItemModel.GetEntity().IncentiveCode; } set { lineItemModel.GetEntity().IncentiveCode = value; } }
        public decimal? IncentiveExtendedPrice { get { return lineItemModel.GetEntity().IncentiveExtendedPrice; } set { lineItemModel.GetEntity().IncentiveExtendedPrice = value; } }
        public string IncentiveType { get { return lineItemModel.GetEntity().IncentiveType; } set { lineItemModel.GetEntity().IncentiveType = value; } }
        public bool? IsAssetPricing { get { return lineItemModel.GetEntity().IsAssetPricing; } set { lineItemModel.GetEntity().IsAssetPricing = value; } }
        public bool? IsCustomPricing { get { return lineItemModel.GetEntity().IsCustomPricing; } set { lineItemModel.GetEntity().IsCustomPricing = value; } }
        public bool? IsOptional { get { return lineItemModel.GetEntity().IsOptional; } set { lineItemModel.GetEntity().IsOptional = value; } }
        public bool? IsOptionRollupLine { get { return lineItemModel.GetEntity().IsOptionRollupLine; } set { lineItemModel.GetEntity().IsOptionRollupLine = value; } }
        public bool? IsPrimaryLine { get { return lineItemModel.GetEntity().IsPrimaryLine; } set { lineItemModel.GetEntity().IsPrimaryLine = value; } }
        public bool? IsPrimaryRampLine { get { return lineItemModel.GetEntity().IsPrimaryRampLine; } set { lineItemModel.GetEntity().IsPrimaryRampLine = value; } }
        public bool? IsQuantityModifiable { get { return lineItemModel.GetEntity().IsQuantityModifiable; } set { lineItemModel.GetEntity().IsQuantityModifiable = value; } }
        public bool? IsSellingTermReadOnly { get { return lineItemModel.GetEntity().IsSellingTermReadOnly; } set { lineItemModel.GetEntity().IsSellingTermReadOnly = value; } }
        public bool? IsUsageTierModifiable { get { return lineItemModel.GetEntity().IsUsageTierModifiable; } set { lineItemModel.GetEntity().IsUsageTierModifiable = value; } }
        public int ItemSequence { get { return lineItemModel.GetEntity().ItemSequence; } set { lineItemModel.GetEntity().ItemSequence = value; } }
        public int LineNumber { get { return lineItemModel.GetEntity().LineNumber; } set { lineItemModel.GetEntity().LineNumber = value; } }
        public int? LineSequence { get { return lineItemModel.GetEntity().LineSequence; } set { lineItemModel.GetEntity().LineSequence = value; } }
        public string LineStatus { get { return lineItemModel.GetEntity().LineStatus; } set { lineItemModel.GetEntity().LineStatus = value; } }
        public string LineType { get { return lineItemModel.GetEntity().LineType; } set { lineItemModel.GetEntity().LineType = value; } }
        public decimal? ListPrice { get { return lineItemModel.GetEntity().ListPrice; } set { lineItemModel.GetEntity().ListPrice = value; } }
        public decimal? MaxPrice { get { return lineItemModel.GetEntity().MaxPrice; } set { lineItemModel.GetEntity().MaxPrice = value; } }
        public decimal? MaxUsageQuantity { get { return lineItemModel.GetEntity().MaxUsageQuantity; } set { lineItemModel.GetEntity().MaxUsageQuantity = value; } }
        public string MinMaxPriceAppliesTo { get { return lineItemModel.GetEntity().MinMaxPriceAppliesTo; } set { lineItemModel.GetEntity().MinMaxPriceAppliesTo = value; } }
        public decimal? MinPrice { get { return lineItemModel.GetEntity().MinPrice; } set { lineItemModel.GetEntity().MinPrice = value; } }
        public decimal? MinUsageQuantity { get { return lineItemModel.GetEntity().MinUsageQuantity; } set { lineItemModel.GetEntity().MinUsageQuantity = value; } }
        public decimal? NetAdjustmentPercent { get { return lineItemModel.GetEntity().NetAdjustmentPercent; } set { lineItemModel.GetEntity().NetAdjustmentPercent = value; } }
        public decimal? NetPrice { get { return lineItemModel.GetEntity().NetPrice; } set { lineItemModel.GetEntity().NetPrice = value; } }
        public decimal? NetUnitPrice { get { return lineItemModel.GetEntity().NetUnitPrice; } set { lineItemModel.GetEntity().NetUnitPrice = value; } }
        public string OptionId { get { return lineItemModel.GetEntity().OptionId; } set { lineItemModel.GetEntity().OptionId = value; } }
        public decimal? OptionCost { get { return lineItemModel.GetEntity().OptionCost; } set { lineItemModel.GetEntity().OptionCost = value; } }
        public decimal? OptionPrice { get { return lineItemModel.GetEntity().OptionPrice; } set { lineItemModel.GetEntity().OptionPrice = value; } }
        public int? OptionSequence { get { return lineItemModel.GetEntity().OptionSequence; } set { lineItemModel.GetEntity().OptionSequence = value; } }
        public int? ParentBundleNumber { get { return lineItemModel.GetEntity().ParentBundleNumber; } set { lineItemModel.GetEntity().ParentBundleNumber = value; } }
        public string PaymentTermId { get { return lineItemModel.GetEntity().PaymentTermId; } set { lineItemModel.GetEntity().PaymentTermId = value; } }
        public decimal? PriceAdjustment { get { return lineItemModel.GetEntity().PriceAdjustment; } set { lineItemModel.GetEntity().PriceAdjustment = value; } }
        public decimal? PriceAdjustmentAmount { get { return lineItemModel.GetEntity().PriceAdjustmentAmount; } set { lineItemModel.GetEntity().PriceAdjustmentAmount = value; } }
        public string PriceAdjustmentAppliesTo { get { return lineItemModel.GetEntity().PriceAdjustmentAppliesTo; } set { lineItemModel.GetEntity().PriceAdjustmentAppliesTo = value; } }
        public string PriceAdjustmentType { get { return lineItemModel.GetEntity().PriceAdjustmentType; } set { lineItemModel.GetEntity().PriceAdjustmentType = value; } }
        public string PriceGroup { get { return lineItemModel.GetEntity().PriceGroup; } set { lineItemModel.GetEntity().PriceGroup = value; } }
        public bool? PriceIncludedInBundle { get { return lineItemModel.GetEntity().PriceIncludedInBundle; } set { lineItemModel.GetEntity().PriceIncludedInBundle = value; } }
        public string PriceListId { get { return lineItemModel.GetEntity().PriceListId; } set { lineItemModel.GetEntity().PriceListId = value; } }
        public string PriceListItemId { get { return lineItemModel.GetEntity().PriceListItemId; } set { lineItemModel.GetEntity().PriceListItemId = value; } }
        public string PriceMethod { get { return lineItemModel.GetEntity().PriceMethod; } set { lineItemModel.GetEntity().PriceMethod = value; } }
        public string PriceType { get { return lineItemModel.GetEntity().PriceType; } set { lineItemModel.GetEntity().PriceType = value; } }
        public string PriceUom { get { return lineItemModel.GetEntity().PriceUom; } set { lineItemModel.GetEntity().PriceUom = value; } }
        public DateTime? PricingDate { get { return lineItemModel.GetEntity().PricingDate; } set { lineItemModel.GetEntity().PricingDate = value; } }
        public string PricingGuidance { get { return lineItemModel.GetEntity().PricingGuidance; } set { lineItemModel.GetEntity().PricingGuidance = value; } }
        public string PricingStatus { get { return lineItemModel.GetEntity().PricingStatus; } set { lineItemModel.GetEntity().PricingStatus = value; } }
        public string PricingSteps { get { return lineItemModel.GetEntity().PricingSteps; } set { lineItemModel.GetEntity().PricingSteps = value; } }
        public string LocationId { get { return lineItemModel.GetEntity().LocationId; } set { lineItemModel.GetEntity().LocationId = value; } }
        public int PrimaryLineNumber { get { return lineItemModel.GetEntity().PrimaryLineNumber; } set { lineItemModel.GetEntity().PrimaryLineNumber = value; } }
        public string ProductId { get { return lineItemModel.GetEntity().ProductId; } set { lineItemModel.GetEntity().ProductId = value; } }
        public string ProductOptionId { get { return lineItemModel.GetEntity().ProductOptionId; } set { lineItemModel.GetEntity().ProductOptionId = value; } }
        public decimal? RelatedAdjustmentAmount { get { return lineItemModel.GetEntity().RelatedAdjustmentAmount; } set { lineItemModel.GetEntity().RelatedAdjustmentAmount = value; } }
        public string RelatedAdjustmentAppliesTo { get { return lineItemModel.GetEntity().RelatedAdjustmentAppliesTo; } set { lineItemModel.GetEntity().RelatedAdjustmentAppliesTo = value; } }
        public string RelatedAdjustmentType { get { return lineItemModel.GetEntity().RelatedAdjustmentType; } set { lineItemModel.GetEntity().RelatedAdjustmentType = value; } }
        public string RelatedItemId { get { return lineItemModel.GetEntity().RelatedItemId; } set { lineItemModel.GetEntity().RelatedItemId = value; } }
        public decimal? RelatedPercent { get { return lineItemModel.GetEntity().RelatedPercent; } set { lineItemModel.GetEntity().RelatedPercent = value; } }
        public string RelatedPercentAppliesTo { get { return lineItemModel.GetEntity().RelatedPercentAppliesTo; } set { lineItemModel.GetEntity().RelatedPercentAppliesTo = value; } }
        public decimal? Quantity { get { return lineItemModel.GetEntity().Quantity; } set { lineItemModel.GetEntity().Quantity = value; } }
        public decimal? RenewalAdjustmentAmount { get { return lineItemModel.GetEntity().RenewalAdjustmentAmount; } set { lineItemModel.GetEntity().RenewalAdjustmentAmount = value; } }
        public string RenewalAdjustmentType { get { return lineItemModel.GetEntity().RenewalAdjustmentType; } set { lineItemModel.GetEntity().RenewalAdjustmentType = value; } }
        public string RollupPriceMethod { get { return lineItemModel.GetEntity().RollupPriceMethod; } set { lineItemModel.GetEntity().RollupPriceMethod = value; } }
        public bool? RollupPriceToBundle { get { return lineItemModel.GetEntity().RollupPriceToBundle; } set { lineItemModel.GetEntity().RollupPriceToBundle = value; } }
        public string ShipToAccountId { get { return lineItemModel.GetEntity().ShipToAccountId; } set { lineItemModel.GetEntity().ShipToAccountId = value; } }
        public DateTime? StartDate { get { return lineItemModel.GetEntity().StartDate; } set { lineItemModel.GetEntity().StartDate = value; } }
        public string SellingFrequency { get { return lineItemModel.GetEntity().SellingFrequency; } set { lineItemModel.GetEntity().SellingFrequency = value; } }
        public decimal? SellingTerm { get { return lineItemModel.GetEntity().SellingTerm; } set { lineItemModel.GetEntity().SellingTerm = value; } }
        public string SellingUom { get { return lineItemModel.GetEntity().SellingUom; } set { lineItemModel.GetEntity().SellingUom = value; } }
        public string SummaryGroupId { get { return lineItemModel.GetEntity().SummaryGroupId; } set { lineItemModel.GetEntity().SummaryGroupId = value; } }
        public bool? Taxable { get { return lineItemModel.GetEntity().Taxable; } set { lineItemModel.GetEntity().Taxable = value; } }
        public string TaxCodeId { get { return lineItemModel.GetEntity().TaxCodeId; } set { lineItemModel.GetEntity().TaxCodeId = value; } }
        public bool? TaxInclusive { get { return lineItemModel.GetEntity().TaxInclusive; } set { lineItemModel.GetEntity().TaxInclusive = value; } }
        public decimal? Term { get { return lineItemModel.GetEntity().Term; } set { lineItemModel.GetEntity().Term = value; } }
        public string TransferPriceLineItemId { get { return lineItemModel.GetEntity().TransferPriceLineItemId; } set { lineItemModel.GetEntity().TransferPriceLineItemId = value; } }
        public decimal? TotalQuantity { get { return lineItemModel.GetEntity().TotalQuantity; } set { lineItemModel.GetEntity().TotalQuantity = value; } }
        public decimal? UnitCostAdjustment { get { return lineItemModel.GetEntity().UnitCostAdjustment; } set { lineItemModel.GetEntity().UnitCostAdjustment = value; } }
        public decimal? UnitPriceAdjustmentAuto { get { return lineItemModel.GetEntity().UnitPriceAdjustmentAuto; } set { lineItemModel.GetEntity().UnitPriceAdjustmentAuto = value; } }
        public decimal? UnitPriceAdjustmentManual { get { return lineItemModel.GetEntity().UnitPriceAdjustmentManual; } set { lineItemModel.GetEntity().UnitPriceAdjustmentManual = value; } }
        public string Uom { get { return lineItemModel.GetEntity().Uom; } set { lineItemModel.GetEntity().Uom = value; } }

        public IPriceListItemModel GetPriceListItem()
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

        public IProductLineItemModel GetRootParentLineItem()
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

        public bool IsOptionLineType()
        {
            return lineItemModel.GetLineType() == Lightsaber.Pricing.Common.Callback.Enums.LineType.Option;
        }

        public bool IsProductServiceLineType()
        {
            return lineItemModel.GetLineType() == Lightsaber.Pricing.Common.Callback.Enums.LineType.ProductService;
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

        public string Apttus_Config2__ConfigStatus__c { get { return lineItemModel.Get<string>(LineItemField.Apttus_Config2__ConfigStatus__c); } set { lineItemModel.Set(LineItemField.Apttus_Config2__ConfigStatus__c, value); } }
        public bool? Apttus_Config2__IsReadOnly__c { get { return lineItemModel.Get<bool?>(LineItemField.Apttus_Config2__IsReadOnly__c); } set { lineItemModel.Set(LineItemField.Apttus_Config2__IsReadOnly__c, value); } }
        public bool? Apttus_Config2__IsHidden__c { get { return lineItemModel.Get<bool?>(LineItemField.Apttus_Config2__IsHidden__c); } set { lineItemModel.Set(LineItemField.Apttus_Config2__IsHidden__c, value); } }
        public string Custom_Product_Code__c { get { return lineItemModel.Get<string>(LineItemField.Custom_Product_Code__c); } set { lineItemModel.Set(LineItemField.Custom_Product_Code__c, value); } }
        public string CustomProductValue__c { get { return lineItemModel.Get<string>(LineItemField.CustomProductValue__c); } set { lineItemModel.Set(LineItemField.CustomProductValue__c, value); } }
        public bool? NokiaCPQ_Is_SI__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_Is_SI__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Is_SI__c, value); } }
        public string Source__c { get { return lineItemModel.Get<string>(LineItemField.Source__c); } set { lineItemModel.Set(LineItemField.Source__c, value); } }
        public decimal? Total_ONT_Quantity__c { get { return lineItemModel.Get<decimal?>(LineItemField.Total_ONT_Quantity__c); } set { lineItemModel.Set(LineItemField.Total_ONT_Quantity__c, value); } }
        public decimal? Total_ONT_Quantity_FBA__c { get { return lineItemModel.Get<decimal?>(LineItemField.Total_ONT_Quantity_FBA__c); } set { lineItemModel.Set(LineItemField.Total_ONT_Quantity_FBA__c, value); } }
        public decimal? Total_ONT_Quantity_P2P__c { get { return lineItemModel.Get<decimal?>(LineItemField.Total_ONT_Quantity_P2P__c); } set { lineItemModel.Set(LineItemField.Total_ONT_Quantity_P2P__c, value); } }
        public decimal? NCPQ_Unitary_CLP__c { get { return lineItemModel.Get<decimal?>(LineItemField.NCPQ_Unitary_CLP__c); } set { lineItemModel.Set(LineItemField.NCPQ_Unitary_CLP__c, value); } }
        public bool? NokiaCPQ_Spare__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_Spare__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Spare__c, value); } }
        public string NokiaCPQ_Product_Type__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_Product_Type__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Product_Type__c, value); } }
        public decimal? NokiaCPQ_Extended_CLP_2__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Extended_CLP_2__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Extended_CLP_2__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr1_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Yr1_Extended_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Yr1_Extended_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Yr2_Extended_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Yr2_Extended_Price__c, value); } }
        public decimal? Nokia_SRS_Base_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_SRS_Base_Extended_Price__c); } set { lineItemModel.Set(LineItemField.Nokia_SRS_Base_Extended_Price__c, value); } }
        public decimal? Nokia_SSP_Base_Extended_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_SSP_Base_Extended_Price__c); } set { lineItemModel.Set(LineItemField.Nokia_SSP_Base_Extended_Price__c, value); } }
        public decimal? NokiaCPQ_Unitary_IRP__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Unitary_IRP__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Unitary_IRP__c, value); } }
        public decimal? NokiaCPQ_Extended_CNP__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Extended_CNP__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Extended_CNP__c, value); } }
        public decimal? NokiaCPQ_AdvancePricing_CUP__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_AdvancePricing_CUP__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_AdvancePricing_CUP__c, value); } }
        public decimal? NokiaCPQ_ExtendedPrice_CNP__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_ExtendedPrice_CNP__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_ExtendedPrice_CNP__c, value); } }
        public bool? Advanced_Pricing_Done__c { get { return lineItemModel.Get<bool?>(LineItemField.Advanced_Pricing_Done__c); } set { lineItemModel.Set(LineItemField.Advanced_Pricing_Done__c, value); } }
        public decimal? NokiaCPQ_AdvancePricing_NP__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_AdvancePricing_NP__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_AdvancePricing_NP__c, value); } }
        public decimal? NokiaCPQ_Extended_CUP__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Extended_CUP__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Extended_CUP__c, value); } }
        public decimal? NokiaCPQ_Unitary_Cost__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Unitary_Cost__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Unitary_Cost__c, value); } }
        public string NokiaCPQ_Light_Color__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_Light_Color__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Light_Color__c, value); } }
        public decimal? NokiaCPQ_IRP_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_IRP_Discount__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_IRP_Discount__c, value); } }
        public string NokiaCPQ_Account_Region__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_Account_Region__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Account_Region__c, value); } }
        public string NokiaCPQ_Org__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_Org__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Org__c, value); } }
        public string NokiaCPQ_BU__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_BU__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_BU__c, value); } }
        public string NokiaCPQ_BG__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_BG__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_BG__c, value); } }
        public decimal? NokiaCPQ_Floor_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Floor_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Floor_Price__c, value); } }
        public decimal? NokiaCPQ_Market_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Market_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Market_Price__c, value); } }
        public bool? NokiaCPQ_Custom_Bid__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_Custom_Bid__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Custom_Bid__c, value); } }
        public string Product_Extension__c { get { return lineItemModel.Get<string>(LineItemField.Product_Extension__c); } set { lineItemModel.Set(LineItemField.Product_Extension__c, value); } }
        public bool? Is_List_Price_Only__c { get { return lineItemModel.Get<bool?>(LineItemField.Is_List_Price_Only__c); } set { lineItemModel.Set(LineItemField.Is_List_Price_Only__c, value); } }
        public bool? NokiaCPQ_Is_Direct_Option__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_Is_Direct_Option__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Is_Direct_Option__c, value); } }
        public decimal? Nokia_Maint_Y1_Per__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_Maint_Y1_Per__c); } set { lineItemModel.Set(LineItemField.Nokia_Maint_Y1_Per__c, value); } }
        public decimal? Nokia_Maint_Y2_Per__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_Maint_Y2_Per__c); } set { lineItemModel.Set(LineItemField.Nokia_Maint_Y2_Per__c, value); } }
        public decimal? NokiaCPQ_SSP_Rate__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_SSP_Rate__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_SSP_Rate__c, value); } }
        public decimal? NokiaCPQ_SRS_Rate__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_SRS_Rate__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_SRS_Rate__c, value); } }
        public decimal? NokiaCPQ_Unitary_Cost_Initial__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Unitary_Cost_Initial__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Unitary_Cost_Initial__c, value); } }
        public decimal? Total_Option_Quantity__c { get { return lineItemModel.Get<decimal?>(LineItemField.Total_Option_Quantity__c); } set { lineItemModel.Set(LineItemField.Total_Option_Quantity__c, value); } }
        public decimal? NokiaCPQ_IncotermNew__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_IncotermNew__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_IncotermNew__c, value); } }
        public bool? Is_Contract_Pricing_2__c { get { return lineItemModel.Get<bool?>(LineItemField.Is_Contract_Pricing_2__c); } set { lineItemModel.Set(LineItemField.Is_Contract_Pricing_2__c, value); } }
        public string NokiaCPQAccreditationType__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQAccreditationType__c); } set { lineItemModel.Set(LineItemField.NokiaCPQAccreditationType__c, value); } }
        public string Nokia_Pricing_Cluster__c { get { return lineItemModel.Get<string>(LineItemField.Nokia_Pricing_Cluster__c); } set { lineItemModel.Set(LineItemField.Nokia_Pricing_Cluster__c, value); } }
        public string Nokia_Maintenance_Level__c { get { return lineItemModel.Get<string>(LineItemField.Nokia_Maintenance_Level__c); } set { lineItemModel.Set(LineItemField.Nokia_Maintenance_Level__c, value); } }
        public string Nokia_Maint_Pricing_Cluster__c { get { return lineItemModel.Get<string>(LineItemField.Nokia_Maint_Pricing_Cluster__c); } set { lineItemModel.Set(LineItemField.Nokia_Maint_Pricing_Cluster__c, value); } }
        public decimal? NokiaCPQ_Attachment_Per__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Attachment_Per__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Attachment_Per__c, value); } }
        public decimal? NokiaCPQ_Renewal_Per__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Renewal_Per__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Renewal_Per__c, value); } }
        public decimal? NokiaCPQ_Performance_Per__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Performance_Per__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Performance_Per__c, value); } }
        public decimal? NokiaCPQ_Multi_Yr_Per__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Multi_Yr_Per__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Multi_Yr_Per__c, value); } }
        public decimal? NokiaCPQ_Maint_Accreditation_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Accreditation_Discount__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Accreditation_Discount__c, value); } }
        public decimal? NokiaCPQ_Total_Maintenance_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Total_Maintenance_Discount__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Total_Maintenance_Discount__c, value); } }
        public decimal? NokiaCPQ_Accreditation_Discount__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Accreditation_Discount__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Accreditation_Discount__c, value); } }
        public decimal? Nokia_CPQ_Maint_Prod_Cat_Disc__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_CPQ_Maint_Prod_Cat_Disc__c); } set { lineItemModel.Set(LineItemField.Nokia_CPQ_Maint_Prod_Cat_Disc__c, value); } }
        public bool? Is_Custom_Product__c { get { return lineItemModel.Get<bool?>(LineItemField.Is_Custom_Product__c); } set { lineItemModel.Set(LineItemField.Is_Custom_Product__c, value); } }
        public decimal? Nokia_SSP_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_SSP_List_Price__c); } set { lineItemModel.Set(LineItemField.Nokia_SSP_List_Price__c, value); } }
        public decimal? Nokia_SSP_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_SSP_Base_Price__c); } set { lineItemModel.Set(LineItemField.Nokia_SSP_Base_Price__c, value); } }
        public decimal? Nokia_SRS_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_SRS_List_Price__c); } set { lineItemModel.Set(LineItemField.Nokia_SRS_List_Price__c, value); } }
        public decimal? Nokia_SRS_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_SRS_Base_Price__c); } set { lineItemModel.Set(LineItemField.Nokia_SRS_Base_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Y1_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Y1_List_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Y1_List_Price__c, value); } }
        public decimal? Nokia_Maint_Y1_Extended_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Nokia_Maint_Y1_Extended_List_Price__c); } set { lineItemModel.Set(LineItemField.Nokia_Maint_Y1_Extended_List_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Yr2_List_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Yr2_List_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_Extended_List_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Yr2_Extended_List_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Yr2_Extended_List_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr1_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Yr1_Base_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Yr1_Base_Price__c, value); } }
        public decimal? NokiaCPQ_Maint_Yr2_Base_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Maint_Yr2_Base_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Maint_Yr2_Base_Price__c, value); } }
        public bool? Is_FBA__c { get { return lineItemModel.Get<bool?>(LineItemField.Is_FBA__c); } set { lineItemModel.Set(LineItemField.Is_FBA__c, value); } }
        public bool? Is_P2P__c { get { return lineItemModel.Get<bool?>(LineItemField.Is_P2P__c); } set { lineItemModel.Set(LineItemField.Is_P2P__c, value); } }
        public decimal? NokiaCPQ_SRSBasePrice__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_SRSBasePrice__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_SRSBasePrice__c, value); } }
        public decimal? NokiaCPQ_CareSRSBasePrice__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_CareSRSBasePrice__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_CareSRSBasePrice__c, value); } }
        public decimal? Reference_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Reference_Price__c); } set { lineItemModel.Set(LineItemField.Reference_Price__c, value); } }
        public decimal? Equivalent_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.Equivalent_Price__c); } set { lineItemModel.Set(LineItemField.Equivalent_Price__c, value); } }
        public decimal? NokiaCPQ_Extended_CUP_2__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Extended_CUP_2__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Extended_CUP_2__c, value); } }
        public decimal? NokiaCPQ_Extended_IRP2__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Extended_IRP2__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Extended_IRP2__c, value); } }
        public decimal? Sales_Margin__c { get { return lineItemModel.Get<decimal?>(LineItemField.Sales_Margin__c); } set { lineItemModel.Set(LineItemField.Sales_Margin__c, value); } }
        public string Item_Type_From_CAT__c { get { return lineItemModel.Get<string>(LineItemField.Item_Type_From_CAT__c); } set { lineItemModel.Set(LineItemField.Item_Type_From_CAT__c, value); } }
        public bool? NokiaCPQ_IsArcadiaBundle__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_IsArcadiaBundle__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_IsArcadiaBundle__c, value); } }
        public bool? NokiaCPQ_Is_CLP_in_PDC__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_Is_CLP_in_PDC__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Is_CLP_in_PDC__c, value); } }
        public decimal? NokiaCPQ_Extended_CNP_Without_LSD__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Extended_CNP_Without_LSD__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Extended_CNP_Without_LSD__c, value); } }
        public bool? NokiaCPQ_Is_Contracted_PLI__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_Is_Contracted_PLI__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Is_Contracted_PLI__c, value); } }
        public decimal? NokiaCPQ_Extended_Floor_Price__c { get { return lineItemModel.Get<decimal?>(LineItemField.NokiaCPQ_Extended_Floor_Price__c); } set { lineItemModel.Set(LineItemField.NokiaCPQ_Extended_Floor_Price__c, value); } }

        #endregion

        #region Relationship LineItem Fields

        public string Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_NokiaCPQ_Item_Type__c); } }
        public string Apttus_Config2__ProductId__r_Business_Group__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_Business_Group__c); } }
        public string Apttus_Config2__ProductId__r_NokiaCPQ_Product_Discount_Category__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_NokiaCPQ_Product_Discount_Category__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_Product_Discount_Category__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Product_Discount_Category__c); } }
        public string Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_Apttus_Config2__ConfigurationType__c); } }
        public string Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__OptionId__r_Apttus_Config2__ConfigurationType__c); } }
        public string Apttus_Config2__ProductId__r_NokiaCPQ_Classification2__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_NokiaCPQ_Classification2__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_Classification2__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Classification2__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_Item_Type__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Item_Type__c); } }
        public string Apttus_Config2__ProductId__r_NokiaCPQ_License_Usage__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_NokiaCPQ_License_Usage__c); } }
        public string Apttus_Config2__OptionId__r_NokiaCPQ_License_Usage__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_License_Usage__c); } }
        public string Apttus_Config2__ProductId__r_ProductCode { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_ProductCode); } }
        public string Apttus_Config2__OptionId__r_ProductCode { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__OptionId__r_ProductCode); } }
        public string Apttus_Config2__ProductId__r_Family { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_Family); } }
        public bool? Apttus_Config2__ProductId__r_IsSSP__c { get { return lineItemModel.GetLookupValue<bool?>(LineItemField.Apttus_Config2__ProductId__r_IsSSP__c); } }
        public string Apttus_Config2__ProductId__r_Is_Dummy_Bundle_CPQ__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_Is_Dummy_Bundle_CPQ__c); } }
        public string Apttus_Config2__OptionId__r_Is_Dummy_Bundle_CPQ__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__OptionId__r_Is_Dummy_Bundle_CPQ__c); } }
        public string Apttus_Config2__ProductId__r_Portfolio__c { get { return lineItemModel.GetLookupValue<string>(LineItemField.Apttus_Config2__ProductId__r_Portfolio__c); } }
        public decimal? Apttus_Config2__OptionId__r_Number_of_GE_Ports__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_Number_of_GE_Ports__c); } }
        public decimal? Apttus_Config2__ProductId__r_Number_of_GE_Ports__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__ProductId__r_Number_of_GE_Ports__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_North_America__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_North_America__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Latin_America__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Latin_America__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQMin_SM_Middle_East_and_Africa__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQMin_SM_Middle_East_and_Africa__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_India__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_India__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Asia_Pacific_Japan__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Asia_Pacific_Japan__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Greater_China__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Greater_China__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Europe__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Min_SM_Europe__c); } }
        public decimal? Apttus_Config2__ProductId__r_NokiaCPQ_Maximum_IRP_Discount__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__ProductId__r_NokiaCPQ_Maximum_IRP_Discount__c); } }
        public decimal? Apttus_Config2__OptionId__r_NokiaCPQ_Maximum_IRP_Discount__c { get { return lineItemModel.GetLookupValue<decimal?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_Maximum_IRP_Discount__c); } }
        public bool? Apttus_Config2__ProductId__r_NokiaCPQ_OEM__c { get { return lineItemModel.GetLookupValue<bool?>(LineItemField.Apttus_Config2__ProductId__r_NokiaCPQ_OEM__c); } }
        public bool? Apttus_Config2__OptionId__r_NokiaCPQ_OEM__c { get { return lineItemModel.GetLookupValue<bool?>(LineItemField.Apttus_Config2__OptionId__r_NokiaCPQ_OEM__c); } }
        #endregion

        #region Formula fields and supported methods

        public string Portfolio_from_Quote_Line_Item__c { get { return lineItemModel.Get<string>(LineItemField.Portfolio_from_Quote_Line_Item__c); } }
        public string NokiaCPQ_Configuration_Type__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_Configuration_Type__c); } }
        public string Quote_Type__c { get { return lineItemModel.Get<string>(LineItemField.Quote_Type__c); } }
        public bool? NokiaCPQ_Static_Bundle_Option__c { get { return lineItemModel.Get<bool?>(LineItemField.NokiaCPQ_Static_Bundle_Option__c); }}
        public string NokiaCPQ_Category__c { get { return lineItemModel.Get<string>(LineItemField.NokiaCPQ_Category__c); } }
        public decimal? Product_Number_Of_Ports__c { get { return lineItemModel.Get<decimal?>(LineItemField.Product_Number_Of_Ports__c); } }

        public decimal? NokiaCPQ_Extended_CLP__c { get { return CalculateExtendedCLP(); } }
        public decimal? NokiaCPQ_Extended_IRP__c { get { return CalculateExtendedIRP(); } }
        public decimal? NokiaCPQ_ExtendedPrice_CUP__c { get { return CalculateExtendedPriceCUP(); } }
        public decimal? NokiaCPQ_Extended_Cost__c { get { return CalculateExtendedCost(); } }


        private decimal? CalculatePartnerPrice()
        {
            decimal? partnerPrice;
            var configType = NokiaCPQ_Configuration_Type__c;

            if (configType.equals(Constants.BUNDLE))
            {
                partnerPrice = NCPQ_Unitary_CLP__c;
            }
            else
            {
                partnerPrice = BasePriceOverride ?? BasePrice;
            }

            return partnerPrice;
        }

        private decimal? CalculateExtendedCLP()
        {
            decimal? partnerPrice = CalculatePartnerPrice();
            return partnerPrice * Quantity;
        }

        private decimal? CalculateExtendedIRP()
        {
            decimal? unitaryIRPPrice;

            if (Quote_Type__c == Constants.QUOTE_TYPE_DIRECTCPQ && Portfolio_from_Quote_Line_Item__c == Constants.AIRSCALE_WIFI_STRING)
            {
                unitaryIRPPrice = NokiaCPQ_Unitary_IRP__c;
            }
            else
            {
                if (ChargeType == Constants.STANDARD_PRICE)
                {
                    unitaryIRPPrice = NokiaCPQ_Unitary_IRP__c;
                }
                else
                {
                    unitaryIRPPrice = CalculatePartnerPrice();
                }
            }

            return unitaryIRPPrice * Quantity;
        }

        private decimal? CalculateExtendedPriceCUP()
        {
            var configType = NokiaCPQ_Configuration_Type__c;
            if (Portfolio_from_Quote_Line_Item__c == Constants.AIRSCALE_WIFI_STRING && configType == Constants.NOKIA_STANDALONE && IsOptionLineType())
            {
                if (ExtendedQuantity != null && ExtendedQuantity != 0)
                {
                    return (AdjustedPrice / ExtendedQuantity) * Quantity;
                }
                else
                {
                    return AdjustedPrice;
                }
            }
            else
            {
                if (configType.equals(Constants.BUNDLE))
                {
                    if (IsProductServiceLineType())
                    {
                        return NokiaCPQ_Extended_CUP__c * Quantity;
                    }
                    else
                    {
                        return NokiaCPQ_Extended_CUP__c;
                    }
                }
                else
                {
                    return AdjustedPrice;
                }
            }
        }

        private decimal? CalculateExtendedCost()
        {
            var configType = NokiaCPQ_Configuration_Type__c;
            if ((configType.equals(Constants.BUNDLE) && IsOptionLineType()) || NokiaCPQ_IsArcadiaBundle__c == true)
            {
                return NokiaCPQ_Unitary_Cost__c;
            }
            else
            {
                return NokiaCPQ_Unitary_Cost__c * Quantity;
            }
        }

        #endregion
    }
}