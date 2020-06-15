/*
    @TestClass APTS_CustomPricingCallBackTest_Ultra
*/
class APTS_CustomPricingCallBack_Switch
{
    private Apttus_Config2.CustomClass.PricingMode mode = null;
    private Apttus_Config2.ProductConfiguration cart = null;
    public APTS_CustomPricingCallBackHelper_Ultra pcbHelper_Ultra;
    Map<String, APTS_Local_Bundle_Component__c> componentSOMap = new Map<String, APTS_Local_Bundle_Component__c>();

    Set<Decimal> lineNumWithLocalBundleSet = new Set<Decimal>();

    /**
        * Callback at the beginning of the pricing call.
        * Use the start method to initialize state
        * @param cart the cart object
    */
    void start(Apttus_Config2.ProductConfiguration cart)
    {
        this.cart = cart;
        Apttus_Config2__ProductConfiguration__c prodConfigSO = cart.getConfigSO();

        Apttus_Proposal__Proposal__c proposalSO = [SELECT ID, RecordTypeId, Apttus_QPConfig__PriceListId__c, APTS_ContractNumbers__c,
                                                    APTS_Billing_Plan__c, APTS_Inco_Term__c, Apttus_Proposal__Payment_Term__c, APTS_Acoount_Agreement__c,
                                                    APTS_Country_code__c, APTS_Solution__c, Account_Sold_to__c
                                                    FROM Apttus_Proposal__Proposal__c WHERE ID = :prodConfigSO.Apttus_QPConfig__Proposald__c];

        List<Apttus_Config2.LineItem> allLines = cart.getLineItems();
        
        Set<String> localBundleOptionSet = new Set<String>();
        for (Apttus_Config2.LineItem lineItemMO : allLines)
        {
            Apttus_Config2__LineItem__c liSO = lineItemMO.getLineItemSO();
            if (liSO.Apttus_Config2__OptionId__c != null && liSO.Apttus_Config2__OptionId__r.APTS_Local_Bundle__c)
            {
                lineNumWithLocalBundleSet.add(liSO.Apttus_Config2__LineNumber__c);
                localBundleOptionSet.add(liSO.Apttus_Config2__ProductId__c + '' + liSO.Apttus_Config2__OptionId__c);
            }
        }

        pcbHelper_Ultra = new APTS_CustomPricingCallBackHelper_Ultra(proposalSO);

        List<APTS_Local_Bundle_Header__c> localBundleHeaderSOList = [select id, APTS_Local_Bundle__c, APTS_Parent_Bundle__c, APTS_Parent_Local_Bundle__c, (select id, APTS_Component__c, APTS_Local_Bundle_Header__c from Local_Bundle_Components__r where APTS_Active__c = TRUE) from APTS_Local_Bundle_Header__c where APTS_Active__c = TRUE AND APTS_Parent_Local_Bundle__c IN: localBundleOptionSet];

        for (APTS_Local_Bundle_Header__c localBundleHeaderSO : localBundleHeaderSOList)
        {
            List<APTS_Local_Bundle_Component__c> componentSOList = localBundleHeaderSO.Local_Bundle_Components__r;
            for (APTS_Local_Bundle_Component__c componentSO : componentSOList)
            {
                componentSOMap.put(componentSO.APTS_Component__c + '' + localBundleHeaderSO.APTS_Parent_Bundle__c, componentSO);
            }
        }
    }

    /**
        * Callback to indicate the pricing mode
        * @param mode the pricing mode
    */
    void setMode(Apttus_Config2.CustomClass.PricingMode mode)
    {
        this.mode = mode;
    }

    /**
        * Callback before pricing the line item collection
        * Use this method to do all required pre-processing to prepare the line items for pricing.
        * @param itemColl the line item collection to pre-process
    */
    void beforePricing(Apttus_Config2.ProductConfiguration.LineItemColl itemColl)
    {

        list<Apttus_Config2.LineItem> allLines = itemColl.getAllLineItems();
        list<Apttus_Config2__LineItem__c> lineItems = new list<Apttus_Config2__LineItem__c>();
        for (Apttus_Config2.LineItem lineItemMO : allLines)
        {
            lineItems.add(lineItemMO.getLineItemSO());
        }

        if (Apttus_Config2.CustomClass.PricingMode.BASEPRICE == mode)
        {
            //F23182 - Updated by Prabhat Rai
            pcbHelper_Ultra.populateExtendedQuantity(lineItems);

            //F22944 - Added by Bogdan Botcharov - to be referenced in onPriceItemSet()
            //All logic moved to onpriceitemset method
            pcbHelper_Ultra.populateMapSPOOPLi(lineItems);

            //Tiered Pricing - Added by Bogdan Botcharov - to be referenced in onPriceItemSet()
            //All logic moved to onpriceitemset method
            pcbHelper_Ultra.populateMapPliAgreementContractsBeforePricing(lineItems);
        }

        if (Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT == mode)
        {
            //462170 - Added by Bogdan Botcharov [Nov 11 2019]
            pcbHelper_Ultra.incentiveAdjustmentUnitRounding(lineItems);

            //DE60536 - Removed for PACs
            if (pcbHelper_Ultra.proposalSO != null)
                pcbHelper_Ultra.setDiscountWithAdjustmentSpread(lineItems);

        }
    }

