

namespace jrs.Models
{
    using System;
    public partial class NavPayload
    {
        public string data { get; set; }
        public string info { get; set; }

        /// <summary>
        /// Generates a string representation of the GenericSqlPayload instance.
        /// </summary>
        /// <returns>
        /// The string representation of the object.
        /// </returns>
        public override string ToString()
        {
            string ret = $"{{\"data\":\"{this.data}\", \"info\":\"{this.info}\"}}";
            return ret;
        }
    }
    public partial class NavGLS
    {
        public string Primary_Key { get; set; }
        public string LCY_Code { get; set; }
        public string Local_Currency_Description { get; set; }
        public string Global_Dimension_1_Code { get; set; }
        public string Global_Dimension_2_Code { get; set; }
        public string Shortcut_Dimension_3_Code { get; set; }
        public string Shortcut_Dimension_4_Code { get; set; }
        public string Shortcut_Dimension_5_Code { get; set; }
        public string Shortcut_Dimension_6_Code { get; set; }
        public string Shortcut_Dimension_7_Code { get; set; }
        public string Shortcut_Dimension_8_Code { get; set; }
        public string Shortcut_Dimension_9_Code { get; set; }
        public string Shortcut_Dimension_10_Code { get; set; }
        public string Shortcut_Dimension_11_Code { get; set; }
        public string Shortcut_Dimension_12_Code { get; set; }
        public string Shortcut_Dimension_13_Code { get; set; }
        public string Shortcut_Dimension_14_Code { get; set; }
        public string Shortcut_Dimension_15_Code { get; set; }
        public string Shortcut_Dimension_16_Code { get; set; }
    }

    public partial class NavCompanyList
    {
        public string Name { get; set; }
        public bool Evaluation_Company { get; set; }
        public string Display_Name { get; set; }
        public bool Master_Company { get; set; }
        public bool Registry_To_Propagate { get; set; }
        public bool PBI_Company { get; set; }
        public bool International_Office { get; set; }
    }

    public partial class NavBudgetEntry
    {
        public int Entry_No { get; set; }
        public string Budget_Name { get; set; }
        public string G_L_Account_No { get; set; }
        public string Date { get; set; }
        public string Global_Dimension_1_Code { get; set; }
        public string Global_Dimension_2_Code { get; set; }
        public Decimal Amount { get; set; }
        public string Description { get; set; }
        public string Business_Unit_Code { get; set; }
        public string User_ID { get; set; }
        public string Budget_Dimension_1_Code { get; set; }
        public string Budget_Dimension_2_Code { get; set; }
        public string Budget_Dimension_3_Code { get; set; }
        public string Budget_Dimension_4_Code { get; set; }
        public string Last_Date_Modified { get; set; }
        public int Dimension_Set_ID { get; set; }
        public string Shortcut_Dimension_3_Code { get; set; }
        public string Shortcut_Dimension_4_Code { get; set; }
        public string Shortcut_Dimension_5_Code { get; set; }
        public string Shortcut_Dimension_6_Code { get; set; }
        public string Shortcut_Dimension_7_Code { get; set; }
        public string Shortcut_Dimension_8_Code { get; set; }
        public string Shortcut_Dimension_9_Code { get; set; }
        public string Shortcut_Dimension_10_Code { get; set; }
        public string Shortcut_Dimension_11_Code { get; set; }
        public string Shortcut_Dimension_12_Code { get; set; }
        public string Shortcut_Dimension_13_Code { get; set; }
        public string Shortcut_Dimension_14_Code { get; set; }
        public string Shortcut_Dimension_15_Code { get; set; }
        public string Shortcut_Dimension_16_Code { get; set; }

    }


    public partial class NavBudget2
    {
        public string key { get; set; }

        private int entryNo { get; set; }

        private bool entryNoFieldSpecified;

        public string pMBDGNAME { get; set; }

        public string pMACCNUM { get; set; }

        public string pMDATE { get; set; }

        public string pMGBLDIM1CODE { get; set; }

        public string pMGBLDIM2CODE { get; set; }

        public string pMAMOUNT { get; set; }

        public string pMDESCRIPTION { get; set; }

        public string pMBDGDIM1CODE { get; set; }

        public string pMBDGDIM2CODE { get; set; }

        public string pMBDGDIM3CODE { get; set; }

        public string pMBDGDIM4CODE { get; set; }

        public string pMBDGDIM5CODE { get; set; }

        public string pMBDGDIM6CODE { get; set; }

        public string pMBDGDIM7CODE { get; set; }

        public string pMBDGDIM8CODE { get; set; }

        public string pMBDGDIM9CODE { get; set; }

        public string pMBDGDIM10CODE { get; set; }

        public string pMBDGDIM11CODE { get; set; }

        public string pMBDGDIM12CODE { get; set; }

        public string pMBDGDIM13CODE { get; set; }

        public string pMBDGDIM14CODE { get; set; }

        public string pMMEASUREUNITCODE { get; set; }

        public string pMQUANTITY { get; set; }

        public string pMUNITCOST { get; set; }

        public string pMENTRYNMB { get; set; }

        public string pMLASTMODDT { get; set; }

        public string pMDELETED { get; set; }

        public string pMCOMPANY { get; set; }
    }

    public partial class NavBudget1
    {
        public string key { get; set; }

        private int entry_No { get; set; }