    /**
        * Callback before pricing the given line item in the line item collection
        * Use this method to do all required pre-processing to prepare the line item for pricing.
        * @param itemColl the line item collectionholding the line item
        * @param lineItemMO the line item to pre-process
    */
    void beforePricingLineItem(Apttus_Config2.ProductConfiguration.LineItemColl itemColl, Apttus_Config2.LineItem lineItemMO)
    {
    }

    void onPriceItemSet(Apttus_Config2__PriceListItem__c itemSO, Apttus_Config2.LineItem lineItemMO)
    {
        Apttus_Config2__LineItem__c lineItemSO = lineItemMO.getLineItemSO();
        //F21718 - Added by Bogdan Botcharov
        pcbHelper_Ultra.calculateSPOOPricing(itemSO, lineItemSO);
        //Added by sridhar to set the extended list price
        if (pcbHelper_Ultra.proposalSO != null)
        {
            if (lineItemSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c == 'Service')
            {
                if (lineItemSO.APTS_Service_List_Price__c != null)
                    itemSO.Apttus_Config2__ListPrice__c = lineItemSO.APTS_Service_List_Price__c;

                if (lineItemSO.APTS_Minimum_Price_Service__c != null)
                    lineItemSO.Apttus_Config2__MinPrice__c = lineItemSO.APTS_Minimum_Price_Service__c;

                if (lineItemSO.APTS_Cost_Service__c != null)
                {
                    itemSO.Apttus_Config2__Cost__c = lineItemSO.APTS_Cost_Service__c;
                    lineItemSO.Apttus_Config2__BaseCost__c = lineItemSO.APTS_Cost_Service__c;
                }

                Decimal sellingTerm = lineItemSO.Apttus_Config2__SellingTerm__c != null && lineItemSO.Apttus_Config2__SellingTerm__c != 0 ? lineItemSO.Apttus_Config2__SellingTerm__c : 1;
                Decimal sapListPrice = lineItemSO.APTS_Service_List_Price__c != null ? lineItemSO.APTS_Service_List_Price__c : 0;
                Decimal extPrice = sapListPrice * lineItemSO.APTS_Extended_Quantity__c * sellingTerm;
                lineItemSO.APTS_Extended_List_Price__c = extPrice;

                Decimal lineExtQ = lineItemSO.APTS_Extended_Quantity__c != null ? lineItemSO.APTS_Extended_Quantity__c : 0;
                Decimal lineQ = lineItemSO.Apttus_Config2__Quantity__c != null && lineItemSO.Apttus_Config2__Quantity__c != 0 ? lineItemSO.Apttus_Config2__Quantity__c : 1;
                lineItemSO.APTS_Option_Unit_Price__c = (sapListPrice * lineExtQ) / lineQ;
            }
            else
            {

                Decimal sellingTerm = lineItemSO.Apttus_Config2__SellingTerm__c != null && lineItemSO.Apttus_Config2__SellingTerm__c != 0 ? lineItemSO.Apttus_Config2__SellingTerm__c : 1;
                Decimal sapListPrice = itemSO.Apttus_Config2__ListPrice__c;
                Decimal lineExtQ = lineItemSO.APTS_Extended_Quantity__c != null ? lineItemSO.APTS_Extended_Quantity__c : 0;
                Decimal extPrice = sapListPrice * lineExtQ * sellingTerm;
                lineItemSO.APTS_Extended_List_Price__c = extPrice;
                Decimal lineQ = lineItemSO.Apttus_Config2__Quantity__c != null && lineItemSO.Apttus_Config2__Quantity__c != 0 ? lineItemSO.Apttus_Config2__Quantity__c : 1;
                lineItemSO.APTS_Option_Unit_Price__c = (sapListPrice * lineExtQ) / lineQ;
            }
        }

        if (lineItemSO.Apttus_Config2__ProductId__r.Apttus_Config2__ProductType__c != 'Service')
            pcbHelper_Ultra.populateTierOnPriceItemSet(lineItemMO, itemSO);
        
        //Added by Sridhar for Local Bundle.
        if (lineItemSO.Apttus_Config2__OptionId__c != null && !lineItemSO.Apttus_Config2__OptionId__r.APTS_Local_Bundle__c && lineNumWithLocalBundleSet.contains(lineItemSO.Apttus_Config2__LineNumber__c))
        {
            APTS_Local_Bundle_Component__c componentSO = componentSOMap.get(lineItemSO.Apttus_Config2__OptionId__c + '' + lineItemSO.Apttus_Config2__ProductID__c);
            if (componentSO != null)
            {
                lineItemSO.APTS_Local_Bundle_Header__c = componentSO.APTS_Local_Bundle_Header__c;
                lineItemSO.APTS_Local_Bundle_Component__c = componentSO.ID;
                lineItemSO.Apttus_Config2__IsQuantityModifiable__c = false;
                lineItemSO.APTS_Local_Bundle_Component_Flag__c = true;

                itemSO.Apttus_Config2__AllowManualAdjustment__c = false;
                itemSO.Apttus_Config2__AllocateGroupAdjustment__c = false;
                itemSO.Apttus_Config2__ListPrice__c = 0;
                itemSO.Apttus_Config2__MinPrice__c = 0;

                lineItemSO.APTS_ContractDiscount__c = null;
                lineItemSO.Apttus_Config2__MinPrice__c = 0;
                lineItemSO.APTS_Extended_List_Price__c = 0;
                lineItemSO.APTS_Option_Unit_Price__c = 0;
            }
        }
    }

    /**
        * Callback after pricing the given line item in the line item collection
        * Use this method to do all required post-processing after the line item is priced
        * @param itemColl the line item collection holding the line item
        * @param lineItemMO the line item to post-process
    */
    void afterPricingLineItem(Apttus_Config2.ProductConfiguration.LineItemColl itemColl, Apttus_Config2.LineItem lineItemMO)
    {

    }

    /**
        * Callback after pricing the line item collection
        * Use this method to do all required post-processing after line items are priced.
        * @param itemColl the line item collection to post-process
    */
    void afterPricing(Apttus_Config2.ProductConfiguration.LineItemColl itemColl)
    {
        List<Apttus_Config2.LineItem> allLines = itemColl.getAllLineItems();

        if (this.mode == Apttus_Config2.CustomClass.PricingMode.ADJUSTMENT)
        {
            //GP: this will be removed if second reprice removed.
            if (pcbHelper_Ultra.proposalSO != null)
            {
                for (Apttus_Config2.LineItem lineMO : allLines)
                {
                    Apttus_Config2__LineItem__c liSO = lineMO.getLineItemSO();
                    liSO.Apttus_Config2__NetPrice__c = liSO.Apttus_Config2__LineType__c == 'Product/Service' ? liSO.APTS_Solution_Offered_Price__c : liSO.APTS_Offered_Price_c__c;
                }
            }

            if (pcbHelper_Ultra.proposalSO != null)
            {
                pcbHelper_Ultra.computeNetAdjustment(allLines);
            }

            pcbHelper_Ultra.populateCustomFields(allLines);

            if (pcbHelper_Ultra.proposalSO != null)
                pcbHelper_Ultra.calculatePricePointsForBundle(allLines);
        }
    }

    /**
        * Callback after all batches of line items are processed
        * Use the finish method to release state
    */
    void finish()
    {
        List<Apttus_Config2.LineItem> allLines = cart.getLineItems();
        List<Apttus_Config2__LineItem__c> listLineItemSO = new List<Apttus_Config2__LineItem__c>();

        for (Apttus_Config2.LineItem lineItemMO : allLines)
        {
            Apttus_Config2__LineItem__c lineItemSO = lineItemMO.getLineItemSO();
            listLineItemSO.add(lineItemMO.getLineItemSO());
        }

        if (pcbHelper_Ultra.proposalSO != null)
        {
            pcbHelper_Ultra.SetRollupsAndThresholdFlags(cart.getConfigSO(), listLineItemSO);
        }
    }
}