        private bool entry_NoFieldSpecified;

        public string PM_BDG_NAME { get; set; }

        public string PM_ACC_NUM { get; set; }

        public string PM_DATE { get; set; }

        public string PM_GBL_DIM1_CODE { get; set; }

        public string PM_GBL_DIM2_CODE { get; set; }

        public string PM_AMOUNT { get; set; }

        public string PM_DESCRIPTION { get; set; }

        public string PM_BDG_DIM1_CODE { get; set; }

        public string PM_BDG_DIM2_CODE { get; set; }

        public string PM_BDG_DIM3_CODE { get; set; }

        public string PM_BDG_DIM4_CODE { get; set; }

        public string PM_BDG_DIM5_CODE { get; set; }

        public string PM_BDG_DIM6_CODE { get; set; }

        public string PM_BDG_DIM7_CODE { get; set; }

        public string PM_BDG_DIM8_CODE { get; set; }

        public string PM_BDG_DIM9_CODE { get; set; }

        public string PM_BDG_DIM10_CODE { get; set; }

        public string PM_BDG_DIM11_CODE { get; set; }

        public string PM_BDG_DIM12_CODE { get; set; }

        public string PM_BDG_DIM13_CODE { get; set; }

        public string PM_BDG_DIM14_CODE { get; set; }

        public string PM_MEASURE_UNIT_CODE { get; set; }

        public string PM_QUANTITY { get; set; }

        public string PM_UNIT_COST { get; set; }

        public string PM_ENTRY_NMB { get; set; }

        public string PM_LAST_MOD_DT { get; set; }

        public string PM_DELETED { get; set; }

        public string PM_COMPANY { get; set; }
    }

    public partial class NavDimension1
    {
        public string DimensionCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Dimension_Value_Type { get; set; }
        public string Totaling { get; set; }
        public bool Blocked { get; set; }
        public string Consolidation_Code { get; set; }
        public int Indentation { get; set; }
        public string GlobalDimensionNo { get; set; }
        public string MaptoICDimensionCode { get; set; }
        public string MaptoICDimensionValueCode { get; set; }
        public int Dimension_Value_ID { get; set; }
        public string Id { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public bool MappingCatOfIntervention { get; set; }
        public string CatOfInterventionCode { get; set; }
        public string ProjectCity { get; set; }
        public string Company { get; set; }
        public string Guid { get; set; }
    }


    public partial class NavOdataLedger
    {
        public string Primary_Key { get; set; }
        public string LCY_Code { get; set; }
        public string Local_Currency_Description { get; set; }
        public string Global_Dimension_1_Code { get; set; }
        public string Global_Dimension_2_Code { get; set; }
        public string Shortcut_Dimension_3_Code { get; set; }
        public string Shortcut_Dimension_4_Code { get; set; }
        public string Shortcut_Dimension_5_Code { get; set; }
        public string Shortcut_Dimension_6_Code { get; set; }
        public string Shortcut_Dimension_7_Code { get; set; }
        public string Shortcut_Dimension_8_Code { get; set; }
        public string Shortcut_Dimension_9_Code { get; set; }
        public string Shortcut_Dimension_10_Code { get; set; }
        public string Shortcut_Dimension_11_Code { get; set; }
        public string Shortcut_Dimension_12_Code { get; set; }
        public string Shortcut_Dimension_13_Code { get; set; }
        public string Shortcut_Dimension_14_Code { get; set; }
        public string Shortcut_Dimension_15_Code { get; set; }
        public string Shortcut_Dimension_16_Code { get; set; }
    }
    public partial class NavCoa
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Search_Name { get; set; }
        public string Account_Type { get; set; }
        public string Global_Dimension_1_Code { get; set; }
        public string Global_Dimension_2_Code { get; set; }
        public string Income_Balance { get; set; }
        public string Debit_Credit { get; set; }
        public string No_2 { get; set; }
        public string Blocked { get; set; }
        public string Direct_Posting { get; set; }
        public string Reconciliation_Account { get; set; }
        public string New_Page { get; set; }
        public string No_of_Blank_Lines { get; set; }
        public string Indentation { get; set; }
        public string Last_Date_Modified { get; set; }
        public string Totaling { get; set; }
        public string Consol_Translation_Method { get; set; }
        public string Consol_Debit_Acc { get; set; }
        public string Consol_Credit_Acc { get; set; }
        public string Gen_Posting_Type { get; set; }
        public string Gen_Bus_Posting_Group { get; set; }
        public string Gen_Prod_Posting_Group { get; set; }
        public string Automatic_Ext_Texts { get; set; }
        public string Tax_Area_Code { get; set; }
        public string Omit_Default_Descr_in_Jnl { get; set; }
        public string Cost_Type_No { get; set; }
        public string Default_Deferral_Template_Code { get; set; }
        public string Parent { get; set; }
        public string Comment { get; set; }
        public string Balance_at_Date { get; set; }
        public string Net_Change { get; set; }
        public string Budgeted_Amount { get; set; }
        public string Balance { get; set; }
        public string Budget_at_Date { get; set; }
        public string Debit_Amount { get; set; }
        public string Credit_Amount { get; set; }
        public string Budgeted_Debit_Amount { get; set; }
        public string Budgeted_Credit_Amount { get; set; }

        public string French_Name { get; set; }
        public string Account_Category { get; set; }



    }
}
